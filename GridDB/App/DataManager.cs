using EmptyKeys.UserInterface.Generated.EditFactionIconView_Bindings;
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
        // DataManager - application screen for managing stored data
        //----------------------------------------------------------------------
        public class DataManager : ScreenApp
        {
            public static void Init()
            {
                //GridInfo.Echo("DataManager init");
                // static init
                RegisterApp();
                ScreenAppSeat.GetSeat("Test.Seat", "DataManager", "DataManager");
            }
            static void RegisterApp()
            {
                AvailableApps["DataManager"] = LaunchApp;
            }
            static void LaunchApp(ScreenAppSeat seat)
            {
                new DataManager(seat);
            }
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            TextSprite header;
            HorizontalLayoutArea contentArea;
            TextSprite footer;
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public DataManager(ScreenAppSeat seat) : base(seat, "DataManager")
            {
                header = new TextSprite(new Vector2(0,30), new Vector2(600, 64), "Data Manager", "White", 1.75f,alignment: TextAlignment.CENTER, anchor: ScreenSprite.ScreenSpriteAnchor.TopCenter);
                header.Text = "Data Manager";
                if (soundBlocks.Count > 0)
                {
                    header.Text += " (Audio)";
                }
                if(input.IsValid)
                {
                    header.Text += " (Input)";
                }
                AddSprite(header);
                footer = new TextSprite(new Vector2(0, -30), new Vector2(512, 64), "", "White", 1.0f, alignment: TextAlignment.CENTER, anchor: ScreenSprite.ScreenSpriteAnchor.BottomCenter);
                AddSprite(footer);
                contentArea = new HorizontalLayoutArea(this,Vector2.One * 10, Size - Vector2.One * 20);
                LayoutArea sideBar = new LayoutArea(this, contentArea.MarginPosition, new Vector2(150, contentArea.MarginSize.Y));
                sideBar.FlexibleHeight = true; sideBar.FlexibleWidth = false;
                LayoutArea contentBody = new LayoutArea(this, contentArea.MarginPosition, new Vector2(contentArea.MarginSize.X - 160, contentArea.MarginSize.Y));
                contentBody.FlexibleHeight = true; contentBody.FlexibleWidth = true;
                Vector2 cs = new Vector2(150, 50);
                sideBar.AddItem(new TextSprite(contentArea.MarginPosition,cs, "content item one"), flexibleHeight: false);
                sideBar.AddItem(new TextSprite(contentArea.MarginPosition,cs, "content item two"));
                sideBar.AddItem(new TextSprite(contentArea.MarginPosition,cs, "content item three"));
                cs = new Vector2(contentArea.MarginSize.X - 160, 50);
                contentBody.AddItem(new TextSprite(contentArea.MarginPosition, cs, "main content area header"), flexibleHeight: false);
                contentBody.AddItem(new TextSprite(contentArea.MarginPosition, cs, "main content area body"));
                contentArea.AddItem(sideBar);
                contentArea.AddItem(contentBody);
                contentArea.ApplyLayout();
                //contentArea.AddToScreen(this);
            }
            //----------------------------------------------------------------------
            // main update loop
            //----------------------------------------------------------------------
            public override void Update(string argument)
            {
                string txt = "";
                if (input.IsValid)
                {
                    if (input.SpaceReleased)
                    {
                        txt += " (Space Released)";
                        seat.CurrentApp = "WebBrowser";
                    }
                    if (input.AReleased)
                    {
                        contentArea.Size -= Vector2.One * 50;
                    }
                    if (input.QReleased)
                    {
                        txt += " (Q Released)";
                        seat.CurrentAppId = new ScreenAppId("LauncherHub","GameEditor");
                    }
                    input.Reset();
                }
                //footer.Text = txt;
                footer.Text = txt;
                base.Update(argument);
            }
        }
        //----------------------------------------------------------------------
    }
}
