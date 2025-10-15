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
        // WebBrowser - web browser class for space engineers
        //----------------------------------------------------------------------
        public class WebBrowser : ScreenApp
        {
            //------------------------------------------------------
            // static methods
            //------------------------------------------------------
            public static void Init()
            {
                GridInfo.Echo("WebBrowser.Init");
                RegisterApp();
            }
            static void RegisterApp()
            {
                AvailableApps["WebBrowser"] = LaunchApp;
            }
            static void LaunchApp(ScreenAppSeat seat)
            {
                new WebBrowser(seat);
            }
            //------------------------------------------------------
            // fields
            //------------------------------------------------------
            TextSprite Header;
            TextSprite Body;
            //------------------------------------------------------
            // constructor
            //------------------------------------------------------
            public WebBrowser(ScreenAppSeat seat) : base(seat, "WebBrowser")
            {
                Header = new TextSprite(new Vector2(0, 30), new Vector2(600, 64), "Web Browser", "White", 1.75f, alignment: TextAlignment.CENTER, anchor: ScreenSprite.ScreenSpriteAnchor.TopCenter);
                AddSprite(Header);
                Body = new TextSprite(new Vector2(0, 80), new Vector2(512, 512), "No web access in SE", "White", 1.25f, alignment: TextAlignment.LEFT, anchor: ScreenSprite.ScreenSpriteAnchor.TopLeft);
                AddSprite(Body);
                seat[AppId] = this;
            }
            public override void Update(string argument)
            {
                Body.Text = "Website under construction...";
                if(input.IsValid)
                {
                    if (input.W) { Body.Text += "\nW"; }
                    if (input.A) { Body.Text += "\nA"; }
                    if (input.S) { Body.Text += "\nS"; }
                    if (input.D) { Body.Text += "\nD"; }
                    if (input.Space) { Body.Text += "\nSpace"; }
                    if (input.C) { Body.Text += "\nC"; }
                    if (input.E) { Body.Text += "\nE"; }
                    if (input.QReleased)
                    {
                        GridInfo.Echo("WebBrowser: CloseApp");
                        seat.CloseApp();
                    }
                    input.Reset();
                }
            }
        }
        //----------------------------------------------------------------------
    }
}
