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
        // RasterSprite - a sprite made of monospace font pixels
        //----------------------------------------------------------------------
        public class RasterSprite : ScreenSprite
        {
            public static float PIXEL_TO_SCREEN_RATIO = 28.764f; // line height of monospace font at 1f scale
            public static float DEFAULT_PIXEL_SCALE = 0.1f; // the default scale for a monospace image
            public static string INVISIBLE = ""; // (char)0xE070; // single space
            public static char IGNORE = ''; // fc00fc
            public static int MAX_ITERATIONS = 1000;

            public RasterSprite(Vector2 position, float scale, Vector2 size, string data) : base(ScreenSpriteAnchor.TopLeft, position, scale, size, Color.White, "Monospace", data, TextAlignment.LEFT, SpriteType.TEXT)
            {
                if (size == Vector2.Zero)
                {
                    string[] lines = data.Split('\n');
                    Size = new Vector2(lines[0].Length, lines.Length);
                }
                // decompress the transparent data
                Data = Data.Replace("", INVISIBLE + INVISIBLE); // double space
                Data = Data.Replace("", INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE); // quad space
                Data = Data.Replace("", INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE); // 8 space
            }
            public string Image 
            { 
                get { return Data; } 
                set 
                { 
                    Data = value;
                    string[] lines = value.Split('\n');
                    Size = new Vector2(lines[0].Length, lines.Length);
                    // decompress the transparent data
                    Data = Data.Replace("", INVISIBLE + INVISIBLE); // double space
                    Data = Data.Replace("", INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE); // quad space
                    Data = Data.Replace("", INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE + INVISIBLE); // 8 space
                }
            }
            public Vector2 PixelToScreen(Vector2 pixel)
            {
                return pixel * PIXEL_TO_SCREEN_RATIO * RotationOrScale;
            }
            public Vector2 PixelPosToScreenPos(Vector2 pixel)
            {
                return pixel * PIXEL_TO_SCREEN_RATIO * RotationOrScale + Position;
            }
            public Vector2 ScreenToPixel(Vector2 screen)
            {
                Vector2 res = screen / (PIXEL_TO_SCREEN_RATIO * RotationOrScale);
                return new Vector2((int)res.X, (int)res.Y);
            }
            public Vector2 ScreenPosToPixelPos(Vector2 screen)
            {
                Vector2 res = (screen - Position) / (PIXEL_TO_SCREEN_RATIO * RotationOrScale);
                return new Vector2((int)res.X, (int)res.Y);
            }
            // bytes between 0 - 7
            // usage: PixelIcon.rgb(0, 0, 7);
            public static char rgb(byte r, byte g, byte b)
            {
                return (char)(0xE100 + (r << 6) + (g << 3) + b);
            }
            public static char rgb(int r, int g, int b)
            {
                return rgb((byte)r, (byte)g, (byte)b);
            }
            public static char rgb(Color color)
            {
                return rgb(color.R, color.G, color.B);
            }
            public static Color CharToColor(char c)
            {
                int i = (int)c - 0xE100;
                int r = (i >> 6) & 7;
                int g = (i >> 3) & 7;
                int b = i & 7;
                return new Color(r * 32, g * 32, b * 32);
            }
            // remap an int from 0-255 to a byte from 0-7
            public static byte remap(int value)
            {
                if (value < 0) return 0;
                if (value > 255) return 7;
                return (byte)(value / 32);
            }
            //
            // draw functions
            //
            // add a pixel to the icon
            // ints between 0 - 255
            Vector2 _addPosition = Vector2.Zero;
            public void addPixelRGB(int r, int g, int b)
            {
                if (_addPosition.Y >= Size.Y) return;
                Data += rgb(remap(r), remap(g), remap(b));
                _addPosition.X += 1f;
                if (_addPosition.X >= Size.X)
                {
                    _addPosition.X = 0f;
                    _addPosition.Y += 1f;
                    Data += "\n";
                }
            }
            // fill the icon with a color
            // ints between 0 - 255
            public void fillRGB(int r, int g, int b)
            {
                Data = "";
                for (int x = 0; x < Size.X; x++)
                {
                    addPixelRGB(r, g, b);
                }
                string line = Data;
                for (int y = 1; y < Size.Y; y++)
                {
                    Data += line;
                }
            }
            // fill half the icon with a color
            // ints between 0 - 255
            public void fillHalfRGB(int r, int g, int b)
            {
                string cache = Data;
                Data = "";
                for (int x = 0; x < Size.X; x++)
                {
                    addPixelRGB(r, g, b);
                }
                string line = Data;
                Data = cache;
                for (int y = 0; y < Size.Y / 2; y++)
                {
                    Data += line;
                }
            }
            public void fillHalfRGB(Color color)
            {
                fillHalfRGB(color.R, color.G, color.B);
            }
            public void fillRGB(Color color)
            {
                fillRGB(color.R, color.G, color.B);
            }
            // set a pixel to a color at a position
            // ints between 0 - 255
            public void setPixelRGB(int x, int y, int r, int g, int b)
            {
                if (x < 0 || x >= Size.X || y < 0 || y >= Size.Y) return;
                Data = Data.Remove((int)(y * (Size.X + 1) + x), 1);
                Data = Data.Insert((int)(y * (Size.X + 1) + x), rgb(remap(r), remap(g), remap(b)).ToString());
            }
            // get a box of pixels
            public string getPixels(int x, int y, int width, int height)
            {
                // make sure the box is within the Size range
                if (x < 0 || x >= Size.X || y < 0 || y >= Size.Y) return "error";
                string pixels = "";
                for (int y1 = y; y1 < y + height; y1++)
                {
                    pixels += Data.Substring((int)(y1 * (Size.X + 1) + x), width);
                    /*
                    for (int x1 = x; x1 < x + width; x1++)
                    {
                        pixels += Data[(int)(y1 * (Size.X + 1) + x1)];
                    }
                    */
                    pixels += "\n";
                }
                return pixels;
            }
            // draw a box of pixels over the image
            public bool drawPixels(int x, int y, string pixels)
            {
                string[] lines = pixels.Split('\n');
                int width = lines[0].Length;
                int height = lines.Length;
                // make sure the box is within the Size range
                if (x < 0 || x + width >= Size.X || y < 0 /*|| y + height >= Size.Y*/) return false;
                int i = 0;
                for (int y1 = y; y1 < y + height && y1 < Size.Y; y1++)
                {
                    int index = (int)(y1 * (Size.X + 1) + x);
                    if (index + width < Data.Length && index > 0)
                    {
                        Data = Data.Remove(index, width);
                        Data = Data.Insert(index, lines[i]);
                    }
                    i++;
                }
                return true;
            }
            // draw a box of pixels over the image
            public bool drawPixelColumnWithIgnore(int x, int y, string pixels)
            {
                string[] lines = pixels.Split('\n');
                int width = lines[0].Length;
                int height = lines.Length;
                // make sure the box is within the Size range
                if (x < 0 || x + width >= Size.X || y < 0 /*|| y + height >= Size.Y*/) return false;
                int i = 0;
                for (int y1 = y; y1 < y + height && y1 < Size.Y; y1++)
                {
                    int index = (int)(y1 * (Size.X + 1) + x);
                    if (index + width < Data.Length && index > 0 && lines[i][0] != IGNORE)
                    {
                        Data = Data.Remove(index, width);
                        Data = Data.Insert(index, lines[i]);
                    }
                    i++;
                }
                return true;
            }
            // draw a line from x1,y1 to x2,y2
            // rgb ints between 0 - 255
            public void drawLineRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = (x1 < x2) ? 1 : -1;
                int sy = (y1 < y2) ? 1 : -1;
                int err = dx - dy;
                int i = 0;
                while (true && i++ < MAX_ITERATIONS)
                {
                    setPixelRGB(x1, y1, r, g, b);
                    if ((x1 == x2) && (y1 == y2)) break;
                    int e2 = 2 * err;
                    if (e2 > -dy)
                    {
                        err -= dy;
                        x1 += sx;
                    }
                    if (e2 < dx)
                    {
                        err += dx;
                        y1 += sy;
                    }
                }
            }
            public void drawLineRGB(Vector2 start, Vector2 end, Color color)
            {
                drawLineRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // draw a rectangle from x1,y1 to x2,y2
            // ints between 0 - 255
            public void drawRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                drawLineRGB(x1, y1, x2, y1, r, g, b);
                drawLineRGB(x2, y1, x2, y2, r, g, b);
                drawLineRGB(x2, y2, x1, y2, r, g, b);
                drawLineRGB(x1, y2, x1, y1, r, g, b);
            }
            public void drawRectRGB(Vector2 start, Vector2 end, Color color)
            {
                drawRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // fill a rectangle from x1,y1 to x2,y2
            // ints between 0 - 255
            public void fillRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                for (int y = y1; y <= y2; y++)
                {
                    drawLineRGB(x1, y, x2, y, r, g, b);
                }
            }
            public void fillRectRGB(Vector2 start, Vector2 end, Color color)
            {
                fillRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // draw a circle at x,y with radius r
            // ints between 0 - 255
            public void drawCircleRGB(int x, int y, int r, int red, int green, int blue)
            {
                int f = 1 - r;
                int ddF_x = 1;
                int ddF_y = -2 * r;
                int x1 = 0;
                int y1 = r;
                setPixelRGB(x, y + r, red, green, blue);
                setPixelRGB(x, y - r, red, green, blue);
                setPixelRGB(x + r, y, red, green, blue);
                setPixelRGB(x - r, y, red, green, blue);
                while (x1 < y1)
                {
                    if (f >= 0)
                    {
                        y1--;
                        ddF_y += 2;
                        f += ddF_y;
                    }
                    x1++;
                    ddF_x += 2;
                    f += ddF_x;
                    setPixelRGB(x + x1, y + y1, red, green, blue);
                    setPixelRGB(x - x1, y + y1, red, green, blue);
                    setPixelRGB(x + x1, y - y1, red, green, blue);
                    setPixelRGB(x - x1, y - y1, red, green, blue);
                    setPixelRGB(x + y1, y + x1, red, green, blue);
                    setPixelRGB(x - y1, y + x1, red, green, blue);
                    setPixelRGB(x + y1, y - x1, red, green, blue);
                    setPixelRGB(x - y1, y - x1, red, green, blue);
                }
            }
            // fill a circle at x,y with radius r
            // ints between 0 - 255
            public void fillCircleRGB(int x, int y, int r, int red, int green, int blue)
            {
                for (int y1 = -r; y1 <= r; y1++)
                    for (int x1 = -r; x1 <= r; x1++)
                        if (x1 * x1 + y1 * y1 <= r * r)
                            setPixelRGB(x + x1, y + y1, red, green, blue);
            }
            // intesect with another raster sprite that might not have the same position in screen space
            // and turn the intersecting pixels red
            public bool Intersect(RasterSprite sprite, bool highlight = false, bool delete = false)
            {
                Vector2 pixelPos = ScreenPosToPixelPos(sprite.Position);
                if (pixelPos.X + sprite.Size.X < 0 || pixelPos.X >= Size.X || pixelPos.Y + sprite.Size.Y < 0 || pixelPos.Y >= Size.Y) return false;
                //GridInfo.Echo("Intersect: " + pixelPos.ToString() + " | " + sprite.Size.ToString());
                // there's overlap in the sprites. do any pixels overlap?
                int x1 = (int)Math.Max(0, pixelPos.X);
                int y1 = (int)Math.Max(0, pixelPos.Y);
                int x2 = (int)Math.Min(Size.X, pixelPos.X + sprite.Size.X);
                int y2 = (int)Math.Min(Size.Y, pixelPos.Y + sprite.Size.Y);
                bool intersect = false;
                string SourceColider = Data.Replace(INVISIBLE, IGNORE.ToString());
                SourceColider = SourceColider.Replace("\r\n", "\n");
                if (highlight || delete) Data = SourceColider;
                string TargetColider = sprite.Data.Replace(INVISIBLE, IGNORE.ToString());
                TargetColider = TargetColider.Replace("\r\n", "\n");
                for (int y = y1; y < y2; y++)
                {
                    for (int x = x1; x < x2; x++)
                    {
                        int sourceIndex = (int)(y * (Size.X + 1) + x);
                        int targetIndex = (int)((y - (int)pixelPos.Y) * ((int)sprite.Size.X + 1) + (x - (int)pixelPos.X));
                        //int sourceIndex = (int)(y * (Size.X + 1) + x);
                        //int targetIndex = (int)((y - (int)pixelPos.Y) * ((int)sprite.Size.X + 1) + (x - (int)pixelPos.X));

                        //GridInfo.Echo("\nlocal pos: "+x.ToString()+","+y.ToString() +" | "+ sourceIndex +"/"+SourceColider.Length + " == " + (SourceColider[sourceIndex] != IGNORE));
                        //GridInfo.Echo("sprite pos: " + (x - (int)pixelPos.X).ToString() + "," + (y - (int)pixelPos.Y).ToString() + " | " + targetIndex +"/"+TargetColider.Length+ " == "+(TargetColider[targetIndex] != IGNORE));
                        //if (sourceIndex < SourceColider.Length && SourceColider[sourceIndex] != IGNORE && SourceColider[sourceIndex] != '\n') setPixelRGB(x, y, 0, 255, 0);

                        // check if the pixel is not transparent in both sprites and if not, turn it red
                        if (sourceIndex < SourceColider.Length && targetIndex < TargetColider.Length && SourceColider[sourceIndex] != IGNORE && TargetColider[targetIndex] != IGNORE)
                        {
                            intersect = true;
                            if (delete) { Data = Data.Remove(sourceIndex, 1); Data = Data.Insert(sourceIndex, IGNORE.ToString()); }
                            else if (highlight) setPixelRGB(x, y, 255, 0, 0);
                            else return true;
                        }
                    }
                }
                if (highlight || delete) Data = Data.Replace(IGNORE.ToString(), INVISIBLE);
                return intersect;
            }
            //flip the sprite horizontally
            public void FlipHorizontal()
            {
                string[] lines = Data.Split('\n');
                string newData = "";
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = lines[y].Length - 1; x >= 0; x--)
                    {
                        newData += lines[y][x];
                    }
                    newData += "\n";
                }
                Data = newData;
            }
            //flip the sprite vertically
            public void FlipVertical()
            {
                string[] lines = Data.Split('\n');
                string newData = "";
                for (int y = lines.Length - 1; y >= 0; y--)
                {
                    newData += lines[y];
                    newData += "\n";
                }
                Data = newData;
            }
            //rotate the sprite 90 degrees clockwise
            public void Rotate90()
            {
                string[] lines = Data.Split('\n');
                string newData = "";
                if (Data.Length > MAX_ITERATIONS * 10)
                {
                    GridInfo.Echo("WARNING::Rotate90: Data too long: " + Data.Length.ToString());
                }
                for (int x = 0; x < lines[0].Length; x++)
                {
                    for (int y = lines.Length - 1; y >= 0; y--)
                    {
                        newData += lines[y][x];
                    }
                    newData += "\n";
                }
                Data = newData;
            }
            //rotate the sprite 90 degrees counter clockwise
            public void Rotate270()
            {
                string[] lines = Data.Split('\n');
                string newData = "";
                for (int x = lines[0].Length - 1; x >= 0; x--)
                {
                    for (int y = 0; y < lines.Length; y++)
                    {
                        newData += lines[y][x];
                    }
                    newData += "\n";
                }
                Data = newData;
            }
            //rotate the sprite 180 degrees
            public void Rotate180()
            {
                string[] lines = Data.Split('\n');
                string newData = "";
                for (int y = lines.Length - 1; y >= 0; y--)
                {
                    for (int x = lines[y].Length - 1; x >= 0; x--)
                    {
                        newData += lines[y][x];
                    }
                    newData += "\n";
                }
                Data = newData;
            }
        }
        //----------------------------------------------------------------------
    }
}
