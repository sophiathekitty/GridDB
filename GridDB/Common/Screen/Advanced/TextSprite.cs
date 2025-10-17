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
        // TextSprite - a sprite for displaying multiline text
        //----------------------------------------------------------------------
        public class TextSprite : ScreenSprite
        {
            //----------------------------------------------------------------------
            // fields
            //----------------------------------------------------------------------
            int scrollOffset = 0;   // current scroll offset
            int maxLines = 0;       // max lines that fit in the size
            public int ScrollOffset // line offset for scrolling
            {
                get { return scrollOffset; }
                set
                {
                    if (value == scrollOffset) return;
                    scrollOffset = value;
                    if (scrollOffset < 0) scrollOffset = 0;
                    if (scrollOffset > maxLines) scrollOffset = maxLines;
                    ScrollText();
                }
            }
            string text;            // raw text
            string data;            // text with line breaks
            public string Text      // get/set raw text
            {
                get { return text; }
                set
                {
                    if (value == text) return;
                    text = value;
                    ApplyText();
                }
            }
            //----------------------------------------------------------------------
            // apply text with word wrapping
            //----------------------------------------------------------------------
            void ApplyText()
            {
                if(_surface == null || text == "")
                {
                    return;
                }
                string[] lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                data = "";
                foreach (string line in lines)
                {
                    if (data != "") data += "\n";
                    if (MeasureStringInPixels(new StringBuilder(line)).X > Size.X) data += WrapText(line, Size.X);
                    else data += line;
                }
                FindMaxLines();
                if (data.Split('\n').Length > maxLines && maxLines > 0)
                {
                    // find the last line that fits
                    scrollOffset = 0;
                    ScrollText();
                }
                else Data = data;
            }
            //----------------------------------------------------------------------
            // find max lines that fit in the size
            //----------------------------------------------------------------------
            void FindMaxLines()
            {
                string[] lines = data.Split('\n');
                maxLines = 0;
                string testData = "";
                foreach (string line in lines)
                {
                    if (testData != "") testData += "\n";
                    testData += line;
                    if (MeasureStringInPixels(new StringBuilder(testData)).Y < Size.Y) maxLines++;
                }
            }
            //----------------------------------------------------------------------
            // scroll text to current offset
            //----------------------------------------------------------------------
            void ScrollText()
            {
                string[] lines = data.Split('\n');
                Data = "";
                for (int i = scrollOffset; i < maxLines; i++)
                {
                    Data += lines[i] + "\n";
                }
            }
            //----------------------------------------------------------------------
            // word wrap a single line of text to fit in maxWidth
            //----------------------------------------------------------------------
            string WrapText(string text, float maxWidth)
            {
                string[] words = text.Split(' ');
                StringBuilder currentLine = new StringBuilder();
                StringBuilder wrappedText = new StringBuilder();
                foreach (string word in words)
                {
                    StringBuilder testLine = new StringBuilder(currentLine.ToString());
                    if (currentLine.Length > 0) testLine.Append(" ");
                    testLine.Append(word);
                    if (MeasureStringInPixels(testLine).X > maxWidth)
                    {
                        wrappedText.AppendLine(currentLine.ToString());
                        currentLine.Clear();
                    }
                    if (currentLine.Length > 0) currentLine.Append(" ");
                    currentLine.Append(word);
                }
                wrappedText.AppendLine(currentLine.ToString());
                return wrappedText.ToString();
            }
            public override Vector2 Size
            {
                get { return base.Size; }
                set
                {
                    base.Size = value;
                    ApplyText();
                }
            }
            //----------------------------------------------------------------------
            // constructor
            //----------------------------------------------------------------------
            public TextSprite(Vector2 position, Vector2 size, string text, string fontId = "White", float Scale = 1f, Color color = default(Color), TextAlignment alignment = TextAlignment.LEFT, VerticalAlignments verticalAlignment = VerticalAlignments.None, ScreenSpriteAnchor anchor = ScreenSpriteAnchor.TopLeft)
            {
                if (color == default(Color)) color = Screen.DEFAULT_TEXT_COLOR;
                Position = position;
                Size = size;
                FontId = fontId;
                RotationOrScale = Scale;
                Color = color;
                Visible = true;
                Alignment = alignment;
                VerticalAlignment = verticalAlignment;
                Anchor = anchor;
                Type = SpriteType.TEXT;
                Data = text;
            }
            override public void SetViewport(RectangleF viewport, IMyTextSurface surface)
            {
                base.SetViewport(viewport, surface);
                Text = Data;
            }
        }
        //----------------------------------------------------------------------
    }
}
