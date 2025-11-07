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
        //----------------------------------------------------------------------
        // WindowLayout
        //----------------------------------------------------------------------
        public class WindowLayout : LayoutArea
        {
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            protected AppHeader header;
            protected AppFooter footer;
            protected HorizontalLayoutArea MainContentArea;
            protected Dictionary<string, LayoutArea> MainContent = new Dictionary<string, LayoutArea>();
            public string FileText
            {
                get { return header.FileText; }
                set { header.FileText = value; }
            }
            public string StatusText
            {
                get { return footer.StatusText; }
                set { footer.StatusText = value; }
            }
            public string KeyPromptText
            {
                get { return footer.KeyPromptText; }
                set { footer.KeyPromptText = value; }
            }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public WindowLayout(ScreenApp screen, Vector2 position, Vector2 size, string title, float borderWidth = 1) : base(screen, position, size, new TextureSprite(position,size,TextureSprite.SQUARE_SIMPLE,screen.AppStyle.BackgroundColor), borderWidth)
            {
                Margin = new Rectangle(0, 0, 0, 0);
                header = new AppHeader(screen, MarginPosition, MarginSize.X, screen.AppStyle.HeaderColor,title);
                footer = new AppFooter(screen, new Vector2(MarginPosition.X, MarginPosition.Y + MarginSize.Y - 36), MarginSize.X, screen.AppStyle.FooterColor,false);
                MainContentArea = new HorizontalLayoutArea(screen, new Vector2(MarginPosition.X, MarginPosition.Y + header.Item.Size.Y), new Vector2(MarginSize.X, MarginSize.Y - header.Item.Size.Y - footer.Item.Size.Y), borderWidth: 1);
                AddItem(header);
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
                if (MainContent.ContainsKey(name)) return;
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
                MainContentArea.AddItem(area, index + 1);
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
            //-------------------------------------------------------------------
            // remove from screen
            public override void RemoveFromScreen()
            {
                base.RemoveFromScreen();
                header.RemoveFromScreen();
                footer.RemoveFromScreen();
                MainContentArea.RemoveFromScreen();
            }
        }
        //----------------------------------------------------------------------
    }
}
