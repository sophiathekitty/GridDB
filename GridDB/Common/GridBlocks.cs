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
        }
        //-----------------------------------------------------------------------
    }
}
