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
            public GameInput input { get; private set; }
            public List<IMySoundBlock> soundBlocks { get; private set; }
            string address { get { return seat.Address; } }
            public ScreenAppSeat seat = null;
            ScreenAppId appId = new ScreenAppId("None");
            public string AppId { get { return appId.Id; } set { appId = new ScreenAppId(value); } }
            public string Address { get { return address; } }

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
            }
        }
        //----------------------------------------------------------------------
    }
}
