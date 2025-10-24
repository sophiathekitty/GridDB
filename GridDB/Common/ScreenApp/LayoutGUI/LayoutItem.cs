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
        // LayoutItem
        //-----------------------------------------------------------------------
        public class LayoutItem : IScreenSpriteProvider
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            //TextureSprite CellBack;                                     // position and size of the cell the item can occupy...
            //TextureSprite MarginBack;                                   // position and size of the cell with margin applied...
            public ScreenSprite Item;                                   // the actual item to display
            Vector2 position = new Vector2(0, 0);                       // relative to screen
            Vector2 size = new Vector2(1, 1);                           // overall size of the item including margin and padding
            public virtual Vector2 Position                             // relative to screen
            {
                get
                {
                    return position;
                }
                set
                {
                    position = value;
                    if (Item != null) Item.Position = MarginPosition;
                    //if (CellBack != null) CellBack.Position = Position;
                    //if (MarginBack != null) MarginBack.Position = MarginPosition;
                }
            }
            public Vector2 MarginPosition                               // position of the item inside the margin
            {
                get
                {
                    return Position + new Vector2(Margin.Left, Margin.Top);
                }
            }
            public virtual Vector2 Size                                 // overall size of the item including margin and padding
            {
                get
                {
                    return size;
                }
                set
                {
                    Vector2 s = value;
                    if (!FlexibleWidth) s.X = size.X;
                    if (!FlexibleHeight) s.Y = size.Y;
                    size = s;
                    if (Item != null) Item.Size = MarginSize;
                    //if (CellBack != null) CellBack.Size = size;
                    //if (MarginBack != null) MarginBack.Size = MarginSize;
                }
            }
            public Vector2 MarginSize                                   // size available for the item inside the margin
            {
                get
                {
                    return Size - new Vector2(Margin.Right, Margin.Bottom);
                }
            }
            public Rectangle Margin = new Rectangle(5,5,5,5);        // space between item border and other items
            public bool FlexibleWidth = true;                           // can the item expand in width
            public bool FlexibleHeight = false;                         //  can the item expand in height
            Vector2 minSize = new Vector2(16, 16);                      // minimum size of the item
            public float MinWidth                                       // minimum width of the item
            {
                get
                {
                    if(!FlexibleWidth) return size.X;
                    float width = minSize.X + Margin.Right;
                    if (Item != null) width = Math.Min(width, Item.SizeOnScreen.X + Margin.Right);
                    return width;
                }
            }
            public float MinHeight                                      // minimum height of the item
            {
                get
                {
                    if (!FlexibleHeight) return size.Y;
                    float height = minSize.Y + Margin.Bottom;
                    if (Item != null) height = Math.Min(height, Item.SizeOnScreen.Y + Margin.Bottom);
                    return height;
                }
            }
            public virtual Color TextColor                              // text color for TextSprite items
            {
                get
                {
                    if (Item is TextSprite) return (Item as TextSprite).Color;
                    return Color.White;
                }
                set
                {
                    if (Item is TextSprite) (Item as TextSprite).Color = value;
                }
            }
            public Color BackgroundColor                                // background color for TextureSprite items
            {
                get
                {
                    if (Item is TextureSprite) return (Item as TextureSprite).Color;
                    return Color.Transparent;
                }
                set
                {
                    if (Item is TextureSprite) (Item as TextureSprite).Color = value;
                }
            }

            // store a reference to the ref Action<string> UpdateVar passed in constructor
            Action<string> UpdateVar = null;
            bool hasUpdateVar = false;
            //-------------------------------------------------------------------
            // IScreenSpriteProvider implementation
            //-------------------------------------------------------------------
            public virtual bool Visible             // visibility of the item
            {
                get
                {
                    if(Item == null) return false;
                    return Item.Visible;
                }

                set
                {
                    if (Item == null) return;
                    Item.Visible = value;
                }
            }            
            public virtual Vector2 SizeOnScreen     // size on screen
            {
                get
                {
                    if (Item == null) return Vector2.Zero;
                    return Item.SizeOnScreen;
                }
            }
            public virtual Vector2 GetPosition()    // position on screen
            {
                if (Item == null) return Vector2.Zero;
                return Item.GetPosition();
            }
            //-------------------------------------------------------------------
            // Explicit interface implementations
            //-------------------------------------------------------------------
            public void AddToScreen(Screen screen, int layer)                   // add to screen
            {
                //if (CellBack != null) screen.AddSprite(CellBack, layer);
                //if (MarginBack != null) screen.AddSprite(MarginBack, layer);
                if (Item != null) screen.AddSprite(Item, layer);
            }
            bool IScreenSprite.Overlaps(IScreenSprite other)                    // check for overlap with another sprite
            {
                if(Item == null || other == null) return false;
                return Item.Overlaps(other);
            }
            void IScreenSpriteProvider.RemoveFromScreen(Screen screen)          // remove from screen
            {
                if (Item != null) screen.RemoveSprite(Item);
                if (hasUpdateVar) UpdateVar -= UpdateVariable;
            }
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public LayoutItem(Vector2 position, Vector2 size, ScreenSprite item = null)
            {
                Position = position;
                this.size = size;
                minSize = size;
                if (item != null)
                {
                    Item = item;
                    Item.Position = MarginPosition;
                    Item.Size = MarginSize;
                }
                //CellBack = new TextureSprite(position, size, "SquareSimple", Color.DarkGray * 0.5f);
                //MarginBack = new TextureSprite(MarginPosition, MarginSize, "SquareSimple", Color.Gray * 0.5f);
            }
            public LayoutItem(Vector2 position, Vector2 size, ScreenSprite item, ref Action<string> UpdateVar) : this(position, size, item)
            {
                UpdateVar += UpdateVariable;
                this.UpdateVar = UpdateVar;
                hasUpdateVar = true;
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            void UpdateVariable(string value)
            {
                if (value == null) return;
                if (Item is TextSprite) (Item as TextSprite).Text = value;
                else if (Item is RasterSprite) (Item as RasterSprite).Image = value;
            }
            // virtual [int] for contained items placeholder
        }
        //-----------------------------------------------------------------------
    }
}
