# GridDB - Space Engineers Database Framework

A comprehensive framework for managing and displaying data in Space Engineers using text panels as storage and providing a rich GUI framework for interactive applications.

## Core Components

### GridDB System

The GridDB system provides distributed data storage using text panels as the storage medium, with support for networked data sharing between grids.

#### Key Classes

- **`GridDB`** - Core database class that manages data stored across text panels on a subgrid
  - Organizes data into domains and sub-categories (e.g., "Shows/Main", "Games/GameData")
  - Provides statistics and metrics (used/unused blocks, show/game counts)
  - Handles automatic storage allocation and data persistence

- **`GridDBAddress`** - Addressing system for locating specific data
  - Format: "domain.sub.index" (e.g., "Movies.Main.5")
  - Enables precise data retrieval across the database

#### Data Classes

- **`GridData`** - Represents a collection of data blocks within a domain
  - Parses and manages header information and data blocks
  - Provides indexed access to data by name or position
  - Handles serialization/deserialization of stored data

- **`GridDataBlock`** - Individual data unit within a GridData collection
  - Stores name, type, and raw data content
  - Supports various data types and content formats

#### Remote/Network Components

- **`GridDBServer`** - Provides network services for sharing data between grids
  - Responds to domain list requests, block lists, and data requests
  - Uses Space Engineers' Inter-Grid Communication (IGC) system
  
- **`GridDBClient`** - Client for accessing remote GridDB instances
  - Discovers available database hosts on the network
  - Downloads domain information and data from remote servers
  - Supports asynchronous data operations

- **`GridDBHostInfo`** - Information about remote database hosts
  - Tracks host ID, name, and available domains
  - Used for network discovery and connection management

### Screen Framework

A layered graphics framework for creating interactive displays on Space Engineers screens.

#### Core Screen Classes

- **`Screen`** - Base screen management class
  - Encapsulates an `IMyTextSurface` for drawing
  - Manages sprite layers and viewport calculations
  - Handles drawing frame management and sprite rendering

- **`ScreenSprite`** - Basic sprite implementation
  - Supports text, texture, and custom sprite types
  - Provides positioning with anchor points and alignments
  - Handles viewport transformations and collision detection

#### Advanced Sprite Types

- **`TextSprite`** - Multi-line text rendering with advanced formatting
- **`TextureSprite`** - Image/texture rendering with rotation and scaling
- **`RasterSprite`** - Pixel-perfect graphics using monospace font characters
  - Converts pixel coordinates to screen coordinates
  - Supports custom pixel-based graphics and colors
  - Used for detailed icons and low-resolution images

#### Interface System

- **`IScreenSprite`** - Base interface for all drawable objects
- **`IScreenSpriteProvider`** - Interface for objects that provide sprites to screens

### ScreenApp Framework

A GUI application framework built on top of the Screen system, providing structured layouts and user interaction.

#### Core App Classes

- **`ScreenApp`** - Base class for interactive screen applications
  - Extends Screen with input handling and application lifecycle
  - Manages game input, sound blocks, and app identification
  - Provides foundation for building complex user interfaces

- **`ScreenAppSeat`** - Manages application instances tied to specific game seats/controllers
  - Associates applications with physical pilot seats or cockpits
  - Handles app launching, switching, and lifecycle management
  - Supports multiple apps per seat with switching capabilities

#### Layout System (LayoutGUI)

- **`LayoutArea`** - Base container for organizing UI elements
  - Supports flexible width/height sizing
  - Manages margins, padding, and child element positioning
  - Provides automatic layout calculation and updates

- **`HorizontalLayoutArea`** - Horizontal arrangement of UI elements
- **`LayoutItem`** - Individual UI element with positioning and sizing

#### Interactive Components

- **`LayoutButton`** - Clickable button with text and icon support
  - Supports both text-only and icon+text configurations
  - Handles click events and visual feedback
  - Integrates with the interaction system

- **`LayoutMenu`** - Container for multiple interactive menu items
  - Automatically manages button layout and spacing
  - Supports dynamic menu item addition
  - Handles menu item selection and navigation

- **`InteractableGroup`** - Base class for managing groups of interactive elements
- **`IInteractable`** - Interface for elements that can receive input

#### Application Layout Components

- **`AppLayout`** - Complete application layout with header, content, and footer
  - Manages header with app title and file information
  - Provides content areas for main application functionality
  - Includes footer with key prompt information

- **`AppHeader`** - Application title bar showing app name and current file
- **`AppFooter`** - Bottom bar showing available keyboard shortcuts

### GridInfo System

A global state management and inter-script communication system.

#### Key Features

