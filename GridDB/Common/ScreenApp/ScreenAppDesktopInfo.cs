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
        //---------------------------------------------------------------------------
        // ScreenAppDesktopInfo -- data structure for desktop app info
        //---------------------------------------------------------------------------
        public class ScreenAppDesktopInfo : MessageData
        {
            public static List<ScreenAppDesktopInfo> AvailableApps = new List<ScreenAppDesktopInfo>();
            public static void Init()
            {
                GridInfo.Echo("Initializing ScreenAppDesktopInfo...");
                //GridInfo.AddBroadcastListener("ReportDesktopApps");
                GridInfo.AddMessageHandler("ReportDesktopApps", ReportDesktopAppsHandler);
                GridInfo.AddMessageHandler("DesktopAppInfo", RecievedAppInfo);
                GridInfo.AddScriptMessageHandler(HandleMessage);
                // report that we have desktop apps available so that the desktop app can request our app info if it wants
                GridInfo.IGC.SendBroadcastMessage("DesktopAppProviderBooted", GridInfo.Me.CubeGrid.ToString());
            }
            public static void HandleMessage(string msg) => HandleMessage(MessageData.ParseMessage(msg));
            public static void HandleMessage(MessageData msg)
            {
                if (msg.Tag == "ReportDesktopApps")
                {
                    ReportDesktopAppsHandler(msg);
                }
            }
            static void ReportDesktopAppsHandler(MyIGCMessage msg) => ReportDesktopAppsHandler(MessageData.ParseMessage(msg));
            static void ReportDesktopAppsHandler(MessageData msg)
            {
                //GridInfo.Echo($"Received request for desktop apps from {msg.Sender}");
                //if (msg.Source == GridInfo.Me.EntityId || msg.As<string>() != GridInfo.Me.CubeGrid.ToString()) return; // only respond to requests for our grid that also aren't from ourselves
                foreach (var app in AvailableApps)
                {
                    if (app.AppScript != GridInfo.ProgramName) continue; // only report local apps
                    MessageData data = new MessageData(app.ToString());
                    data["Grid"] = GridInfo.Me.CubeGrid.ToString();
                    //GridInfo.IGC.SendUnicastMessage(msg.Sender, "DesktopAppInfo", app.ToString());
                }
            }
            static void RecievedAppInfo(MyIGCMessage msg)
            {
                //GridInfo.Echo($"Received app info?");
                ScreenAppDesktopInfo appInfo = new ScreenAppDesktopInfo(msg.As<string>());
                //if (appInfo.Grid != GridInfo.Me.CubeGrid.ToString()) return; // only accept apps from our grid
                if (AvailableApps.Find(a => a.AppName == appInfo.AppName) == null) AvailableApps.Add(appInfo);
                //GridInfo.Echo($"Available desktop app count: {AvailableApps.Count}");
            }
            public static void FindAvailableApps()
            {
                //GridInfo.Echo("Requesting available desktop apps...");
                //GridInfo.IGC.SendBroadcastMessage("ReportDesktopApps", GridInfo.Me.CubeGrid.ToString());
                List<IMyProgrammableBlock> programs = new List<IMyProgrammableBlock>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(programs, block => block.IsSameConstructAs(GridInfo.Me));
                MessageData msg = new MessageData("ReportDesktopApps", GridInfo.IGC.Me);
                string msg_str = msg.ToString();
                foreach (var program in programs)
                {
                    if (program != GridInfo.Me)
                    {
                        program.TryRun(msg_str);
                    }
                }
            }
            public static void FindAvailableApps(long igcAddress)
            {
                //GridInfo.Echo($"Requesting available desktop apps from {igcAddress}...");
                GridInfo.IGC.SendUnicastMessage(igcAddress, "ReportDesktopApps", GridInfo.Me.CubeGrid.ToString());
            }
            public static void RegisterLocalApp(string appName, string appIcon)
            {
                //GridInfo.Echo($"Registering local desktop app '{appName}' with icon '{appIcon.Substring(0, 5)}'...");
                AvailableApps.Add(new ScreenAppDesktopInfo(appName, appIcon, GridInfo.ProgramName, GridInfo.Me.CubeGrid.ToString()));
            }
            //-----------------------------------------------------------
            // fields
            //-----------------------------------------------------------
            public string AppName   // the name of the app
            {
                get { return data["AppName"]; }
                set { data["AppName"] = value; }
            }
            public string AppIcon   // the icon string (could be a path or a special code)
            {
                get { return data["AppIcon"]; }
                set { data["AppIcon"] = value; }
            }
            public string AppScript // the script code to run the app
            {
                get { return data["AppScript"]; }
                set { data["AppScript"] = value; }
            }
            public string Grid
            {
                get { return data["Grid"]; }
                set { data["Grid"] = value; }
            }
            //char dataSeparator = MessageData.DataSeparator;
            //-----------------------------------------------------------
            // constructors
            //-----------------------------------------------------------
            public ScreenAppDesktopInfo(string appName, string appIcon, string appScript, string grid)
            {
                AppName = appName;
                AppIcon = appIcon;
                AppScript = appScript;
                Grid = grid;
            }
            public ScreenAppDesktopInfo(string data) : base(data)
            {
                /*
                string[] parts = data.Split(dataSeparator);
                if (parts.Length != 3) throw new Exception("Invalid ScreenAppDesktopInfo data string");
                AppName = parts[0];
                AppIcon = parts[1];
                AppScript = parts[2];
                */
            }
            //-----------------------------------------------------------
            // methods
            //-----------------------------------------------------------
            public void Launch(ScreenAppSeat seat)
            {
                if (ScreenApp.AvailableApps.ContainsKey(AppName)) ScreenApp.AvailableApps[AppName](seat);
                else seat.CurrentAppId = new ScreenAppId(AppName, AppScript);
            }
            /*
            override public string ToString()
            {
                return $"{AppName}{dataSeparator}{AppIcon}{dataSeparator}{AppScript}";
            }
            */
        }
        //---------------------------------------------------------------------------
    }
}
