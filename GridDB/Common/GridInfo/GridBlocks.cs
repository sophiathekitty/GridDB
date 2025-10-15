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
        // This class is used to store references to blocks on the grid.
        //-----------------------------------------------------------------------
        public class GridBlocks
        {
            //-----------------------------------------------------------------------
            // static fields
            // cached block lists
            //-----------------------------------------------------------------------
            public static List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
            public static void Init()
            {
                textPanels.Clear();
                // get all the text panels on the same subgrid as the programmable block
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, x => x.CubeGrid == GridInfo.Me.CubeGrid);
                textPanels.Sort((a, b) => a.CustomName.CompareTo(b.CustomName));
                int index = 1;
                foreach (IMyTextPanel panel in textPanels)
                {
                    if (panel.CustomName.Contains("DB")) continue;
                    panel.CustomName = "DB:Unused "+ index++;
                }
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
                List<IMyTextPanel> textPanels = new List<IMyTextPanel>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textPanels, x => x.IsSameConstructAs(GridInfo.Me));
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
                // check seat
                IMyShipController controller = GetController(address);
                if (controller is IMyTextSurfaceProvider)
                {
                    IMyTextSurfaceProvider provider = controller as IMyTextSurfaceProvider;
                    if (provider.SurfaceCount > 0) return provider.GetSurface(0);
                }
                return null;
            }
            //----------------------------------------------------------------------
            // static method to get another programmable block where the name contains the given string
            //----------------------------------------------------------------------
            public static IMyProgrammableBlock GetPB(string keyword)
            {
                List<IMyProgrammableBlock> pbs = new List<IMyProgrammableBlock>();
                GridInfo.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(pbs, x => x.IsSameConstructAs(GridInfo.Me) && x.EntityId != GridInfo.Me.EntityId && x.CustomName.Contains(keyword));
                if (pbs.Count > 0) return pbs[0];
                return null;
            }
            public static long ScriptId(string scriptName)
            {
                IMyProgrammableBlock pb = GetPB(scriptName);
                if (pb != null) return pb.EntityId;
                return 0;
            }
        }
        //-----------------------------------------------------------------------
    }
}
