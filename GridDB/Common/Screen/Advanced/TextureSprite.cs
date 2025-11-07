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
        // TextureSprite - a sprite made of a texture
        //----------------------------------------------------------------------
        public class TextureSprite : ScreenSprite
        {
            public static string SQUARE_SIMPLE = "SquareSimple";
            public TextureSprite(Vector2 position, Vector2 size, string texture, Color color, float rotation = 0, TextAlignment alignment = TextAlignment.LEFT, VerticalAlignments verticalAlignment = VerticalAlignments.Top, ScreenSpriteAnchor anchor = ScreenSpriteAnchor.TopLeft, bool visible = true)
            {
                Anchor = anchor;
                Position = position;
                Size = size;
                Data = texture;
                Color = color;
                RotationOrScale = rotation;
                Visible = visible;
                Type = SpriteType.TEXTURE;
                Alignment = alignment;
                VerticalAlignment = verticalAlignment;
            }
            public override void SetViewport(RectangleF viewport, IMyTextSurface surface)
            {
                base.SetViewport(viewport, surface);
                List<string> sprites = new List<string>();
                surface.GetSprites(sprites);
                if (!sprites.Contains(Data)) throw new Exception("Texture not found: " + Data);
            }
        }
        //----------------------------------------------------------------------
    }
}
