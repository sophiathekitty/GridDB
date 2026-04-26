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
        //----------------------------------------------------------------------
        // Seat for ScreenApp User
        //----------------------------------------------------------------------
        public class ScreenAppSeat
        {
            //------------------------------------------------------
            // static
            //------------------------------------------------------
            public static Dictionary<string, ScreenAppSeat> SeatsByAddress = new Dictionary<string, ScreenAppSeat>();
            public static void Init()
            {
                //GridInfo.Echo("ScreenAppSeat.Init");
                SeatsByAddress.Clear();
                GridInfo.AddMainLoop(Main);
                GridInfo.AddScriptMessageHandler(HandleMessage);
                GridInfo.AddMessageHandler("FocusApp", HandleMessage);
                GridInfo.AddMessageHandler("ProviderReset", HandleMessage);
                ScreenAppId screenAppId = new ScreenAppId(GridInfo.ProgramName);
                MessageData msg = new MessageData("ProviderReset", GridInfo.IGC.Me);
                msg["appId"] = screenAppId.ToString();
                string msg_str = msg.ToString();
                List<IMyProgrammableBlock> programs = new List<IMyProgrammableBlock>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(programs, block => block.IsSameConstructAs(GridInfo.Me));
                foreach (var program in programs)
                {
                    if (program.CustomName.Contains(" @")) program.TryRun(msg_str);
                }
                //GridInfo.IGC.SendBroadcastMessage("ProviderReset", msg.ToString());
                //GridInfo.Echo("ScreenAppSeat initialized.");
            }
            //------------------------------------------------------
            // static Main loop (try to run one seat per call)
            //------------------------------------------------------
            public static int SeatsPerMainCall = 1;
            static void Main(string argument)
            {
                for (int i = 0; i < SeatsPerMainCall; i++) Next?.Main(argument);
            }
            //-----------------------------------------------------------------------
            // static methods to get blocks by address for ScreenAppSeat
            //-----------------------------------------------------------------------
            public static IMyShipController GetController(string address)
            {
                List<IMyShipController> controllers = new List<IMyShipController>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyShipController>(controllers, x => x.IsSameConstructAs(GridInfo.Me) && x.CustomName.Contains(address));
                if (controllers.Count > 0) return controllers[0];
                return null;
            }
            public static List<IMySoundBlock> GetSoundBlocks(string address)
            {
                List<IMySoundBlock> soundBlocks = new List<IMySoundBlock>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(soundBlocks, x => x.IsSameConstructAs(GridInfo.Me) && x.CustomName.Contains(address));
                return soundBlocks;
            }
            public static IMyTextSurface GetSurface(string address)
            {
                // check seat
                IMyShipController controller = GetController(address);
                if (controller is IMyTextSurfaceProvider)
                {
                    IMyTextSurfaceProvider provider = controller as IMyTextSurfaceProvider;
                    if (provider.SurfaceCount > 0) return provider.GetSurface(0);
                }
                // check text panels
                List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, x => x.IsSameConstructAs(GridInfo.Me) && x.CustomName.Contains(address));
                foreach (IMyTextPanel panel in textPanels)
                {
                    if (panel.CustomName.Contains(address)) return panel;
                }
                // check sound blocks
                List<IMySoundBlock> soundBlocks = GetSoundBlocks(address);
                foreach (IMySoundBlock block in soundBlocks)
                {
                    if (block is IMyTextSurfaceProvider)
                    {
                        IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;
                        if (provider.SurfaceCount > 0) return provider.GetSurface(0);
                    }
                }
                return null;
            }
            //------------------------------------------------------
            // get or create a seat
            //------------------------------------------------------
            public static ScreenAppSeat GetSeat(string address, string rootApp = "", string currentApp = "")
            {
                if (!SeatsByAddress.ContainsKey(address))
                {
                    SeatsByAddress[address] = new ScreenAppSeat(address, rootApp, currentApp);
                }
                return SeatsByAddress[address];
            }
            //------------------------------------------------------
            // handle argument message
            //------------------------------------------------------
            public static void HandleMessage(MyIGCMessage msg) => HandleMessage(MessageData.ParseMessage(msg));
            public static void HandleMessage(string msg) => HandleMessage(MessageData.ParseMessage(msg));
            public static void HandleMessage(MessageData msg)
            {
                if (msg.Tag == "FocusApp")
                {
                    //GridInfo.Echo("FocusApp! " + msg.Address + " " + msg["appId"] + " " + msg["rootApp"]);
                    ScreenAppId appId = new ScreenAppId(msg["appId"]);
                    if (!appId.Local) return; // local apps are handled by the seat
                    //GridInfo.Echo("FocusApp Local " + appId.Name);
                    ScreenAppSeat seat = GetSeat(msg["Address"], msg["rootApp"]);
                    seat.CurrentApp = appId.Id;
                }
                if (msg.Tag == "ProviderReset")
                {
                    //GridInfo.Echo("ProviderReset! " + msg["appId"]);
                    ScreenAppId appId = new ScreenAppId(msg["appId"]);
                    foreach (ScreenAppSeat seat in SeatsByAddress.Values)
                    {
                        ScreenAppId rootAppId = new ScreenAppId(seat.RootApp);
                        if (rootAppId.Host == appId.Host)
                        {
                            seat.AppFocus.Clear();
                            continue;
                        }
                        // search the AppFocus stack for any apps hosted by the provider and reset to root if found
                        foreach (string app in seat.AppFocus)
                        {
                            ScreenAppId aId = new ScreenAppId(app);
                            if (aId.Host == appId.Host)
                            {
                                seat.AppFocus.Clear();
                                // if this is the root app is local then focus it
                                if (rootAppId.Local) seat.CurrentApp = seat.RootApp;
                                break;
                            }
                        }
                    }
                }
            }
            // get the next app to run in the main loop
            static Queue<ScreenAppSeat> screenAppSeats = new Queue<ScreenAppSeat>();
            public static ScreenApp Next
            {
                get
                {
                    if (screenAppSeats.Count == 0)
                    {
                        screenAppSeats = new Queue<ScreenAppSeat>(SeatsByAddress.Values);
                    }
                    while (screenAppSeats.Count > 0)
                    {
                        ScreenAppSeat seat = screenAppSeats.Dequeue();
                        if (seat.App != null)
                        {
                            return seat.App;
                        }
                    }
                    //GridInfo.Echo("No ScreenAppSeats with Apps");
                    return null;
                }
            }
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            public string Address { get; private set; }                                     // seat address (a custom string. desktop:artist_desk, tv:artist_desk, sfx:artist_desk use "artist_desk" as address)
            public string RootApp { get; private set; }                                     // root app for the seat
            public GameInput input { get; private set; }                                    // input handler for the screen grid
            public List<IMySoundBlock> soundBlocks { get; private set; }                    // sound blocks on the screen grid
            Stack<string> AppFocus = new Stack<string>();                                   // stack of focused apps
            Dictionary<string, ScreenApp> LocalApps = new Dictionary<string, ScreenApp>();  // local apps only
            public string CurrentApp                                                        // the currently focused app (id string)
            {
                get
                {
                    if (AppFocus.Count == 0) return RootApp;
                    return AppFocus.Peek();
                }
                set
                {
                    ScreenAppId appId = new ScreenAppId(value);
                    if (appId.Id == CurrentApp || appId.Name == "None") return;
                    if (appId.Local && !LocalApps.ContainsKey(appId.Id) && ScreenApp.AvailableApps.ContainsKey(appId.Name))
                    {
                        ScreenApp.AvailableApps[appId.Name]?.Invoke(this);
                    }
                    else AppFocus.Push(appId.Id);
                }
            }
            public ScreenAppId CurrentAppId                                                 // the currently focused app (ScreenAppId)
            {
                get
                {
                    return new ScreenAppId(CurrentApp);
                }
                set
                {
                    CurrentApp = value.Id;
                    if (value.Local == false)
                    {
                        MessageData msg = new MessageData("FocusApp", GridInfo.IGC.Me);
                        msg["appId"] = CurrentApp;
                        msg["rootApp"] = PreviousApp;
                        msg["Address"] = Address;
                        GridInfo.SendScriptMessage($"@{value.Host}", msg.ToString());
                    }
                }
            }
            public string PreviousApp                                                       // the previously focused app (id string)
            {
                get
                {
                    if (AppFocus.Count < 2) return RootApp;
                    string current = AppFocus.Pop();
                    string previous = AppFocus.Peek();
                    AppFocus.Push(current);
                    return previous;
                }
            }
            public ScreenApp App                                                            // the currently focused app (ScreenApp)
            {
                get
                {
                    return this[CurrentApp];
                }
            }
            public bool LocalSeat                                                           // is the seat local (root app is local)
            {
                get
                {
                    ScreenAppId appId = new ScreenAppId(RootApp);
                    return appId.Local;
                }
            }
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public ScreenAppSeat(string address, string rootApp = "", string currentApp = "")
            {
                //GridInfo.Echo("Creating ScreenAppSeat: " + address);
                Address = address;
                RootApp = rootApp;
                input = new GameInput(GetController(address));
                soundBlocks = GetSoundBlocks(address);
                if (currentApp != "" && ScreenApp.AvailableApps.ContainsKey(currentApp))
                {
                    //GridInfo.Echo("Seat CurrentApp: " + currentApp);
                    ScreenApp.AvailableApps[currentApp]?.Invoke(this);
                    CurrentApp = rootApp;
                }
                else if (RootApp != "" && ScreenApp.AvailableApps.ContainsKey(RootApp))
                {
                    //GridInfo.Echo("Seat RootApp: " + RootApp);
                    ScreenApp.AvailableApps[RootApp]?.Invoke(this);
                }
                else
                {
                    GridInfo.Echo("Seat has no valid RootApp or CurrentApp.");
                }
            }
            //------------------------------------------------------
            // [] for App
            //------------------------------------------------------
            public ScreenApp this[string key]
            {
                get
                {
                    if (LocalApps.ContainsKey(key))
                    {
                        return LocalApps[key];
                    }
                    return null;
                }
                set
                {
                    LocalApps[key] = value;
                    CurrentApp = value.AppId;
                }
            }

            public void CloseApp()
            {
                if (AppFocus.Count > 0)
                {
                    string app = AppFocus.Pop();
                    if (LocalApps.ContainsKey(app))
                    {
                        LocalApps.Remove(app);
                    }
                    ScreenAppId appId = new ScreenAppId(CurrentApp);
                    MessageData msg = new MessageData("FocusApp", GridInfo.IGC.Me);
                    msg["appId"] = CurrentApp;
                    msg["rootApp"] = PreviousApp;
                    msg["Address"] = Address;
                    GridInfo.SendScriptMessage(appId.Host.ToString(), msg.ToString());

                }
            }
        }
        //------------------------------------------------------
    }
}
