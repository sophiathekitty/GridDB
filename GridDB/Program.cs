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
        // constructor
        //--------------------------
        public Program()
        {
            GridInfo.Init("GridDB",this);
            GridBlocks.Init();
            GridDB.Init();
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
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
            // main loop...
        }
        //==========================
    }
}
