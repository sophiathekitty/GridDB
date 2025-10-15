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
                GridInfo.Echo("DataManager init");
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
                    if (input.EReleased)
                    {
                        txt += " (E Released)";
                        if(GridInfo.IGC.SendUnicastMessage(ScriptId("GameEditor"), "AvailableApps", "testing baby"))
                        {
                            txt += " (Message Sent)";
                        }
                        else
                        {
                            txt += " (Message Failed)";
                        }
                    }
                    if (input.QReleased)
                    {
                        txt += " (Q Released)";
                        
                    }
                    input.Reset();
                }
                footer.Text = txt;
                base.Update(argument);
            }
        }
        //----------------------------------------------------------------------
    }
}
