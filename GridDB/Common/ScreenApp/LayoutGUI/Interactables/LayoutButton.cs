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
        // LayoutButton
        //-----------------------------------------------------------------------
        public class LayoutButton : HorizontalLayoutArea, IInteractable
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            TextSprite label;
            RasterSprite icon;
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public LayoutButton(ScreenApp screen, Vector2 position, Vector2 size, string button_text, string button_id, string button_value = "", TextureSprite bg = null, float borderWidth = 0) : base(screen, position, size, bg, borderWidth)
            {
                Id = button_id;
                Value = button_value;
                // add text label
                label = new TextSprite(position, size, button_text);
                AddItem(label, true, false);
            }
            // lable and raster sprite icon
            public LayoutButton(ScreenApp screen, Vector2 position, Vector2 size, string button_text, string icon, string button_id, string button_value = "", TextureSprite bg = null, float borderWidth = 0) : base(screen, position, size, bg, borderWidth)
            {
                Id = button_id;
                Value = button_value;
                // add icon
                this.icon = new RasterSprite(position, RasterSprite.DEFAULT_PIXEL_SCALE, Vector2.Zero, icon);
                AddItem(this.icon, false, false);
                // add text label
                label = new TextSprite(position, size, button_text);
                AddItem(label, true, false);
            }
            public string Text                                      // get or set button text
            {
                get
                {
                    return label.Text;
                }
                set
                {
                    label.Text = value;
                }
            }
            public float TestSize                                // get or set button text size
            {
                get
                {
                    return label.RotationOrScale;
                }
                set
                {
                    label.RotationOrScale = value;
                }
            }
            public string Icon                                     // get or set button icon
            {
                get
                {
                    if(icon == null) return "";
                    return icon.Image;
                }
                set
                {
                    if (icon != null) icon.Image = value;
                }
            }
            //-------------------------------------------------------------------
            // IInteractable implementation
            //-------------------------------------------------------------------
            public string Id { get; set; }                              // unique identifier for the item
            public string Value { get; set; }                           // value associated with the item
            public Vector2 Center                                       // center point of the item
            {
                get
                {
                    return Position + (Size / 2);
                }
            }
            bool focused = false;                                       // is the item currently focused
            public bool IsFocused                                       // is the item currently focused
            {
                get
                {
                    return focused;
                }

                set
                {
                    focused = value;
                    if (focused)
                    {
                        (this as IInteractable).OnFocus?.Invoke(this);
                        TextColor = Color.Yellow;
                    }
                    else
                    {
                        (this as IInteractable).OnBlur?.Invoke(this);
                        TextColor = Color.White;
                    }
                    if(border != null)
                    {
                        border.Color = focused ? Color.Yellow : Color.White;
                    }
                }
            }
            public string KeyPrompt { get; set; }                       // prompt to show when requesting keyboard input
            LayoutArea IInteractable.Area                               // the area the item is contained in
            {
                get
                {
                    return this;
                }
            }
            public Action<IInteractable> OnFocus { get; set; }   // when the item receives focus (hovered)
            public Action<IInteractable> OnBlur { get; set; }    // when the item loses focus (unhovered)
            public Action<IInteractable> OnClick { get; set; }   // when the item is clicked
            public Action LayoutChanged { get; set; }            // when the layout of the item changes
            public void Click()                                         // perform click action
            {
                OnClick?.Invoke(this);
            }
            bool IInteractable.RunInput(GameInput input)
            {
                return false;
            }
        }
        //-------------------------------------------------------------------
    }
}