- **Variable Management** - Global variables with change notifications
- **Message Handling** - IGC message routing and processing
- **Script Communication** - Inter-programmable block communication
- **Grid Context** - Maintains references to grid terminal system, IGC, and programmable block

#### Core Classes

- **`GridInfo`** - Main state management class
  - Provides variable storage with change notifications
  - Handles IGC message routing and broadcasting
  - Manages script lifecycle and persistence

- **`GridBlocks`** - Block discovery and management utilities
  - Caches references to commonly used block types
  - Provides address-based block lookup for ScreenAppSeat
  - Handles automatic text panel naming and organization

- **`MessageData`** - Structured message format for inter-script communication
  - Key-value pair based message format
  - Support for client identification and addressing
  - Used throughout the remote database system

## Current Implementation Status

### Working Components

- ✅ **Core GridDB** - Fully functional database with local storage
- ✅ **Screen Framework** - Complete graphics system with sprite support
- ✅ **Basic ScreenApp** - Application framework with input handling
- ✅ **Layout System** - Functional UI layout and container system
- ✅ **Interactive Elements** - Buttons, menus, and basic interaction
- ✅ **GridInfo** - State management and communication system
- ✅ **Network Communication** - Basic client/server for data sharing

### Applications

- ✅ **DataManager** - Database browsing and management interface
- 🚧 **WebBrowser** - Basic web-like content viewing system
- 🚧 **Advanced UI Components** - Scroll areas, complex layouts (partial)

### Framework State

The ScreenApp and GridDB frameworks are in a mature, functional state with most core features implemented. The system provides:

1. **Robust Data Storage** - Reliable text panel-based database with network sharing
2. **Rich Graphics** - Multi-layer sprite system with advanced rendering options
3. **Interactive GUI** - Complete application framework with layout management
4. **Modular Design** - Easy to extend with new applications and components
5. **Network Capabilities** - Distributed database access across multiple grids

The framework is suitable for building complex Space Engineers automation systems, HUD displays, and interactive control panels. The modular design allows developers to use individual components (Screen, GridDB, GridInfo) independently or as a complete application framework.

## Usage

### Getting Started

Copy the scripts from the `Common` folder into your Space Engineers programmable block script. The `Program.cs` constructor shows the standard initialization order:

```csharp
public Program()
{
    GridInfo.Init("GridDB", this, UpdateFrequency.Update10);
    GridBlocks.Init();
    GridDB.Init();
    GridDBServer.Init();
    GridDBClient.Init();
    ProgScreen.Init();
    ScreenAppSeat.Init();
    // register your apps here
    DataManager.Init();
}
```

---

> ⚠️ **Important Configuration Note**
>
> Place the programmable block running your GridDB script on its **own dedicated sub-grid** (connected via rotor or hinge). If the programmable block is on your main grid it will discover and claim **all** text panels on that grid as database storage blocks. Keeping it on a separate sub-grid limits block discovery to only the panels on that sub-grid.

---

### GridDB System

#### Setting Up Storage Blocks

GridDB uses text panels as its storage medium. Name each text panel you want to use as a database block with the prefix `DB:`:

```
DB:Shows.Main.00
DB:Shows.Main.01
DB:Games.GameData.00
DB:Unused          ← available for allocation
```

The naming format is `DB:<domain>.<sub>.<index>` (two-digit zero-padded index). Any panel named `DB:Unused` is treated as a free block available for allocation.

When `GridDB.Init()` is called it scans all text panels on the grid and populates the internal database from their names.

#### GridDBAddress Format

Addresses identify specific data locations using dot notation:

| Format | Example | Meaning |
|--------|---------|---------|
| `domain.sub.index.customdata` | `Shows.Main.0.customdata` | Custom data field of block 0 |
| `domain.sub.index.text` | `Shows.Main.0.text` | Text field of block 0 |
| `domain.sub.index` | `Shows.Main.0` | Block 0 (defaults to text) |
| `domain.sub` | `Shows.Main` | Random block from sub-category |
| `domain.sub.index@hostId` | `Shows.Main.0.customdata@12345` | Block on a remote host |

Addresses map directly to text panel custom names via `GridDBAddress.ToBlockName()`, e.g. `Shows.Main.0` → `DB:Shows.Main.00`.

#### Reading and Writing Data

```csharp
// Read data
string data = GridDB.Get("Shows.Main.0.customdata");

// Write data (block must already exist)
GridDB.Set("Shows.Main.0.customdata", myData);

// Write data, creating the block if it doesn't exist
GridDB.Set("Shows.Main.0.customdata", myData, addifnew: true);

// Delete an entire domain (frees all its blocks back to Unused)
GridDB.Delete("Shows");

// List all domains
List<string> domains = GridDB.GetDomains();
```

