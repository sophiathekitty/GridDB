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
            public static bool hasShows { get { return Database.Count > 0; } }
            //-----------------------------------------------------------------------
            // Init
            //-----------------------------------------------------------------------
            public static void Init()
            {
                Database.Clear();
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
                        if (!Database.ContainsKey(address.show)) Database[address.show] = new Dictionary<string, List<IMyTextPanel>>();
                        if (!Database[address.show].ContainsKey(address.scene)) Database[address.show][address.scene] = new List<IMyTextPanel>();
                        Database[address.show][address.scene].Add(panel);
                    }
                }
            }
            //-----------------------------------------------------------------------
            // Get
            //-----------------------------------------------------------------------
            public static string Get(string show, string scene, int index, bool custom_data)
            {
                if (Database.ContainsKey(show) && Database[show].ContainsKey(scene) && Database[show][scene].Count > index) return custom_data ? Database[show][scene][index].CustomData : Database[show][scene][index].GetText();
                //GridInfo.Echo("GridDB.Get: " + show + "." + scene + "." + index + " not found");
                return "";
            }
            public static string Get(string address)
            {
                GridDBAddress addr = new GridDBAddress(address);
                if (addr.index < 0)
                {
                    return GetRandom(addr.show, addr.scene);
                }
                return Get(addr.show, addr.scene, addr.index, addr.custom_data);
            }
            static Random random = new Random();
            public static string GetRandom(string show, string scene)
            {
                if (Database.ContainsKey(show) && Database[show].ContainsKey(scene) && Database[show][scene].Count > 0) return random.Next(100) > 50 ? Database[show][scene][random.Next(Database[show][scene].Count)].CustomData : Database[show][scene][random.Next(Database[show][scene].Count)].GetText();
                return "";
            }
            //-----------------------------------------------------------------------
            // Set
            //-----------------------------------------------------------------------
            public static void Set(string show, string scene, int index, bool custom_data, string data, bool addifnew = false)
            {
                if (Database.ContainsKey(show) && Database[show].ContainsKey(scene) && Database[show][scene].Count > index)
                {
                    if (custom_data) Database[show][scene][index].CustomData = data;
                    else Database[show][scene][index].WriteText(data);
                }
                else if (addifnew)
                {
                    Add(show, scene, index, data);
                }
                else throw new Exception("GridDB.Set: " + show + "." + scene + "." + index + " not found");
            }
            public static void Set(string address, string data, bool addifnew = false)
            {
                GridDBAddress addr = new GridDBAddress(address);
                Set(addr.show, addr.scene, addr.index, addr.custom_data, data, addifnew);
            }
            public static void Add(string show, string scene, int index, string data)
            {
                if (!Database.ContainsKey(show)) Database[show] = new Dictionary<string, List<IMyTextPanel>>();
                if (!Database[show].ContainsKey(scene)) Database[show][scene] = new List<IMyTextPanel>();
                IMyTextPanel panel = GetUnused();
                if (panel == null)
                {
                    //GridInfo.Echo("GridDB.Add: No unused panels available");
                    throw new Exception("GridDB.Add: No unused panels available");
                }
                panel.CustomData = data;
                // DB:ShowName.SceneName.00
                panel.CustomName = "DB:" + show + "." + scene + "." + index.ToString("00");
                Database[show][scene].Add(panel);
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
            // AddShowBlock
            //-----------------------------------------------------------------------
            public static void AddShowBlock(string show, string scene, IMyTextPanel panel)
            {
                if (!Database.ContainsKey(show)) Database[show] = new Dictionary<string, List<IMyTextPanel>>();
                if (!Database[show].ContainsKey(scene)) Database[show][scene] = new List<IMyTextPanel>();
                Database[show][scene].Add(panel);
            }
            //-----------------------------------------------------------------------
            // Get the number of blocks a show uses
            //-----------------------------------------------------------------------
            public static int GetBlockCount(string show)
            {
                int count = 0;
                if (Database.ContainsKey(show))
                {
                    foreach (string scene in Database[show].Keys)
                    {
                        count += Database[show][scene].Count;
                    }
                }
                return count;
            }
            public static int TotalBlockCount()
            {
                return Unused.Count + UsedBlockCount();
            }
            public static int TotalShowsCount()
            {
                return Database.Keys.Count;
            }
            public static int UsedBlockCount()
            {
                int count = 0;
                foreach (string show in Database.Keys)
                {
                    count += GetBlockCount(show);
                }
                return count;
            }
            public static List<string> GetGames()
            {
                List<string> games = new List<string>();
                foreach (string show in Database.Keys)
                {
                    // check that the show has the scene "GameData"
                    if (Database[show].ContainsKey("GameData")) games.Add(show);
                }
                return games;
            }
            //-----------------------------------------------------------------------
            // GetBlockUsage
            //-----------------------------------------------------------------------
            public static Dictionary<string, int> GetBlockUsage()
            {
                Dictionary<string, int> usage = new Dictionary<string, int>();
                foreach (string show in Database.Keys)
                {
                    usage[show] = GetBlockCount(show);
                }
                return usage;
            }
        }
        //-----------------------------------------------------------------------
        // GridDBAddress
        //-----------------------------------------------------------------------
        public class GridDBAddress
        {
            public string show;
            public string scene;
            public int index;
            public bool custom_data;
            public int x;
            public int y;
            public int length;
            // constructor
            public GridDBAddress(string address)
            {
                if (address.StartsWith("DB:")) address = address.Substring(3);
                string[] parts = address.Split('.');
                length = parts.Length;
                if (parts.Length == 6)
                {
                    int index = 0;
                    int x = 0;
                    int y = 0;
                    if (int.TryParse(parts[2], out index) && int.TryParse(parts[4], out x) && int.TryParse(parts[5], out y))
                    {
                        this.show = parts[0];
                        this.scene = parts[1];
                        this.index = index;
                        this.custom_data = parts[3].ToLower() == "customdata";
                        this.x = x;
                        this.y = y;
                    }
                }
                else if (parts.Length == 4)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        this.show = parts[0];
                        this.scene = parts[1];
                        this.index = index;
                        this.custom_data = parts[3].ToLower() == "customdata";
                    }
                }
                else if (parts.Length == 3)
                {
                    int index = 0;
                    if (int.TryParse(parts[2], out index))
                    {
                        this.show = parts[0];
                        this.scene = parts[1];
                        this.index = index;
                        this.custom_data = false;
                    }
                }
                else if (parts.Length == 2)
                {
                    this.show = parts[0];
                    this.scene = parts[1];
                    this.index = -1;
                    this.custom_data = false;
                }
            }
            // to string
            public override string ToString()
            {
                if (index >= 0) return show + "." + scene + "." + index + "." + (custom_data ? "customdata" : "text");
                return show + "." + scene;
            }
            public string ToBlockName()
            {
                if (index < 10) return "DB:" + show + "." + scene + ".0" + index;
                if (index >= 0) return "DB:" + show + "." + scene + "." + index;
                return "TV." + show + "." + scene + ".0";
            }
        }

        //-----------------------------------------------------------------------
    }
}
