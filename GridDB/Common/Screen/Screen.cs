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
        // Screen - encapsulates a drawing surface
        //----------------------------------------------------------------------
        public class Screen
        {
            public static Color DEFAULT_TEXT_COLOR = new Color(179, 237, 255);
            public static Color DEFAULT_BACKGROUND_COLOR = new Color(0, 88, 151);
            private IMyTextSurface _drawingSurface;
            public IMyTextSurface DrawingSurface
            {
                get { return _drawingSurface; }
            }
            private RectangleF _viewport;
            private List<List<ScreenSprite>> _sprites = new List<List<ScreenSprite>>();
            public float bottomPadding = 0f;
            public float topPadding = 0f;
            public float leftPadding = 0f;
            public float rightPadding = 0f;
            public bool hasColorfulIcons
            {
                get
                {
                    List<string> sprites = new List<string>();
                    _drawingSurface.GetSprites(sprites);
                    return sprites.Contains("ColorfulIcons_Ore/Iron");
                }
            }
            public Color BackgroundColor
            {
                get { return _drawingSurface.ScriptBackgroundColor; }
                set { _drawingSurface.ScriptBackgroundColor = value; }
            }
            public Color ForegroundColor
            {
                get { return _drawingSurface.ScriptForegroundColor; }
                set { _drawingSurface.ScriptForegroundColor = value; }
            }
            public Vector2 Size
            {
                get { return _drawingSurface.SurfaceSize; }
            }
            public RectangleF Viewport
            {
                get { return _viewport; }
            }
            //
            // constructor
            //
            public Screen(IMyTextSurface drawingSurface)
            {
                _drawingSurface = drawingSurface;
                _drawingSurface.ContentType = ContentType.SCRIPT;
                _drawingSurface.Script = "";
                // calculate the viewport offset by centering the surface size onto the texture size
                _viewport = new RectangleF((_drawingSurface.TextureSize - _drawingSurface.SurfaceSize) / 2f, _drawingSurface.SurfaceSize);
            }
            public virtual void Update(string argument)
            {
                //place holder for derived classes
            }
            public virtual void Main(string argument)
            {
                Update(argument);
                Draw();
            }
            //
            // DrawSprites - draw sprites to the screen
            //
            public virtual void Draw()
            {
                //GridInfo.Echo("Screen: Draw");
                var frame = _drawingSurface.DrawFrame();
                DrawSprites(ref frame);
                frame.Dispose();
            }
            //
            // DrawSprites - draw sprites to the screen
            //
            private void DrawSprites(ref MySpriteDrawFrame frame)
            {
                // draw all the sprites
                foreach (List<ScreenSprite> spriteList in _sprites)
                {

                    foreach (ScreenSprite sprite in spriteList)
                    {
                        if (sprite != null && sprite.Visible) frame.Add(sprite.ToMySprite(_viewport));
                    }
                }
            }
            public void AddSprite(ScreenSprite sprite, int layer = 0)
            {
                while (layer >= _sprites.Count)
                {
                    _sprites.Add(new List<ScreenSprite>());
                }
                if (_sprites[layer].Contains(sprite)) return;
                _sprites[layer].Add(sprite);
                sprite.SetViewport(_viewport, _drawingSurface);
            }
            public void AddSprite(IScreenSpriteProvider spriteProvider, int layer = 0)
            {
                spriteProvider.AddToScreen(this, layer);
            }
            public void RemoveSprite(ScreenSprite sprite)
            {
                foreach (List<ScreenSprite> spriteList in _sprites)
                {
                    if (spriteList.Remove(sprite)) break;
                }
            }
            public void RemoveSprite(IScreenSpriteProvider spriteProvider)
            {
                spriteProvider.RemoveFromScreen(this);
            }
        }
        //----------------------------------------------------------------------
    }
}