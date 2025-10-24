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
        // LayoutMenu
        //-----------------------------------------------------------------------
        public class LayoutMenu : InteractableGroup
        {
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public LayoutMenu(ScreenApp screen, Vector2 position, Vector2 size, string header = "", ScreenSprite item = null, float borderWidth = 0) : base(screen, position, size, item, borderWidth)
            {
                if(header != "")
                {
                    AddItem(new TextSprite(position, new Vector2(size.X, 30), header, Scale: 1.25f), true, false);
                    Id = "Menu." + header;
                }
                else Id = "Menu";
            }
            //-------------------------------------------------------------------
            // menu item clicked event pass-through
            //-------------------------------------------------------------------
            void ItemClicked(IInteractable item)
            {
                //GridInfo.Echo("Menu item clicked: " + item.Id);
                OnClick?.Invoke(item);
            }
            //-------------------------------------------------------------------
            // add menu item
            //-------------------------------------------------------------------
            public void AddMenuItem(LayoutButton item, string DOM = "")
            {
                item.OnClick += ItemClicked;
                if(DOM != "") AddInteractable(item, DOM);
                else AddInteractable(item);
            }
            public void AddMenuItem(Vector2 size, string button_text, string button_id, TextureSprite bg = null, string button_value = "", float borderWidth = 0, string DOM = "")
            {
                LayoutButton item = new LayoutButton(screen, Vector2.Zero, size, button_text, button_id, button_value, bg, borderWidth);
                AddMenuItem(item,DOM);
            }
            public void AddMenuItem(Vector2 size, string button_text, string icon, string button_id, string button_value = "", TextureSprite bg = null, float borderWidth = 0, string DOM = "")
            {
                LayoutButton item = new LayoutButton(screen, Vector2.Zero, size, button_text, icon, button_id, button_value, bg, borderWidth);
                AddMenuItem(item, DOM);
            }
        }
        //-------------------------------------------------------------------
    }
}