#### GridData Format

`GridData` provides structured storage within a single text panel using special Unicode separator characters to avoid conflicting with typical text content.

**Block layout (stored in a text panel):**

```
Key1═Value1╬Key2═Value2╣DataBlock1║DataBlock2║DataBlock3
└──────────────────────┘└──────────────────────────────┘
       header                        data
```

| Separator | Character | Role |
|-----------|-----------|------|
| `╣` | `HeaderSeparator` | Divides the header section from the data section |
| `╬` | `HeaderKVSeparator` | Separates key-value pairs in the header |
| `═` | `HeaderKVPairSeparator` | Separates a key from its value |
| `║` | `BlockSeparator` | Separates individual data blocks |

Each `GridDataBlock` inside a `GridData` record uses its own inner separators:

| Separator | Character | Role |
|-----------|-----------|------|
| `┤` | `HeaderSeparator` | Divides the block header from its data content |
| `┼` | `HeaderKVSeparator` | Separates key-value pairs within the block header |
| `─` | `HeaderKVPairSeparator` | Separates a key from its value in the block header |
| `┐` | `ListSeparator` | Separates list items encoded in a single field |

Example usage:

```csharp
GridData data = new GridData("Shows.Main.0.customdata");
string title = data.header["Name"];
GridDataBlock firstBlock = data[0];         // by index
GridDataBlock namedBlock = data["Episode1"]; // by name
```

---

### Client-Server Model

GridDB supports sharing data across different grids using Space Engineers' Inter-Grid Communication (IGC) system.

#### GridDBServer (host side)

Add `GridDBServer.Init()` to your host programmable block. The server automatically:

- Broadcasts its presence so clients can discover it.
- Responds to domain-list requests from clients.
- Responds to block-list and data-download requests.

No further configuration is required; the server handles all incoming IGC messages automatically.

#### GridDBClient (client side)

```csharp
// Create a client (clientName is used to tag requests/responses)
GridDBClient client = new GridDBClient("MyClient");

// The client broadcasts a discovery request on init.
// GridDBClient.GridDBHosts is populated as hosts respond.

// Connect to a discovered host by name and request its domain list
client.DomainInfoReceived += (domains) => {
    foreach (var domain in domains)
        Echo(domain.Domain + " - " + domain.BlockCount + " blocks");
};
client.ConnectToHost("SomeHostName");

// Once connected, request the block list for a specific domain
client.DomainAddressReceived += (domain, blocks) => {
    // blocks is a sorted list of GridDBAddress for all panels in that domain
};
client.RequestDomainBlocks("Shows");

// Download individual data blocks from the remote host
client.DataReceived += (gridData) => {
    GridDB.Set(gridData.address, gridData.ToString(), addifnew: true);
};
client.RequestData(someGridDBAddress);
```

> **Note:** `RequestData` processes one block at a time. Call it again inside `DataReceived` to download the next block in a queue (see `DownloadWindow` for a reference implementation).

---

### Screen App System

#### Overview

The ScreenApp framework provides a GUI application model built on top of the Screen/Sprite rendering system. Each app renders onto a specific `IMyTextSurface` and receives game controller input through the associated `ScreenAppSeat`.

#### Creating a Screen App

1. Subclass `ScreenApp`.
2. Register the app in the static `AvailableApps` dictionary so the seat can launch it by name.
3. Call `ScreenAppSeat.GetSeat(address, rootApp)` to bind a seat to the app.

```csharp
public class MyApp : ScreenApp
{
    public static void Init()
    {
        AvailableApps["My App"] = (seat) => new MyApp(seat);
        ScreenAppSeat.GetSeat("my_seat_address", "My App");
    }

    public MyApp(ScreenAppSeat seat) : base(seat, "My App")
    {
        // build your UI layout here
    }

    public override void Update(string argument)
    {
        // handle input and update display
        base.Update(argument);
    }
}
```

The `address` string passed to `GetSeat` is an arbitrary tag (e.g. `"artist_desk"`) shared by:
- The programmable block's text surface (identified by `desktop:artist_desk`).
- The game controller/cockpit seat (e.g. `tv:artist_desk`).
- Any associated sound blocks (`sfx:artist_desk`).

`GridBlocks` resolves these tagged names to actual game blocks automatically.

#### ScreenAppSeat Focus Management

`ScreenAppSeat` maintains a **stack** of focused app IDs. The top of the stack is the currently active app.

```csharp
// Switch to a different local app (creates it if not already open)
seat.CurrentAppId = new ScreenAppId("Another App");

// Switch to an app running in a different script (sends an IGC FocusApp message)
seat.CurrentAppId = new ScreenAppId("LauncherHub", "GameEditor");

// Close the current app and return focus to the previous one
seat.CloseApp();
```

