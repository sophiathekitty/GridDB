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
        // LayoutStyle
        //-----------------------------------------------------------------------
        public class LayoutStyle
        {
            //-------------------------------------------------------
            // static fields
            //------------------------------------------------------
            public static Color DEFAULT_TEXT_COLOR = Color.White;                   // text color
            public static Color DEFAULT_HIGHLIGHT_TEXT_COLOR = Color.Yellow;        // highlight color
            public static Color DEFAULT_BACKGROUND_COLOR = Color.DarkBlue;          // highlight background color
            //-------------------------------------------------------
            // instance fields
            //-------------------------------------------------------
            Screen screen;                                                          // reference to screen for colors
            public Color BackgroundColor { get { return screen.BackgroundColor; } } // get from screen
            public Color BackgroundTintColor
            {
                get
                {
                    return new Color(
                        (int)(BackgroundColor.R * 1.25f),
                        (int)(BackgroundColor.G * 1.25f),
                        (int)(BackgroundColor.B * 1.25f));
                }
            }
            public Color BackgroundShadeColor
            {
                get
                {
                    return new Color(
                        (int)(BackgroundColor.R * 0.75f),
                        (int)(BackgroundColor.G * 0.75f),
                        (int)(BackgroundColor.B * 0.75f));
                }
            }
            public Color ForegroundColor { get { return screen.ForegroundColor; } } // get from screen
            public Color ForegroundComplimentaryColor                               // get the complimentary color
            {
                get
                {
                    return new Color(
                        255 - ForegroundColor.R,
                        255 - ForegroundColor.G,
                        255 - ForegroundColor.B);
                }
            }
            public Color TextColor = DEFAULT_TEXT_COLOR;
            public Color HighlightTextColor = DEFAULT_HIGHLIGHT_TEXT_COLOR;
            public Color BorderColor = Color.Gray * 0.5f;
            public Color HeaderColor = Color.DarkGray * 0.75f;
            public Color FooterColor = Color.Gray * 0.75f;
            public Rectangle Padding;
            public LayoutStyle(Screen screen, int padding = 5)
            {
                this.screen = screen;
                Padding = new Rectangle(padding, padding, padding, padding);
            }
        }
        //-----------------------------------------------------------------------
    }
}
