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
    partial class Program : MyGridProgram
    {
        //==========================
        //--------------------------
        // constructor
        //--------------------------
        public Program()
        {
            // systems
            GridInfo.Init("GridDB",this,UpdateFrequency.Update10);
            GridBlocks.Init();
            // grid db
            GridDB.Init();
            GridDBServer.Init();
            GridDBClient.Init();
            // screens
            ProgScreen.Init();
            ScreenAppSeat.Init();
            // apps
            DataManager.Init();
            WebBrowser.Init();
        }
        //--------------------------
        // save data
        //--------------------------
        public void Save()
        {
            Storage = GridInfo.Save();
        }
        //--------------------------
        // main loop
        //--------------------------
        public void Main(string argument, UpdateType updateSource)
        {
            GridInfo.Main(argument, updateSource);
        }
        //==========================
    }
}
