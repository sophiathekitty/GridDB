using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //-----------------------------------------------------------------------
        // GridDB
        //-----------------------------------------------------------------------
        public class GridDB
        {
            //-----------------------------------------------------------------------
            // fields
            //-----------------------------------------------------------------------
            static Dictionary<string, Dictionary<string, List<IMyTextPanel>>> Database = new Dictionary<string, Dictionary<string, List<IMyTextPanel>>>();
            static List<IMyTextPanel> Unused = new List<IMyTextPanel>();
            public static bool hasData { get { return Database.Count > 0; } }
            public static bool hasShows
            {
                get
                {
                    foreach (string domain in Database.Keys)
                    {
                        if (Database[domain].ContainsKey("Main") && Database[domain]["Main"].Count > 0) return true;
                    }
                    return false;
                }
            }
            public static bool hasGames
            {
                get
                {
                    foreach (string domain in Database.Keys)
                    {
                        if (Database[domain].ContainsKey("GameData") && Database[domain]["GameData"].Count > 0) return true;
                    }
                    return false;
                }
            }
            public static int UnusedCount { get { return Unused.Count; } }
            public static int UsedCount { get { return TotalBlockCount() - UnusedCount; } }
            public static int TotalCount { get { return TotalBlockCount(); } }
            public static float UsedPercent { get { return (float)UsedCount / (float)TotalCount * 100f; } }
            public static int showsCount
            {
                get
                {
                    int count = 0;
                    foreach (string domain in Database.Keys)
                    {
                        if (Database[domain].ContainsKey("Main")) count++;
                    }
                    return count;
                }
            }
            public static int gamesCount
            {
                get
                {
                    int count = 0;
                    foreach (string domain in Database.Keys)
                    {
                        if (Database[domain].ContainsKey("GameData")) count++;
                    }
                    return count;
                }
            }
            public static int otherCount
            {
                get
                {
                    int count = 0;
                    foreach (string domain in Database.Keys)
                    {
                        foreach (string sub in Database[domain].Keys)
                        {
                            if (sub != "Main" && sub != "GameData") count++;
                        }
                    }
                    return count;
                }
            }
            public static string Info
            {
                get
                {
                    return "Used: " + UsedCount + "/" + TotalCount + " (" + UsedPercent.ToString("0.0") + "%)\nShows:" + showsCount + "\nGames:" + gamesCount + "\nOther:" + otherCount;
                }
            }
            //-----------------------------------------------------------------------
            // Init
            //-----------------------------------------------------------------------
            public static void Init()
            {
                foreach (IMyTextPanel panel in GridBlocks.textPanels)
                {
                    if (panel.CustomName.StartsWith("DB:"))
                    {
                        if (panel.CustomName.Contains("Unused"))
                        {
                            Unused.Add(panel);
                            continue;
                        }
                        GridDBAddress address = new GridDBAddress(panel.CustomName);
                        if (address.domain == null || address.sub == null) continue;
                        if (!Database.ContainsKey(address.domain)) Database[address.domain] = new Dictionary<string, List<IMyTextPanel>>();
                        if (!Database[address.domain].ContainsKey(address.sub)) Database[address.domain][address.sub] = new List<IMyTextPanel>();
                        Database[address.domain][address.sub].Add(panel);
                    }
                }
            }
            public static void Update()
            {
                Database.Clear();
                Unused.Clear();
                Init();
            }
            //-----------------------------------------------------------------------
            // Get
            //-----------------------------------------------------------------------
            public static string Get(string domain, string sub, int index, bool custom_data)
            {
                if (Database.ContainsKey(domain) && Database[domain].ContainsKey(sub) && Database[domain][sub].Count > index) return custom_data ? Database[domain][sub][index].CustomData : Database[domain][sub][index].GetText();
                //GridInfo.Echo("GridDB.Get: " + domain + "." + sub + "." + index + " not found");
                return "";
            }
            public static string Get(string address)
            {
                GridDBAddress addr = new GridDBAddress(address);
                if (addr.index < 0)
                {
                    return GetRandom(addr.domain, addr.sub);
                }
                return Get(addr.domain, addr.sub, addr.index, addr.custom_data);
            }
            public static string Get(GridDBAddress addr)
            {
                if (addr.index < 0)
                {
                    return GetRandom(addr.domain, addr.sub);
                }
                return Get(addr.domain, addr.sub, addr.index, addr.custom_data);
            }
            static Random random = new Random();
            public static string GetRandom(string domain, string sub)
            {
                if (Database.ContainsKey(domain) && Database[domain].ContainsKey(sub) && Database[domain][sub].Count > 0) return random.Next(100) > 50 ? Database[domain][sub][random.Next(Database[domain][sub].Count)].CustomData : Database[domain][sub][random.Next(Database[domain][sub].Count)].GetText();
                return "";
            }
            //-----------------------------------------------------------------------
            // Set
            //-----------------------------------------------------------------------
            public static void Set(string domain, string sub, int index, bool custom_data, string data, bool addifnew = false)
            {
                if (Database.ContainsKey(domain) && Database[domain].ContainsKey(sub) && Database[domain][sub].Count > index)
                {
                    if (custom_data) Database[domain][sub][index].CustomData = data;
                    else Database[domain][sub][index].WriteText(data);
                }
                else if (addifnew)
                {
                    Add(domain, sub, index, data);
                }
                else throw new Exception("GridDB.Set: " + domain + "." + sub + "." + index + " not found");
            }
            public static void Set(string address, string data, bool addifnew = false)
            {
                GridDBAddress addr = new GridDBAddress(address);
                Set(addr.domain, addr.sub, addr.index, addr.custom_data, data, addifnew);
            }
            public static void Set(GridDBAddress addr, string data, bool addifnew = false)
            {
                Set(addr.domain, addr.sub, addr.index, addr.custom_data, data, addifnew);
            }
            //-----------------------------------------------------------------------
            // Add
            //-----------------------------------------------------------------------
            public static void Add(string domain, string sub, int index, string data)
            {
                if (!Database.ContainsKey(domain)) Database[domain] = new Dictionary<string, List<IMyTextPanel>>();
                if (!Database[domain].ContainsKey(sub)) Database[domain][sub] = new List<IMyTextPanel>();
                IMyTextPanel panel = GetUnused();
                if (panel == null)
                {
                    //GridInfo.Echo("GridDB.Add: No unused panels available");
                    throw new Exception("GridDB.Add: No unused panels available");
                }
                panel.CustomData = data;
                // DB:ShowName.SceneName.00
                panel.CustomName = "DB:" + domain + "." + sub + "." + index.ToString("00");
                Database[domain][sub].Add(panel);
            }
            //-----------------------------------------------------------------------
            // GetUnused
            //-----------------------------------------------------------------------
            public static IMyTextPanel GetUnused()
            {
                if (Unused.Count > 0)
                {
                    IMyTextPanel panel = Unused[0];
                    Unused.RemoveAt(0);
                    return panel;
                }
                return null;
            }
            //-----------------------------------------------------------------------
            // AddDomainBlock
            //-----------------------------------------------------------------------
            public static void AddDomainBlock(string domain, string sub, IMyTextPanel panel)
            {
                if (!Database.ContainsKey(domain)) Database[domain] = new Dictionary<string, List<IMyTextPanel>>();
                if (!Database[domain].ContainsKey(sub)) Database[domain][sub] = new List<IMyTextPanel>();
                Database[domain][sub].Add(panel);
            }
            //-----------------------------------------------------------------------
            // Get the number of blocks a domain uses
            //-----------------------------------------------------------------------
            public static int GetBlockCount(string domain)
            {
                int count = 0;
                if (Database.ContainsKey(domain))
                {
                    foreach (string sub in Database[domain].Keys)
                    {
                        count += Database[domain][sub].Count;
                    }
                }
                return count;
            }
            public static int TotalBlockCount()
            {
                return Unused.Count + UsedBlockCount();
            }
            public static int TotalDomainsCount()
            {
                return Database.Keys.Count;
            }
            public static int UsedBlockCount()
            {
                int count = 0;
                foreach (string domain in Database.Keys)
                {
                    count += GetBlockCount(domain);
                }
                return count;
            }
            public static List<string> GetGames()
            {
                List<string> games = new List<string>();
                foreach (string domain in Database.Keys)
                {
                    // check that the domain has the sub "GameData"
                    if (Database[domain].ContainsKey("GameData")) games.Add(domain);
                }
                return games;
            }
            //-----------------------------------------------------------------------
            // GetBlockUsage
            //-----------------------------------------------------------------------
            public static Dictionary<string, int> GetBlockUsage()
            {
                Dictionary<string, int> usage = new Dictionary<string, int>();
                foreach (string domain in Database.Keys)
                {
                    usage[domain] = GetBlockCount(domain);
                }
                return usage;
            }

        }
        //-----------------------------------------------------------------------
    }
}
