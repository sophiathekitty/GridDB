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
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // screen sprite - encapsulates a sprite
        //----------------------------------------------------------------------
        public class ScreenSprite : IScreenSprite
        {
            public static float DEFAULT_FONT_SIZE = 1.5f;
            public static float MONOSPACE_FONT_SIZE = 0.2f;
            public enum ScreenSpriteAnchor
            {
                TopLeft,
                TopCenter,
                TopRight,
                CenterLeft,
                Center,
                CenterRight,
                BottomLeft,
                BottomCenter,
                BottomRight
            }
            public enum VerticalAlignments
            {
                None,
                Top,
                Center,
                Bottom
            }
            public ScreenSpriteAnchor Anchor { get; set; }
            public Vector2 Position { get; set; }
            public float RotationOrScale { get; set; }
            public virtual Vector2 Size { get; set; }
            public Vector2 SizeOnScreen
            {
                get
                {
                    if (Type == SpriteType.TEXT)
                    {
                        return _surface.MeasureStringInPixels(new StringBuilder(Data), FontId, RotationOrScale);
                    }
                    else
                    {
                        return Size;
                    }
                }
            }
            public Color Color { get; set; }
            public string FontId { get; set; }
            public string Data { get; set; }
            public TextAlignment Alignment { get; set; }
            public SpriteType Type { get; set; }
            public VerticalAlignments VerticalAlignment { get; set; }
            public bool Visible { get; set; } = true;
            RectangleF _viewport;
            IMyTextSurface _surface;
            public virtual void SetViewport(RectangleF viewport, IMyTextSurface surface)
            {
                _viewport = viewport;
                _surface = surface;
            }
            // constructor
            public ScreenSprite()
            {
                Anchor = ScreenSpriteAnchor.Center;
                Position = Vector2.Zero;
                RotationOrScale = 1f;
                Size = Vector2.Zero;
                Color = Color.White;
                FontId = "White";
                Data = "";
                Alignment = TextAlignment.CENTER;
                Type = SpriteType.TEXT;
                VerticalAlignment = VerticalAlignments.None;
            }
            // constructor
            public ScreenSprite(ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Vector2 size, Color color, string fontId, string data, TextAlignment alignment, SpriteType type, VerticalAlignments verticalAlignment = VerticalAlignments.None)
            {
                Anchor = anchor;
                Position = position;
                RotationOrScale = rotationOrScale;
                Size = size;
                Color = color;
                FontId = fontId;
                Data = data;
                Alignment = alignment;
                Type = type;
                VerticalAlignment = verticalAlignment;
            }
            // convert the sprite to a MySprite
            public virtual MySprite ToMySprite(RectangleF _viewport)
            {
                if (Type == SpriteType.TEXT)
                {
                    return new MySprite()
                    {
                        Type = Type,
                        Data = Data,
                        Position = GetPosition(_viewport),
                        RotationOrScale = RotationOrScale,
                        Color = Color,
                        Alignment = Alignment,
                        FontId = FontId
                    };
                }
                return new MySprite()
                {
                    Type = Type,
                    Data = Data,
                    Position = GetPosition(_viewport),
                    RotationOrScale = RotationOrScale,
                    Color = Color,
                    Alignment = Alignment,
                    Size = Size,
                    FontId = FontId
                };
            }
            public Vector2 GetPosition()
            {
                if (_viewport == null)
                {
                    return Position;
                }
                return GetPosition(_viewport);
            }
            public Vector2 GetPosition(RectangleF _viewport)
            {
                Vector2 _position = Position + _viewport.Position;
                switch (Anchor)
                {
                    case ScreenSpriteAnchor.TopCenter:
                        _position = Position + new Vector2(_viewport.Center.X, _viewport.Y);
                        break;
                    case ScreenSpriteAnchor.TopRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Y);
                        break;
                    case ScreenSpriteAnchor.CenterLeft:
                        _position = Position + new Vector2(_viewport.X, _viewport.Center.Y);
                        break;
                    case ScreenSpriteAnchor.Center:
                        _position = Position + _viewport.Center;
                        break;
                    case ScreenSpriteAnchor.CenterRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Center.Y);
                        break;
                    case ScreenSpriteAnchor.BottomLeft:
                        _position = Position + new Vector2(_viewport.X, _viewport.Bottom);
                        break;
                    case ScreenSpriteAnchor.BottomCenter:
                        _position = Position + new Vector2(_viewport.Center.X, _viewport.Bottom);
                        break;
                    case ScreenSpriteAnchor.BottomRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Bottom);
                        break;
                }
                // apply vertical alignment based on sprite type and size
                if (VerticalAlignment == VerticalAlignments.None) return _position;
                if (Type == SpriteType.TEXT)
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignments.Center:
                            _position.Y -= SizeOnScreen.Y / 2;
                            break;
                        case VerticalAlignments.Bottom:
                            _position.Y -= SizeOnScreen.Y;
                            break;
                    }
                }
                else
                {
                    // might need to use the rotation to adjust the position?
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignments.Top:
                            _position.Y += Size.Y / 2;
                            break;
                        case VerticalAlignments.Bottom:
                            _position.Y -= Size.Y / 2;
                            break;
                    }
                }

                return _position;
            }
            public Vector2 MeasureStringInPixels(StringBuilder text)
            {
                if (_surface == null)
                {
                    return Vector2.Zero;
                }
                return _surface.MeasureStringInPixels(text, FontId, RotationOrScale);
            }
            public bool Overlaps(IScreenSprite other)
            {
                Vector2 position = GetPosition();
                Vector2 size = SizeOnScreen;
                Vector2 otherPosition = other.GetPosition();
                Vector2 otherSize = other.SizeOnScreen;
                return position.X < otherPosition.X + otherSize.X && position.X + size.X > otherPosition.X && position.Y < otherPosition.Y + otherSize.Y && position.Y + size.Y > otherPosition.Y;
            }
        }
        //----------------------------------------------------------------------
    }
}
