using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
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
        // ScreenApp - application screen base class
        //----------------------------------------------------------------------
        public class ScreenApp : Screen
        {
            //----------------------------------------------------------------------
            // static fields
            //----------------------------------------------------------------------
            // dictionary of AppName,Action to create app
            public static Dictionary<string, Action<ScreenAppSeat>> AvailableApps = new Dictionary<string, Action<ScreenAppSeat>>();
            public static long ScriptId(string ScriptName)
            {
                return GridBlocks.ScriptId(ScriptName);
            }
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            public GameInput input { get; private set; }                                                // input available to the app
            public List<IMySoundBlock> soundBlocks { get; private set; }                                // sound blocks available to the app
            string address { get { return seat.Address; } }                                             // the address of the seat this app is running on
            public ScreenAppSeat seat = null;                                                           // the seat this app is running on
            ScreenAppId appId = new ScreenAppId("None");                                                // unique id for the app as ScreenAppId
            public string AppId { get { return appId.Id; } set { appId = new ScreenAppId(value); } }    // unique id for the app as a string
            public string Address { get { return address; } }                                           // the address of the seat this app is running on
            public string Name { get { return appId.Name; } }                                           // the name of the app
            string domain = "";                                                                       // for GridDB domains (like for loading sets of data) like current project or game or show (DB:Domain.Set.01)
            public string Domain
            {
                get { return domain; }
                set
                {
                    GridInfo.Echo($"ScreenApp Domain set to {value}");  
                    if (domain == value) return;
                    domain = value;
                    FileNameChanged?.Invoke(fileName);
                }
            }
            // for GridDB domains (like for loading sets of data) like current project or game or show (DB:Domain.Set.01)
            int fileIndex = -1;
            public int FileIndex
            {
                get { return fileIndex; }
                set 
                {
                    GridInfo.Echo($"ScreenApp FileIndex set to {value}");
                    if (fileIndex == value) return;
                    fileIndex = value;
                    FileNameChanged?.Invoke(fileName);
                }
            }
            string set = "";                                                                          // for GridDB sets (like for loading sets of data) like current level or scene or act (DB:Domain.Set.FileIndex)
            public string Set
            {
                get { return set; }
                set
                {
                    GridInfo.Echo($"ScreenApp Set set to {value}");
                    if (set == value) return;
                    set = value;
                }
            }
            public string fileName 
            { 
                get
                {
                    if(FileIndex >= 0) return $"{Domain} {Set}#{FileIndex}";
                    if(Set != "") return $"{Domain} {Set}";
                    return Domain;
                } 
            }
                // for GridDB file index (like for loading sets of data) like a specific spritesheet or data set (DB:Domain.Set.FileIndex)
            string keyPrompt = "";                                                                     // prompt for input key (if any)
            public string KeyPrompt
            {
                get { return keyPrompt; }
                set
                {
                    if (keyPrompt == value) return;
                    keyPrompt = value;
                    KeyPromptChanged?.Invoke(keyPrompt);
                }
            }
            string status = "OK";                                                                       // status message for the app
            public string Status                                                                        // status message for the app
            {
                get { return status; }
                set
                {
                    if (status == value) return;
                    status = value;
                    StatusChanged?.Invoke(status);
                }
            }
            public Action<string> FileNameChanged;                                                      // action for updating display variable when file name changes
            public Action<string> KeyPromptChanged;                                                     // action for updating display variable when key prompt changes
            public Action<string> StatusChanged;                                                        // action for updating display variable when status changes
            public LayoutStyle AppStyle;                                                                // style for the app
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public ScreenApp(ScreenAppSeat seat, string AppName) : base(GridBlocks.GetSurface(seat.Address))
            {
                GridInfo.Echo($"ScreenApp init {AppName} for {seat.Address}");
                AppId = AppName;
                this.seat = seat;
                input = seat.input;
                soundBlocks = seat.soundBlocks;
                seat[AppId] = this;
                AppStyle = new LayoutStyle(this);
            }
        }
        //----------------------------------------------------------------------
    }
}
