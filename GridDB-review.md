# GridDB Comprehensive Code Review

> **Review Date:** April 2026  
> **Scope:** Address System, Data Classes, Server/Client Architecture, Future Use Cases

---

## Table of Contents

1. [Address System — `GridDBAddress`](#1-address-system--griddbaddress)
2. [Data Classes — `GridData`, `GridDataBlock`, `DomainInfo`](#2-data-classes--griddata-griddatablock-domaininfo)
3. [Core Database — `GridDB`](#3-core-database--griddb)
4. [Messaging Layer — `MessageData`](#4-messaging-layer--messagedata)
5. [Server/Client Systems — `GridDBServer` / `GridDBClient`](#5-serverclient-systems--griddbserver--griddbclient)
6. [Host Discovery — `GridDBHostInfo`](#6-host-discovery--griddbhostinfo)
7. [Architectural Concerns](#7-architectural-concerns)
8. [Future Use Cases and Integration Risks](#8-future-use-cases-and-integration-risks)
9. [Recommended Test Cases](#9-recommended-test-cases)
10. [Summary of Recommendations](#10-summary-of-recommendations)

---

## 1. Address System — `GridDBAddress`

### Overview

`GridDBAddress` is the central addressing class used across the database, client, and server layers. It models a four-component address string of the form `domain.sub.index.{customdata|text}`, with an optional `@hostId` suffix for remote addresses.

### Observations

#### 1.1 Mutable Public Fields

All address components (`domain`, `sub`, `index`, `custom_data`, `host`) are exposed as plain public fields. Any caller can mutate an address after construction, potentially corrupting state in dictionaries that use `GridDBAddress` as a logical key (via `ToString()`/`GetHashCode()`).

```csharp
// Current — fields are mutable from outside
public string domain;
public string sub;
public int index = -1;
public bool custom_data = false;
public long host;
```

**Recommendation:** Expose all fields as read-only properties and provide a factory method or builder for controlled construction. At a minimum, mark them with private setters.

#### 1.2 Silent Parse Failure

The constructor accepts any string. If the string does not match any of the expected patterns (4, 3, or 2 parts) all fields remain at their default values (`null`, `null`, `-1`, `false`, `0`). There is no way for a caller to distinguish a successfully parsed address from a parse failure short of checking `domain == null`.

```csharp
// If address is "bad" or empty, domain == null and sub == null silently
GridDBAddress addr = new GridDBAddress("bad_address");
// No exception, no error flag — addr.domain is null
```

**Recommendation:** Add an `IsValid` property or throw an `ArgumentException` on invalid input. `GridDB.Init()` already skips panels where `address.domain == null`, but callers that construct addresses from user input or remote data get no feedback.

#### 1.3 `Equals` NullReferenceException

The `Equals` override does not null-check its argument:

```csharp
public override bool Equals(object obj)
{
    return obj.ToString() == ToString();
}
```

Passing `null` will throw a `NullReferenceException`. The standard pattern is:

```csharp
public override bool Equals(object obj)
{
    if (obj is null) return false;
    return obj.ToString() == ToString();
}
```

#### 1.4 `ToString` / `GetAddressString` Asymmetry

`GetAddressString` always returns a 4-part address (including the data-type segment), while `ToString` can return a 2-part address (`domain.sub`) when `index < 0`. The 3-part form (`domain.sub.index` without data type) is also parsed in the constructor but is never produced by any serializer. This inconsistency makes it hard to know which callers will produce 3-part addresses and whether they round-trip correctly.

**Recommendation:** Consolidate address serialization so there is one canonical format. The 3-part form appears to be a transitional artifact that should be removed or explicitly documented.

#### 1.5 `ToBlockName` Index Limit

```csharp
public string ToBlockName()
{
    return $"{GridDB.DB_PREFIX}{domain}.{sub}.{index.ToString("D2")}";
}
```

The `"D2"` format supports a maximum index of 99. Block naming will silently become incorrect for any sub-category with 100 or more text panels. There is no guard against this in `GridDB.Add()`.

**Recommendation:** Use `"D3"` or document the 100-block limit explicitly.

#### 1.6 Commented-Out Length Field

```csharp
//public int length;
//length = parts.Length;
```

Dead code should be removed, not commented out.

---

## 2. Data Classes — `GridData`, `GridDataBlock`, `DomainInfo`

### `GridData`

#### 2.1 Mutable Static Separator Characters

```csharp
public static char BlockSeparator = '║';
public static char HeaderSeparator = '╣';
public static char HeaderKVSeparator = '╬';
public static char HeaderKVPairSeparator = '═';
```

These separators are `public static` and mutable. Any code can change them, which would silently corrupt all future parsing. Because data is persisted across sessions, even a temporary in-memory change could cause long-term data corruption.

**Recommendation:** Declare these as `public static readonly` or `const`.

#### 2.2 Inconsistent Access Control on Collections

`header` is `public`, but `blocks` and `BlocksByName` are `private`. This means callers can directly modify header key-value pairs (potentially causing `Save()` to write malformed data) but cannot iterate blocks without going through the indexer.

**Recommendation:** Make `header` read-only from outside (expose it as `IReadOnlyDictionary<string, string>`) and provide explicit mutation methods.

#### 2.3 Tight Coupling to `GridDB`

`GridData(string address, bool headerOnly)` constructor calls `GridDB.Get(address)` immediately:

```csharp
public GridData(string address, bool headerOnly = false)
{
    this.address = new GridDBAddress(address);
    ParseData(GridDB.Get(address), headerOnly);
}
```

This means `GridData` cannot be constructed, tested, or used in any context where `GridDB` is not initialised. The second constructor that accepts raw data directly (`GridData(string address, string data)`) is the better pattern. The first constructor should be avoided or documented as requiring `GridDB.Init()` to have been called.

#### 2.4 Silent Duplicate Block Handling

When two blocks share the same name, the duplicate is silently dropped and a `Warning` entry is written to the header:

```csharp
header["Warning"] = "Duplicate block name: " + b.Name;
```

Because this is stored in the header dictionary, it will persist if the data is saved back. Future reads will then surface a spurious `Warning` key, which could confuse header-aware code.

**Recommendation:** Emit the warning via `GridInfo.Echo()` instead of polluting the header. Alternatively, append a disambiguation suffix and log the issue.

#### 2.5 No Block Enumeration API

There is no `Count` property, no `IEnumerable<GridDataBlock>` interface, and no way to iterate all blocks without calling `this[int index]` in a loop with an unknown upper bound. Callers are forced to guess the block count.

**Recommendation:** Expose `public int BlockCount => blocks.Count;` and optionally implement `IEnumerable<GridDataBlock>`.

### `GridDataBlock`

#### 2.6 Mutable Static Separators (Same Issue as `GridData`)

```csharp
public static char HeaderSeparator = '┤';
public static char HeaderKVSeparator = '┼';
public static char HeaderKVPairSeparator = '─';
public static char ListSeparator = '┐';
```

Same mutability concern as `GridData` separators. Should be `readonly` or `const`.

#### 2.7 Fully Public `data` and `header` Fields

Both `data` (the raw content) and `header` (the key-value metadata) are exposed as plain public fields. Direct mutation bypasses any future validation logic.

**Recommendation:** Expose `data` as a property with a setter, and expose `header` as `IReadOnlyDictionary<string, string>` with explicit `SetHeader(string key, string value)` or constructor-based mutation.

#### 2.8 Embedded Separator Characters in Data Values

If a block's data content or a header value contains one of the separator characters (`┤`, `┼`, `─`), parsing will silently produce wrong results. There is no escaping mechanism.

**Recommendation:** Implement an escape/unescape pass around all separator characters, or switch to a length-prefixed binary format. At minimum, document that separator characters are forbidden in stored values.

#### 2.9 Unnamed Blocks

When constructed from raw data with no header, `Name` and `Type` will both be empty strings. `BlocksByName` in `GridData` skips unnamed blocks (`if (b.Name != "") BlocksByName[b.Name] = b;`), so they are only accessible via numeric index. This is an undocumented, implicit distinction that can cause data to appear to be missing.

### `DomainInfo`

#### 2.10 `MainAddress` Not Serialised

`DomainInfo.ToString()` does not include `MainAddress`:

```csharp
sb.Append("Name").Append(...).Append("Domain").Append(...).Append("BlockCount").Append(...).Append("Icon").Append(...);
// MainAddress is omitted
```

When a domain list is transmitted from server to client over IGC, the client's `DomainInfo` objects have `MainAddress == null`. Any client code that relies on `MainAddress` will fail silently after a remote connection.

**Recommendation:** Add `MainAddress` to both `ToString()` and the parsing `switch` statement, or document why it is intentionally excluded from the wire format.

#### 2.11 `Domain` Only Set Conditionally in `GridData` Constructor

```csharp
public DomainInfo(string domain, GridData dd)
{
    if (dd.header.ContainsKey("Name")) Name = dd.header["Name"];
    if (dd.header.ContainsKey("Domain")) Domain = domain; // ← Domain is set only if header has "Domain"
    ...
}
```

The guard should be `Domain = domain;` unconditionally (the `domain` parameter is the source of truth, not the header). As written, `Domain` may remain `null` for domains whose main data block lacks a `Domain` header key.

#### 2.12 Parsing Splits With `RemoveEmptyEntries` on Values

```csharp
string[] kv = part.Split(new char[] { GridDataBlock.HeaderKVPairSeparator }, StringSplitOptions.RemoveEmptyEntries);
```

Using `RemoveEmptyEntries` on a key-value split means a value that is an empty string will produce a single-element array, which is rejected by `if (kv.Length != 2) continue;`. Empty string values are silently dropped. Use `StringSplitOptions.None` with a limit of 2 for key-value splits.

---

## 3. Core Database — `GridDB`

### 3.1 Fully Static, Global State

`GridDB` is implemented entirely with static members. There is no instance, no interface, and no way to inject or replace the implementation. Consequences:

- Only one database can exist per programmable block at a time.
- Unit testing is impractical — tests have to set up actual `IMyTextPanel` instances.
- Importing `GridDB`-dependent classes (e.g., `GridData`) into another script implicitly imports all of `GridDB`'s static state.

**Recommendation:** Convert to an instance class with a default singleton for backward compatibility. Define an `IGridDB` interface so downstream code can work against an abstraction.

### 3.2 Domain Logic Hard-Coded in Database Layer

The database is aware of domain-specific conventions:

```csharp
public static bool hasShows { get { ... if (Database[domain].ContainsKey("Main") ...) } }
public static bool hasGames { get { ... if (Database[domain].ContainsKey("GameData") ...) } }
public static int showsCount { ... if (Database[domain].ContainsKey("Main")) count++; }
public static int gamesCount { ... if (Database[domain].ContainsKey("GameData")) count++; }
```

And:

```csharp
if (Database[domain][sub][0].CustomData.Contains("Main═True"))
```

The `GridDB` class should not know about shows, games, or the string literal `"Main═True"`. This is application logic that belongs in a higher-level layer.

**Recommendation:** Remove domain-type properties from `GridDB`. Let callers query `GetDomains()` and `GetSubCategories()` and apply their own filters.

### 3.3 `DB_PREFIX` and `DB_UNUSED` are Mutable Statics

```csharp
public static string DB_PREFIX = "DB:";
public static string DB_UNUSED = "Unused";
```

If any code reassigns these, every panel naming and lookup operation breaks. Declare them `const` or `static readonly`.

### 3.4 Division by Zero in `UsedPercent`

```csharp
public static float UsedPercent { get { return (float)UsedCount / (float)TotalCount * 100f; } }
```

If `TotalCount == 0` (no panels have been registered), this returns `NaN`. Any code that formats this value (e.g., `UsedPercent.ToString("0.0")`) will print `NaN` in the UI.

**Recommendation:** Guard against zero: `return TotalCount == 0 ? 0f : (float)UsedCount / TotalCount * 100f;`.

### 3.5 `GetRandom` Selects Two Independent Random Indexes

```csharp
public static string GetRandom(string domain, string sub)
{
    ... return random.Next(100) > 50
        ? Database[domain][sub][random.Next(Database[domain][sub].Count)].CustomData
        : Database[domain][sub][random.Next(Database[domain][sub].Count)].GetText();
}
```

The ternary randomly picks between `CustomData` and `GetText()`, but uses a *different* random index for each branch. If `custom_data` and `text` store different content on the same panel (which is the expected use), this returns random data from a random panel — which may be intentional. However, the two `random.Next()` calls mean the selection is not atomic: the data-type branch and the panel-index branch are independent. This should be commented to make the intent explicit.

### 3.6 `Add()` Does Not Enforce Index Ordering

```csharp
public static void Add(string domain, string sub, int index, string data)
{
    ...
    panel.CustomName = "DB:" + domain + "." + sub + "." + index.ToString("00");
    Database[domain][sub].Add(panel);   // ← appended regardless of index value
}
```

The panel is appended to the list regardless of the `index` parameter. If blocks are added out of order, list positions will not match address indexes. A subsequent `Get(domain, sub, 3, ...)` will read position 3 in the list, not the panel named `...03`.

**Recommendation:** Insert at the correct list position (or sort after insertion), and validate that the given index does not already exist.

### 3.7 `Delete` Recycles Panels But Does Not Clear Screen Text

```csharp
panel.CustomName = DB_PREFIX + DB_UNUSED;
panel.CustomData = "";
panel.WriteText("");
```

This is adequate for most cases, but if a panel is set to show its text content, recycling it and clearing text while still visible could produce a momentary blank screen. The visual state (content type, font, etc.) is not reset. This is likely acceptable for the current use case but worth noting for scenarios where panels are viewer-facing.

### 3.8 `System.Web` Import

`GridDB.cs` includes `using System.Web;`, which is not available in the Space Engineers Mod API and would cause a compile error in the SE scripting environment. This import appears to be a leftover from development in a standard .NET project context.

**Recommendation:** Remove `using System.Web;` from `GridDB.cs`.

---

## 4. Messaging Layer — `MessageData`

### 4.1 Separators Hardcoded in `ParseData`

The static separator fields `KVPairSeparator` and `DataSeparator` are defined but not used in `ParseData`:

```csharp
public static char KVPairSeparator = '╪';
public static char DataSeparator = '╫';

// ParseData uses hardcoded literals, not the static fields:
string[] parts = rawData.Split(new char[] { '╫' }, ...);
string[] kv = part.Split(new char[] { '╪' }, ...);
```

If someone changes `KVPairSeparator` (it is mutable), parsing is not affected — a misleading design.

**Recommendation:** Use the static fields in `ParseData`, or make them `const`/`readonly` and use them consistently.

### 4.2 Embedded Separator in Values Corrupts Parsing

The same concern as in `GridDataBlock`: if any value in a `MessageData` dictionary contains `'╫'` or `'╪'`, the round-trip through `ToString()`/`ParseData()` will corrupt data. There is no escaping.

**Recommendation:** Use a length-prefixed format or escape the separator characters before serialising values.

### 4.3 KV Split Removes Empty Entries

```csharp
string[] kv = part.Split(new char[] { '╪' }, StringSplitOptions.RemoveEmptyEntries);
if (kv.Length == 2) { data[kv[0]] = kv[1]; }
```

Empty string values are silently dropped. Change to `StringSplitOptions.None` with a count of 2 to preserve empty values.

### 4.4 Fallback to Plain String for Non-Structured Data

```csharp
if (!rawData.Contains("╫") && !rawData.Contains("╪"))
{
    data["Data"] = rawData;
    return;
}
```

This fallback is useful but means that a `MessageData` constructed from a plain string (e.g., a block name that happens to have no separators) will silently be treated as structured data with a single `"Data"` key. This implicit conversion could confuse callers expecting other keys to be present.

---

## 5. Server/Client Systems — `GridDBServer` / `GridDBClient`

### 5.1 `GridDBServer` Is Fully Static

`GridDBServer.Init()` registers IGC listeners as static callbacks. Only one server can exist per programmable block. While this matches the SE scripting model, it means the server cannot be tested, mocked, or configured differently per deployment.

**Recommendation:** Consider moving the announcement timing configuration to a settings class so it can be adjusted without modifying source code.

### 5.2 No Authentication or Access Control

The server responds to any IGC message from any source. A malicious or buggy client can request the full domain list, all block addresses, and all data without restriction. For a game modding context this may be acceptable, but it should be explicitly acknowledged.

### 5.3 No Error Responses

When the server cannot find a requested address, it sends an empty string in the `Data` field:

```csharp
response["Data"] = GridDB.Get(msg["Address"]);
```

`GridDB.Get()` returns `""` for missing addresses. The client receives a `GridData` object with empty content and no indication that the address was not found versus found but empty. A dedicated error mechanism (e.g., a `Status` field with values `"OK"` / `"NotFound"`) would allow clients to distinguish these cases.

### 5.4 No Timeout or Retry in Client

Once `RequestData` is called, `DownloadingAddress` is set and no further downloads can begin until `DataReceived` fires:

```csharp
public void RequestData(GridDBAddress address)
{
    if (DownloadingAddress != "") return; // already downloading
    ...
}
```

If the server does not respond (e.g., the server block is powered off), the client locks permanently. There is no timeout, no retry count, and no way for the caller to cancel or reset the download state.

**Recommendation:** Track a download start time and expose a configurable timeout. After expiry, reset `DownloadingAddress` and invoke a configurable error callback.

### 5.5 All Instances Share Static IGC Handlers

In the `GridDBClient` constructor:

```csharp
GridInfo.AddMessageHandler(DOMAINLIST, DomainListRecieved);
GridInfo.AddMessageHandler(DOMAINBLOCKS, DomainBlocksReceived);
GridInfo.AddMessageHandler(GRIDDATA, GridDataReceived);
```

If multiple `GridDBClient` instances are created (e.g., one per screen), each registers its own handler for the same IGC tags. Depending on how `GridInfo.AddMessageHandler` works, this may result in all clients receiving all messages, filtered only by `ClientName`. A message intended for client A will be silently discarded by client B only if `ClientName` differs. If two clients are created in the same clock tick, they share the same `ClientName` (since it is `Ticks`-based) and will cross-contaminate responses.

**Recommendation:** Use a GUID or counter-based suffix for `ClientName` uniqueness. Centralise message dispatch in a static router rather than registering per-instance handlers.

### 5.6 Protocol String Constants Are Mutable

```csharp
public static string GRIDDBHOST = "GridDBHost";
public static string DOMAINLIST = "DomainList";
// ...
```

These are `public static string` (mutable), meaning they could be reassigned. Declare them `const` or `static readonly`.

### 5.7 Typo in Method Name

```csharp
void DomainListRecieved(...)   // "Recieved" should be "Received"
```

This appears in both `GridDBClient` and `DataManager`. While it has no functional impact, it violates code style and makes searching for this method by correct spelling fail.

### 5.8 `ConnectToHost(string hostName)` Silently Fails

```csharp
public void ConnectToHost(string hostName, bool getDomainList = true)
{
    GridDBHostInfo host = GridDBHosts.FirstOrDefault(h => h.HostName == hostName);
    if (host != null)
    {
        ConnectToHost(host, getDomainList);
    }
    // else: silently does nothing
}
```

If the host name is not yet in the discovered list, the call is silently ignored. The caller has no way to know whether the connection was attempted. **Recommendation:** Return a `bool` indicating success, or invoke an error callback.

### 5.9 Host Find Rate Limiting Does Not Account for Initialization

```csharp
public static void FindHosts()
{
    if ((DateTime.Now - lastHostFindTime).TotalMinutes < 1) return;
    ...
}
```

`GridDBClient.Init()` calls `FindHosts()` once. If `Init()` is called within a minute of a previous `Init()` (e.g., due to a script recompile), the broadcast is silently suppressed. The caller gets no feedback that host discovery was skipped.

---

## 6. Host Discovery — `GridDBHostInfo`

### 6.1 `Me` Property Has Side Effects

```csharp
public static GridDBHostInfo Me
{
    get
    {
        if (_me == null)
        {
            ...
            if (!GridInfo.Me.CustomData.Contains("HostName"))
            {
                GridInfo.Me.CustomData = $"HostName={hostName}\n" + GridInfo.Me.CustomData;
            }
            ...
        }
        return _me;
    }
}
```

Accessing `GridDBHostInfo.Me` as a getter modifies `GridInfo.Me.CustomData`. This is an unexpected side effect of a property getter and can corrupt the programmable block's custom data if called before the data is properly set up.

**Recommendation:** Move the `CustomData` initialisation to an explicit `Init()` method.

### 6.2 `_me` Cache Is Never Invalidated

Once `_me` is set, it persists for the lifetime of the script. If the host name is changed in `CustomData` after initialisation, the stale cached value is used. An explicit `Reset()` or re-reading from `CustomData` on each access (with caching per tick) would be safer.

### 6.3 Reuses `GridData` Separators for Own Serialization

`GridDBHostInfo.ToString()` uses `GridData.HeaderKVPairSeparator` (`═`) and `GridData.HeaderKVSeparator` (`╬`) for its own wire format. This creates an implicit dependency on `GridData`'s separator values. If `GridData`'s separators are ever changed, `GridDBHostInfo` serialisation would break.

**Recommendation:** Define host-info-specific separators or use a dedicated serialisation utility.

---

## 7. Architectural Concerns

### 7.1 `partial class Program` Namespace Locks Everything to SE Scripting

All classes are nested inside `partial class Program` inside `namespace IngameScript`. This is required for Space Engineers programmable blocks, but it means:

- No class can be imported or tested outside the SE context without significant refactoring.
- Third-party scripts that want to reuse `GridDBAddress` or `GridData` must include the entire `Program` class.
- Static analysis and IDE tooling may struggle with the partial-class pattern.

**Recommendation:** Where feasible, keep the SE-specific bootstrapping in `Program` but move core logic into standalone classes that do not depend on `IngameScript` globals.

### 7.2 No Interfaces Defined

None of the core classes implement interfaces. This makes:

- Mocking for tests impossible without real SE API objects.
- Swapping implementations (e.g., an in-memory test database) requires changing every call site.
- Extension by subclassing risky because the base classes are not designed for it.

**Recommendation:** Define `IGridDB`, `IGridDBAddress`, and `IGridDBClient` interfaces for the primary abstractions.

### 7.3 Unicode Box-Drawing Characters as Separators

The system uses multiple sets of Unicode box-drawing characters as data separators:

| Context | Chars |
|---------|-------|
| `GridData` outer format | `║`, `╣`, `╬`, `═` |
| `GridDataBlock` inner format | `┤`, `┼`, `─`, `┐` |
| `MessageData` (IGC) | `╫`, `╪` |
| `GridDBHostInfo` | reuses `GridData` chars |

These characters are visually distinctive in the SE custom data editor, which is a deliberate design choice. However:

- There is no escaping mechanism. Data containing any of these characters will corrupt the database or message parsing.
- Different layers use overlapping but distinct character sets, making the overall format non-obvious.
- The character sets are not documented in a single location.

**Recommendation:** Add a guard that validates (or strips) reserved separator characters from all user-supplied values before they are written to storage or transmitted. Document the complete separator table in a single comment block or README section.

### 7.4 Coupling Between Layers

`GridDBServer` directly calls `GridDB` (static), making it impossible to run a server without a fully initialised `GridDB`. `GridData` constructors call `GridDB.Get()`. `GridDBAddress` references `GridDB.DB_PREFIX`. This tight coupling creates an implicit initialization order requirement that is not enforced or documented:

1. `GridInfo.Init()` must be called first.
2. `GridBlocks` must be populated before `GridDB.Init()`.
3. `GridDB.Init()` must be called before constructing `GridData` via the first constructor.
4. `GridDBClient.Init()` must be called before `GridDBServer.Init()` if both run on the same block.

Failure to follow this order produces silent bugs, not clear errors.

### 7.5 No Event/Error Reporting Mechanism

`GridInfo.Echo()` calls are commented out throughout the codebase. During normal operation there is no observable logging of errors, misconfigurations, or warnings. The commented-out lines suggest they were used during development. A structured logging approach (toggleable verbosity) would make diagnosing in-game issues much easier.

---

## 8. Future Use Cases and Integration Risks

### 8.1 Direct Import by External Scripts

If another SE script imports `GridDB`, `GridData`, or `GridDBAddress`, it implicitly inherits:

- The entire static `GridDB` state (any write from one script affects all scripts sharing the block).
- The `GridDB.DB_PREFIX` convention — changing this in one script would break panel discovery for all.
- The separator character conventions — data written by one script version must be readable by all other script versions.

**Risk:** Breaking changes to the separator characters or address format are non-backward-compatible and will silently corrupt existing stored data.

**Mitigation:** Version the data format (a version header in `GridData.header`) and provide a migration path.

### 8.2 Subclassing Core Data Classes

`GridData` and `GridDataBlock` are concrete, non-sealed classes with no virtual methods. Subclassing them to add domain-specific behaviour is possible but fragile:

- `GridData.ParseData()` is public but re-initialises the object's state — calling it on a subclass from a base-class constructor will bypass subclass setup.
- `GridDataBlock.ToString()` serialises only what the base class knows; subclass additions would not be serialised without overriding `ToString()`.

**Recommendation:** Mark extension points with `virtual`, or make the classes `sealed` and compose rather than inherit.

### 8.3 Client Class as a Public API

`GridDBClient` as a public API for external scripts has the following hidden dependencies:

- `GridInfo.IGC` must be available and initialised (the SE IGC system).
- `GridDBClient.Init()` must have been called (registers the host broadcast listener).
- `GridDBClient.GridDBHosts` is shared static state — all client instances see the same host list.
- All callbacks (`DomainInfoReceived`, `DataReceived`) are synchronous; in the SE scripting model, they fire during the script's execution tick. Callers must not block inside callbacks.

**Risk:** An external script that creates a `GridDBClient` without calling `GridDBClient.Init()` first, or that creates multiple clients expecting isolated host lists, will encounter subtle multi-client bugs.

**Mitigation:** Document the required call sequence. Consider an `IsInitialised` guard that throws a clear exception if Init has not been called.

### 8.4 High Concurrency / High Block Count

Space Engineers scripts run on a single thread per tick, so true concurrency is not a concern. However, scripts that process many ticks quickly while large downloads are in progress may encounter:

- Stale `DownloadingAddress` if a response is dropped.
- `GetDomainAddresses` iterating all blocks in a domain — for large domains (hundreds of panels), this is called on every download request and could impact tick timing.

---

## 9. Recommended Test Cases

The following scenarios should be specifically validated:

| # | Scenario | Expected Result |
|---|----------|-----------------|
| T1 | Construct `GridDBAddress` from each supported format (4-part, 3-part, 2-part, with `@host`) | All fields parsed correctly |
| T2 | Construct `GridDBAddress` from an empty string | `domain == null`, `IsValid == false` (once added) |
| T3 | Construct `GridDBAddress` from a malformed string (e.g., `"one"`, `"a.b.c.d.e"`) | No crash; `IsValid == false` |
| T4 | `GridDBAddress.Equals(null)` | Returns `false`, no exception |
| T5 | Round-trip: `new GridDBAddress(addr.ToString()).ToString() == addr.ToString()` for all formats | Passes |
| T6 | `GridDB.UsedPercent` when `TotalCount == 0` | Returns `0.0f`, no division by zero |
| T7 | `GridDB.Set` on a non-existent address without `addifnew` | Throws exception |
| T8 | `GridDB.Add` inserting blocks out of order | Subsequent `Get` by index returns correct panel |
| T9 | `GridData` round-trip: parse → modify header → `ToString()` → re-parse | Header values preserved |
| T10 | `GridData` with duplicate block names | Warning in header; first block accessible; duplicate dropped |
| T11 | `GridDataBlock` with a value containing a separator character (`┤`) | Detected and rejected or escaped |
| T12 | `MessageData` with a value containing `'╫'` | Round-trip fails (documents current limitation) |
| T13 | `MessageData.ParseData` with an empty string | `data` dictionary is empty, no crash |
| T14 | `GridDBClient.RequestData` called twice without response | Second call blocked; no crash |
| T15 | `GridDBClient` created after `GridDBClient.Init()` is not called | Clear error or `IsInitialised == false` |
| T16 | `DomainInfo` serialise/deserialise including `MainAddress` | `MainAddress` preserved (once fix applied) |
| T17 | `GridDBHostInfo.Me` accessed before `GridInfo` is ready | Error, not silent data corruption |
| T18 | `GridDB.GetRandom` on a domain with a single panel | Returns that panel's content, not empty |
| T19 | `GridDB.Delete` on a domain that does not exist | No-op, no crash |
| T20 | Two `GridDBClient` instances created in the same tick | Unique `ClientName` for each; no cross-contamination |

---

## 10. Summary of Recommendations

### High Priority

| # | Issue | Recommendation |
|---|-------|----------------|
| H1 | Mutable static separator characters | Make all separator fields `const` or `static readonly` |
| H2 | `GridDBAddress` mutable public fields | Expose as read-only properties |
| H3 | `GridDBAddress` silent parse failure | Add `IsValid` property or throw on bad input |
| H4 | `Equals` NullReferenceException | Add null-check in `Equals` |
| H5 | `GridDB.UsedPercent` division by zero | Guard against `TotalCount == 0` |
| H6 | `DomainInfo.MainAddress` not serialised | Add `MainAddress` to `ToString()` and parser |
| H7 | Client download never times out | Add configurable timeout and error callback |
| H8 | `System.Web` import in `GridDB.cs` | Remove unused import |
| H9 | `DomainInfo` Domain not set unconditionally | Set `Domain = domain` without the `ContainsKey` guard |

### Medium Priority

| # | Issue | Recommendation |
|---|-------|----------------|
| M1 | Protocol constants are mutable strings | Declare `const` |
| M2 | No error response from server | Add a `Status` field to server responses |
| M3 | Separator chars not escaped | Validate/strip reserved chars before storage or transmission |
| M4 | `MessageData.ParseData` ignores static separator fields | Use static fields in parsing |
| M5 | `GridDBHostInfo.Me` has side effect | Move `CustomData` init to an explicit `Init()` |
| M6 | `GridDB` has domain-type awareness | Remove `hasShows`, `hasGames`, etc. from database layer |
| M7 | No block enumeration API in `GridData` | Expose `BlockCount` and `IEnumerable<GridDataBlock>` |
| M8 | `ToBlockName` D2 format limits to 99 | Use D3 or document and enforce the limit |
| M9 | Typo `DomainListRecieved` | Rename to `DomainListReceived` |
| M10 | `ConnectToHost(string)` silent failure | Return `bool` success or invoke error callback |

### Low Priority / Future Improvements

| # | Issue | Recommendation |
|---|-------|----------------|
| L1 | All classes are static singletons | Convert to instance classes with interfaces |
| L2 | No logging / Echo commented out | Add toggleable structured logging |
| L3 | Initialization order not documented or enforced | Document or enforce call sequence |
| L4 | No data format versioning | Add version header to `GridData` for migration |
| L5 | Dead code (commented-out `length` field) | Remove |
| L6 | `GetRandom` two independent random calls | Comment intent or make atomic |
| L7 | `GridData` constructor calls `GridDB.Get()` directly | Prefer injection of raw data string |
| L8 | `_me` cache never invalidated | Add explicit `Reset()` method |
| L9 | `GridDB.Add` does not sort inserted panels | Insert at correct index position |
| L10 | `header` in `GridData` is public mutable dict | Expose as `IReadOnlyDictionary` with explicit setters |
