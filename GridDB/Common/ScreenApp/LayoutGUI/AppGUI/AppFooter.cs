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
        // AppFooter
        //-----------------------------------------------------------------------
        public class AppFooter : HorizontalLayoutArea
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            const float _height = 34f;  // footer height
            const int _padding = 1;     // padding around items
            TextSprite statusText;      // status text
            TextSprite keyPromptText;
            public string StatusText
            {
                get { return statusText.Text; }
                set { statusText.Text = value; }
            }
            public string KeyPromptText
            {
                get { return keyPromptText.Text; }
                set { keyPromptText.Text = value; }
            }
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            public AppFooter(ScreenApp app, Vector2 position, float width, Color backgroundColor, bool isApp = true) : base(app, position, new Vector2(width, _height + _padding + _padding), new TextureSprite(position, new Vector2(width, _height + _padding + _padding), "SquareSimple", backgroundColor), 2)
            {
                app.AddSprite(Item);
                Rectangle padding = new Rectangle(_padding, _padding, _padding, _padding);
                statusText = new TextSprite(MarginPosition, new Vector2(200, _height), app.Status, Scale: 0.55f);
                if(isApp) AddItem(statusText, ref app.StatusChanged, false, true, padding);
                else AddItem(statusText, false, true, padding);
                keyPromptText = new TextSprite(MarginPosition, new Vector2(300, _height), app.KeyPrompt, Scale: 0.65f, alignment: TextAlignment.RIGHT);
                if(isApp) AddItem(keyPromptText, ref app.KeyPromptChanged, true, true, padding);
                else AddItem(keyPromptText, true, true, padding);
                FlexibleHeight = false;
            }
        }
        //-----------------------------------------------------------------------
    }
}
