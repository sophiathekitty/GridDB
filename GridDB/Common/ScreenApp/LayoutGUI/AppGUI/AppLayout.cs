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
        // AppLayout
        //-----------------------------------------------------------------------
        public class AppLayout : LayoutArea
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            HorizontalLayoutArea MainContentArea;
            Dictionary<string, LayoutArea> MainContent = new Dictionary<string, LayoutArea>();
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public AppLayout(ScreenApp app, Vector2 position, Vector2 size, Color HeaderBackgroundColor, Color FooterBackgroundColor, ScreenSprite item = null, float borderWidth = 0, int margin = 1) : base(app, position, size, item, borderWidth)
            {
                Margin = new Rectangle(margin,margin,margin,margin);
                AppHeader header = new AppHeader(app, MarginPosition, size.X, HeaderBackgroundColor);
                AppFooter footer = new AppFooter(app, MarginPosition, size.X, FooterBackgroundColor);
                AddItem(header);
                MainContentArea = new HorizontalLayoutArea(app, MarginPosition, MarginSize, borderWidth:1);
                MainContentArea.FlexibleHeight = true;
                MainContentArea.FlexibleWidth = true;
                AddItem(MainContentArea);
                AddItem(footer);
            }
            //-------------------------------------------------------------------
            // methods
            //-------------------------------------------------------------------
            // Add main content areas
            //-------------------------------------------------------------------
            public void AddContent(string name, LayoutArea area)
            {
                if(MainContent.ContainsKey(name)) return;
                MainContent[name] = area;
                MainContentArea.AddItem(area);
            }
            public void AddContent(string name, LayoutArea area, int index)
            {
                if (MainContent.ContainsKey(name)) return;
                MainContent[name] = area;
                MainContentArea.AddItem(area, index);
            }
            public void AddContent(string name, LayoutArea area, string after)
            {
                if (MainContent.ContainsKey(name)) return;
                int index = MainContentArea.Items.IndexOf(MainContent[after]);
                MainContent[name] = area;
                MainContentArea.AddItem(area, index+1);
            }
            //-------------------------------------------------------------------
            // Remove content area by name
            //-------------------------------------------------------------------
            public void RemoveContent(string name)
            {
                if (MainContent.ContainsKey(name))
                {
                    LayoutArea area = MainContent[name];
                    MainContentArea.RemoveItem(area);
                    MainContent.Remove(name);
                }
            }
            //-------------------------------------------------------------------
            // [] for main content areas
            //-------------------------------------------------------------------
            public LayoutArea this[string name]
            {
                get
                {
                    if (MainContent.ContainsKey(name))
                    {
                        return MainContent[name];
                    }
                    return null;
                }
                set
                {
                    if (MainContent.ContainsKey(name))
                    {
                        LayoutArea area = MainContent[name];
                        MainContentArea.RemoveItem(area);
                        MainContentArea.AddItem(value);
                        MainContent[name] = value;
                    }
                    else
                    {
                        MainContent[name] = value;
                        MainContentArea.AddItem(value);
                    }
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
