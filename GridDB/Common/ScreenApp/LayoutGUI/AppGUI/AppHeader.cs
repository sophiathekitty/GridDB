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
        // AppHeader
        //-----------------------------------------------------------------------
        public class AppHeader : HorizontalLayoutArea
        {
            //-------------------------------------------------------------------
            // fields
            //-------------------------------------------------------------------
            const float _height = 60f;
            const int _padding = 2;
            TextSprite titleText;
            TextSprite fileText;
            public string FileText
            {
                get { return fileText.Text; }
                set { fileText.Text = value; }
            }
            //-------------------------------------------------------------------
            // constructor
            //-------------------------------------------------------------------
            // screen app header
            public AppHeader(ScreenApp app, Vector2 position, float width, Color backgroundColor) : base(app, position, new Vector2(width,_height+_padding+_padding), new TextureSprite(position, new Vector2(width,_height+_padding+_padding), "SquareSimple", backgroundColor),2)
            {
                app.AddSprite(Item);
                Rectangle padding = new Rectangle(_padding, _padding, _padding, _padding);
                titleText = new TextSprite(MarginPosition, new Vector2(250, _height), app.Name, Scale:1.35f);
                AddItem(titleText,false,true, padding);
                fileText = new TextSprite(MarginPosition, new Vector2(150, _height), app.fileName, Scale: 1.25f);
                AddItem(fileText, ref app.FileNameChanged, true, true, padding);
                FlexibleHeight = false;
            }
            // window header (gets name from string not app)
            public AppHeader(ScreenApp app, Vector2 position, float width, Color backgroundColor, string title) : base(app, position, new Vector2(width, _height + _padding + _padding), new TextureSprite(position, new Vector2(width, _height + _padding + _padding), "SquareSimple", backgroundColor), 2)
            {
                app.AddSprite(Item);
                Rectangle padding = new Rectangle(_padding, _padding, _padding, _padding);
                titleText = new TextSprite(MarginPosition, new Vector2(250, _height), title, Scale: 1.35f);
                AddItem(titleText, false, true, padding);
                FlexibleHeight = false;
                fileText = new TextSprite(MarginPosition, new Vector2(150, _height), "", Scale: 1.25f);
                AddItem(fileText, ref app.FileNameChanged, true, true, padding);
            }
        }
        //-------------------------------------------------------------------
    }
}