When `CurrentAppId` is set to an app hosted in another programmable block (`Local == false`), the seat sends a `FocusApp` IGC message to that script so it can take control of the display. The receiving script handles this via `ScreenAppSeat.HandleMessage`.

#### App Transitions Between Scripts

To navigate from an app in one script to an app in another script:

```csharp
// Inside a ScreenApp — close this app and open "LauncherHub" in the "GameEditor" script
seat.CurrentAppId = new ScreenAppId("LauncherHub", "GameEditor");
```

`ScreenAppId` encodes both the app name and the host script's entity ID:

```
"LauncherHub@<hostEntityId>"
```

If no `@` suffix is present, the current script's own entity ID is assumed (local app).

#### Main Loop

`ScreenAppSeat.Init()` registers a main-loop handler. Each call to `Main` advances exactly **one** seat's active app, cycling through all seats in round-robin order. This keeps instruction counts predictable when many apps are running simultaneously.

---

### Desktop App Wrapper

> 🚧 **In Development** — The desktop script and desktop ScreenApp are still actively being developed. Features described below represent the current state and may change.

The desktop app wrapper allows applications to be launched directly from a desktop-style launcher script. It wraps a `ScreenApp` to provide:

- An app icon and title on a desktop grid surface.
- Click-to-launch behavior that creates the target app in the current seat.

**Currently supported:**
- Registering apps with `AvailableApps` for on-demand instantiation.
- Switching focus to an app running in another programmable block via `ScreenAppId`.

**Planned / not yet supported:**
- A dedicated desktop grid launcher UI with icon grid layout.
- Persistent app state between script reloads.

---

### Example: DataManager Screen App

`DataManager` is a fully working example that demonstrates menu navigation, local data management, and remote data downloading.

#### Class Overview

```csharp
public class DataManager : ScreenApp
{
    public static void Init()
    {
        // Register this app so seats can launch it by name
        AvailableApps["Data Manager"] = (seat) => new DataManager(seat);
        ScreenAppSeat.GetSeat("Test.Seat", "Data Manager", "Data Manager");
    }

    public DataManager(ScreenAppSeat seat) : base(seat, "Data Manager")
    {
        // Side menu: Local Data | IGC Data | Close
        sideMenu.AddMenuItem(new Vector2(130, 40), "Local Data", "local");
        sideMenu.AddMenuItem(new Vector2(130, 30), "IGC Data",   "remote");
        sideMenu.AddMenuItem(new Vector2(130, 30), "Close",       "close");
        sideMenu.OnClick += MenuItemClicked;

        // GridDBClient for remote operations
        dBClient = new GridDBClient("DatManCli");
        dBClient.DomainInfoReceived    += RemoteDomainListReceived;
        dBClient.DomainAddressReceived += DomainBlocksRecieved;
    }

    void MenuItemClicked(IInteractable item)
    {
        if      (item.Id == "local")    { /* show local domain list */ }
        else if (item.Id == "remote")   { /* show remote host list */ }
        else if (item.Id == "Download") { dBClient.RequestDomainBlocks(item.Value); }
        else if (item.Id == "close")    { seat.CurrentAppId = new ScreenAppId("LauncherHub", "GameEditor"); }
    }
}
```

#### Deleting a Local Dataset

Deletion is triggered from `GridDBDomainItemLayout` via an event chain:

```csharp
// GridDB/App/DataManager/local/GridDBDomainItemLayout.cs
void FileActionClick(IInteractable source)
{
    if (source.Id == "Delete")
    {
        OnDelete?.Invoke(this);   // notify parent (DomainListLayout calls GridDB.Delete)
        OnClick?.Invoke(source);  // bubble up the event chain
        return;
    }
    // ...
}
```

#### Downloading a Remote Dataset

When the user selects a remote domain and clicks **Download**, `DataManager` calls `dBClient.RequestDomainBlocks(domain)`. Once the block list arrives, a `DownloadWindow` is created:

```csharp
void DomainBlocksRecieved(DomainInfo domain, List<GridDBAddress> blocks)
{
    downloadWindow = new DownloadWindow(this, Size * 0.1f, Size * 0.8f, dBClient, domain);
    selectedItem = downloadWindow;
    selectedItem.IsFocused = true;
    downloadWindow.OnBack += CloseWindow;
}
```

`DownloadWindow` (see `GridDB/App/DataManager/Remote/DownloadWindow.cs`) queues all blocks and downloads them one at a time, calling `GridDB.Set(..., addifnew: true)` as each arrives and updating a progress indicator.
