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
        public class GridDBInfo : LayoutArea
        {
            public GridDBInfo(ScreenApp screen, Vector2 position, Vector2 size, ref Action<string> updateInfo, Color bgColor, float borderWidth = 0) : base(screen, position, size, new TextureSprite(position,size, "SquareSimple",bgColor), borderWidth)
            {
                FlexibleWidth = false; FlexibleHeight = true;
                // add header
                AddItem(new TextSprite(position, new Vector2(size.X, 30), "Info", Scale: 1.25f), true, false);
                // add HorizontalArea
                AddItem(new TextSprite(position, new Vector2(size.X, size.Y - 40), GridDB.Info), ref updateInfo, false, true);
            }
        }
    }
}
