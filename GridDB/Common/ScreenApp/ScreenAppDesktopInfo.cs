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
                GridInfo.AddBroadcastListener("ReportDesktopApps");
                GridInfo.AddMessageHandler("ReportDesktopApps", ReportDesktopAppsHandler);
                GridInfo.AddMessageHandler("DesktopAppInfo", RecievedAppInfo);
            }
            static void ReportDesktopAppsHandler(MyIGCMessage msg)
            {
                foreach (var app in AvailableApps)
                {
                    if(app.AppScript != GridInfo.ProgramName) continue; // only report local apps
                    GridInfo.IGC.SendUnicastMessage(msg.Source, "DesktopAppInfo", app.ToString());
                }
            }
            static void RecievedAppInfo(MyIGCMessage msg)
            {
                ScreenAppDesktopInfo appInfo = new ScreenAppDesktopInfo(msg.As<string>());
                if (AvailableApps.Find(a => a.AppName == appInfo.AppName) == null) AvailableApps.Add(appInfo);
            }
            public static void FindAvailableApps()
            {
                GridInfo.IGC.SendBroadcastMessage("ReportDesktopApps", "");
            }
            public static void RegisterLocalApp(string appName, string appIcon)
            {
                AvailableApps.Add(new ScreenAppDesktopInfo(appName, appIcon, GridInfo.ProgramName));
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
            char dataSeparator = MessageData.DataSeparator;
            //-----------------------------------------------------------
            // constructors
            //-----------------------------------------------------------
            public ScreenAppDesktopInfo(string appName, string appIcon, string appScript)
            {
                AppName = appName;
                AppIcon = appIcon;
                AppScript = appScript;
            }
            public ScreenAppDesktopInfo(string data)
            {
                string[] parts = data.Split(dataSeparator);
                if (parts.Length != 3) throw new Exception("Invalid ScreenAppDesktopInfo data string");
                AppName = parts[0];
                AppIcon = parts[1];
                AppScript = parts[2];
            }
            //-----------------------------------------------------------
            // methods
            //-----------------------------------------------------------
            public void Launch(ScreenAppSeat seat)
            {
                if (ScreenApp.AvailableApps.ContainsKey(AppName)) ScreenApp.AvailableApps[AppName](seat);
                else seat.CurrentAppId = new ScreenAppId(AppName, AppScript);
            }
            override public string ToString()
            {
                return $"{AppName}{dataSeparator}{AppIcon}{dataSeparator}{AppScript}";
            }
        }
        //---------------------------------------------------------------------------
    }
}
