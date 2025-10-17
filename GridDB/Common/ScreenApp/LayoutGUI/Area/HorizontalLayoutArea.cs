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
        // HorizontalLayoutArea - a container that arranges items horizontally
        //-----------------------------------------------------------------------
        public class HorizontalLayoutArea : LayoutArea
        {
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public HorizontalLayoutArea(Screen screen, Vector2 position, Vector2 size, ScreenSprite item = null) : base(screen, position, size, item)
            {
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            public override void ApplyLayout()
            {
                //GridInfo.Echo("LayoutArea ApplyLayout: applying layout to " + Items.Count + " items");
                if (Items.Count == 0) return;
                float remainingWidth = MarginSize.X;
                int flexibleCount = 0;
                //GridInfo.Echo("LayoutArea ApplyLayout: starting remaining width " + remainingWidth);
                // figure out how much width is left over after items with fixed widths are counted
                foreach (var item in Items)
                {
                    //GridInfo.Echo("LayoutArea ApplyLayout: item min width " + item.MinWidth + " flexible " + item.FlexibleWidth);
                    if (!item.FlexibleWidth) remainingWidth -= item.MinWidth;
                    else flexibleCount++;
                }
                //GridInfo.Echo("LayoutArea ApplyLayout: remaining width " + remainingWidth + " for " + flexibleCount + " flexible items");
                Vector2 currentPosition = MarginPosition;
                Vector2 itemSize = new Vector2(MarginSize.X, MarginSize.Y);
                if (flexibleCount > 0) itemSize.X = remainingWidth / flexibleCount;
                foreach (var item in Items)
                {
                    item.Position = currentPosition;
                    item.Size = itemSize;
                    currentPosition.X += item.Size.X;
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
