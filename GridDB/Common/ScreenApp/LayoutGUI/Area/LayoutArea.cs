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
        // LayoutArea - a container for layout items
        //-----------------------------------------------------------------------
        public class LayoutArea : LayoutItem
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            Screen screen;                                          // screen to draw to
            public List<LayoutItem> Items = new List<LayoutItem>(); // contained items
            public override bool Visible                            // override visibility to affect contained items
            {
                get
                {
                    foreach (var item in Items) return item.Visible;
                    return base.Visible;
                }

                set
                {
                    base.Visible = value;
                    foreach (var item in Items) item.Visible = value;
                }
            }
            public override Vector2 Position                        // override position to affect contained items
            {
                get
                {
                    return base.Position;
                }
                set
                {
                    base.Position = value;
                    ApplyLayout();
                }
            }
            public override Vector2 Size                            // override size to affect contained items
            {
                get
                {
                    return base.Size;
                }

                set
                {
                    base.Size = value;
                    ApplyLayout();
                }
            }
            //-------------------------------------------------------------------
            // constructors
            //-------------------------------------------------------------------
            public LayoutArea(Screen screen, Vector2 position, Vector2 size, ScreenSprite item = null) : base(position, size, item)
            {
                this.screen = screen;
                screen.AddSprite(this);
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            // apply a layout to the contained items
            //---------------------------------------------------
            public virtual void ApplyLayout()   // default to a virtical list layout
            {
                //GridInfo.Echo("LayoutArea ApplyLayout: applying layout to " + Items.Count + " items");
                if (Items.Count == 0) return;
                float remainingHeight = MarginSize.Y;
                int flexibleCount = 0;
                //GridInfo.Echo("LayoutArea ApplyLayout: starting remaining height " + remainingHeight);
                // figure out how much height is left over after items with fixed heights are counted
                foreach (var item in Items)
                {
                    //GridInfo.Echo("LayoutArea ApplyLayout: item min height " + item.MinHeight + " flexible " + item.FlexibleHeight);
                    if (!item.FlexibleHeight) remainingHeight -= item.MinHeight;
                    else flexibleCount++;
                }
                //GridInfo.Echo("LayoutArea ApplyLayout: remaining height " + remainingHeight + " for " + flexibleCount + " flexible items");
                Vector2 currentPosition = MarginPosition;
                Vector2 itemSize = new Vector2(MarginSize.X, Size.Y);
                if(flexibleCount > 0) itemSize.Y = remainingHeight / flexibleCount;
                foreach (var item in Items)
                {
                    item.Position = currentPosition;
                    item.Size = itemSize;
                    currentPosition.Y += item.Size.Y;
                }
            }
            //---------------------------------------------------
            // add an item to the area
            //---------------------------------------------------
            public void AddItem(ScreenSprite item, bool flexibleWidth = true, bool flexibleHeight = true, Rectangle? margin = null)
            {
                LayoutItem layoutItem = new LayoutItem(item.Position,item.Size, item);
                layoutItem.FlexibleHeight = flexibleHeight;
                layoutItem.FlexibleWidth = flexibleWidth;
                if (margin.HasValue) layoutItem.Margin = margin.Value;
                Items.Add(layoutItem);
                screen.AddSprite(layoutItem);
            }
            public void AddItem(LayoutArea item)
            {
                Items.Add(item);
                screen.AddSprite(item);
            }
            //---------------------------------------------------
        }
        //-----------------------------------------------------------------------
    }
}
