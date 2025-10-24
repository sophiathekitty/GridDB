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
using static IngameScript.Program;

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
            }
            //------------------------------------------------------
            // static Main loop (try to run one seat per call)
            //------------------------------------------------------
            static void Main(string argument)
            {
                Next?.Main(argument);
            }
            //------------------------------------------------------
            // get or create a seat
            //------------------------------------------------------
            public static ScreenAppSeat GetSeat(string address, string rootApp = "", string currentApp = "")
            {
                if (!SeatsByAddress.ContainsKey(address))
                {
                    SeatsByAddress[address] = new ScreenAppSeat(address, rootApp);
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
                    GridInfo.Echo("FocusApp! " + msg.Address + " " + msg["appId"] + " " + msg["rootApp"]);
                    ScreenAppId appId = new ScreenAppId(msg["appId"]);
                    if (!appId.Local) return; // local apps are handled by the seat
                    GridInfo.Echo("FocusApp Local " + appId.Name);
                    ScreenAppSeat seat = GetSeat(msg["Address"], msg["rootApp"]);
                    seat.CurrentApp = appId.Id;
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
                        GridInfo.SendScriptMessage("GameEditor", msg.ToString());
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
                Address = address;
                RootApp = rootApp;
                input = new GameInput(GridBlocks.GetController(address));
                soundBlocks = GridBlocks.GetSoundBlocks(address);
                if (currentApp != "" && ScreenApp.AvailableApps.ContainsKey(currentApp))
                {
                    ScreenApp.AvailableApps[currentApp]?.Invoke(this);
                    CurrentApp = rootApp;
                }
                else if (RootApp != "" && ScreenApp.AvailableApps.ContainsKey(RootApp))
                {
                    ScreenApp.AvailableApps[RootApp]?.Invoke(this);
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
