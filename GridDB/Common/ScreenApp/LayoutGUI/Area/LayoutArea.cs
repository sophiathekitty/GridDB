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
            // static methods
            //-------------------------------------------------------------------
            public static LayoutArea DOM(LayoutArea parent, string path)    // get a sub-area by path 0.0.0
            {
                string[] parts = path.Split('.');
                LayoutArea current = parent;
                foreach (var part in parts)
                {
                    bool found = false;
                    int index = -1;
                    if (int.TryParse(part, out index))
                    {
                        if (index >= 0 && index < current.Items.Count)
                        {
                            if (current.Items[index] is LayoutArea)
                            {
                                current = current.Items[index] as LayoutArea;
                                found = true;
                            }
                        }
                    }
                    if (!found) return null;
                }
                return current;
            }
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            public ScreenApp screen { get; private set; }           // screen to draw to
            public List<LayoutItem> Items = new List<LayoutItem>(); // contained items
            public LayoutBorder border;
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
            public override Color TextColor                         // override text color to affect contained items
            {
                get
                {
                    foreach (var item in Items)
                    {
                        if (item.Item is TextSprite) return (item.Item as TextSprite).Color;
                    }
                    return base.TextColor;
                }

                set
                {
                    base.TextColor = value;
                    foreach (var item in Items)
                    {
                        item.TextColor = value;
                    }
                }
            }
            //-------------------------------------------------------------------
            // constructors
            //-------------------------------------------------------------------
            public LayoutArea(ScreenApp screen, Vector2 position, Vector2 size, ScreenSprite item = null, float borderWidth = 0) : base(position, size, item)
            {
                this.screen = screen;
                screen.AddSprite(this);
                if (borderWidth > 0)
                {
                    border = new LayoutBorder(position, size, borderWidth);
                    screen.AddSprite(border);
                }
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            // apply a layout to the contained items
            //---------------------------------------------------
            public virtual void ApplyLayout()   // default to a virtical list layout
            {
                if(border != null)
                {
                    border.Position = Position;
                    border.Size = Size;
                }
                if(Item != null)
                {
                    Item.Position = Position;
                    Item.Size = Size;
                }
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
            // [] for contained items
            //---------------------------------------------------
            public LayoutItem this[int index] => Items[index];
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
            public void AddItem(LayoutArea item, int index)
            {
                Items.Insert(index, item);
                screen.AddSprite(item);
            }
            public void AddItem(ScreenSprite item, ref Action<string> UpdateVar, bool flexibleWidth = true, bool flexibleHeight = true, Rectangle? margin = null)
            {
                LayoutItem layoutVariable = new LayoutItem(item.Position, item.Size, item, ref UpdateVar);
                layoutVariable.FlexibleHeight = flexibleHeight;
                layoutVariable.FlexibleWidth = flexibleWidth;
                if (margin.HasValue) layoutVariable.Margin = margin.Value;
                else layoutVariable.Margin = screen.AppStyle.Padding;
                Items.Add(layoutVariable);
                screen.AddSprite(layoutVariable);
            }
            public void AddItemAt(LayoutArea item, int index)
            {
                Items.Insert(index, item);
                screen.AddSprite(item);
            }
            //---------------------------------------------------
            // remove an item from the area
            //---------------------------------------------------
            public void RemoveItem(LayoutItem item)
            {
                Items.Remove(item);
                screen.RemoveSprite(item);
                if (item is LayoutArea)
                {
                    LayoutArea area = item as LayoutArea;
                    if(area.border != null) screen.RemoveSprite(area.border);
                    while (area.Items.Count > 0) area.RemoveItem(area.Items[0]);
                }
            }
            public void RemoveLastItem()
            {
                if (Items.Count == 0) return;
                RemoveItem(Items[Items.Count - 1]);
            }
            //---------------------------------------------------
        }
        //-----------------------------------------------------------------------
    }
}
