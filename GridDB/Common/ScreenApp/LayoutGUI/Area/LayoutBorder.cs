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
        // cursor for the map editor
        //-----------------------------------------------------------------------
        public class LayoutBorder : IScreenSpriteProvider
        {
            TextureSprite top;
            TextureSprite bottom;
            TextureSprite left;
            TextureSprite right;
            Vector2 size = new Vector2(1, 1);
            public float lineThickness { get; private set; } = 2f;
            public float Scale { get { return lineThickness; } set { lineThickness = value; Size = size; } }

            public LayoutBorder(Vector2 size, float lineThickness)
            {
                this.lineThickness = lineThickness;
                top = new TextureSprite(Vector2.Zero, new Vector2(size.X, lineThickness), "SquareSimple",Color.White);
                bottom = new TextureSprite(new Vector2(0, size.Y), new Vector2(size.X, lineThickness), "SquareSimple", Color.White);
                left = new TextureSprite(Vector2.Zero, new Vector2(lineThickness, size.Y), "SquareSimple", Color.White);
                right = new TextureSprite(new Vector2(size.X, 0), new Vector2(lineThickness, size.Y), "SquareSimple", Color.White);
                Size = size;
            }
            public LayoutBorder(Vector2 position, Vector2 size, float lineThickness) : this(size, lineThickness)
            {
                Position = position;
            }
            public LayoutBorder(Vector2 position, Vector2 size, float lineThickness, Color color) : this(size, lineThickness)
            {
                Position = position;
                top.Color = color;
                bottom.Color = color;
                left.Color = color;
                right.Color = color;
            }
            public Vector2 Position
            {
                get
                {
                    return top.Position;
                }
                set
                {
                    top.Position = value;
                    bottom.Position = value + new Vector2(0, size.Y);
                    left.Position = value;
                    right.Position = value + new Vector2(size.X,0);
                }
            }
            public Vector2 Size
            {
                get
                {
                    return size;
                }
                set
                {
                    size = value;
                    bottom.Position = top.Position + new Vector2(0, size.Y);
                    right.Position = top.Position + new Vector2(size.X,0);
                    left.Position = top.Position + new Vector2(0,0);
                    top.Size = new Vector2(size.X, lineThickness);
                    left.Size = new Vector2(lineThickness, size.Y);
                    bottom.Size = new Vector2(size.X, lineThickness);
                    right.Size = new Vector2(lineThickness, size.Y);
                }
            }
            public bool Visible
            {
                get
                {
                    return top.Visible;
                }

                set
                {
                    top.Visible = value;
                    bottom.Visible = value;
                    left.Visible = value;
                    right.Visible = value;
                }
            }
            public Color Color
            {
                get
                {
                    return top.Color;
                }
                set
                {
                    top.Color = value;
                    bottom.Color = value;
                    left.Color = value;
                    right.Color = value;
                }
            }
            public bool GetSizeFromParent { get; set; } = true;


            public void AddToScreen(Screen screen, int layer = 0)
            {
                screen.AddSprite(top, layer);
                screen.AddSprite(bottom, layer);
                screen.AddSprite(left, layer);
                screen.AddSprite(right, layer);
            }
            public void RemoveFromScreen(Screen screen)
            {
                screen.RemoveSprite(top);
                screen.RemoveSprite(bottom);
                screen.RemoveSprite(left);
                screen.RemoveSprite(right);
            }

            //-----------------------------------------------------------------------
            // IScreenSprite
            //-----------------------------------------------------------------------
            public Vector2 GetPosition()
            {
                return Position;
            }
            public bool Overlaps(IScreenSprite other)
            {
                Vector2 position = Position;
                Vector2 size = Size;
                Vector2 otherPosition = other.GetPosition();
                Vector2 otherSize = other.SizeOnScreen;
                return position.X < otherPosition.X + otherSize.X && position.X + size.X > otherPosition.X && position.Y < otherPosition.Y + otherSize.Y && position.Y + size.Y > otherPosition.Y;

            }
            public Vector2 SizeOnScreen
            {
                get
                {
                    return size;
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
