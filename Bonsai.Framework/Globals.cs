using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Bonsai.Framework.Content;

namespace Bonsai.Framework
{
    public class Globals
    {
        public static GraphicsDevice Device;
        public static IContentLoader Content;
        public static ICamera Camera;
        public static Rectangle Viewport;

        public static Vector2 Viewport_Centerpoint;
        public static Vector2 Window_Position;

        public static bool UI_DisplayFPS = true;
        public static bool UI_ShowPointsInfo = true;
        public static bool UI_ShowWaveInfo = true;
        public static bool UI_ShowZombieDispatchInfo = true;

        public static string FONT_MENUREGULAR = @"Fonts\menu_regular";
        public static string FONT_MENUHEADER = @"Fonts\menu_header";
        public static string FONT_MAPTEXT = @"Fonts\map_text";

        public static string TEX_PLAYER = @"Textures\player";
        public static string TEX_WEAPON_BALL = @"Textures\Weapons\ball";
        public static string TEX_WEAPON_SEMIAUTO = @"Textures\Weapons\mini14";
        public static string TEX_ZOMBIE = @"Textures\zombie";
        public static string TEX_MAP = @"Textures\map_bg";
        public static string TEX_PLANTAR = @"Textures\Obsticles\plantar";
        public static string TEX_MAINMENU_BG = @"Textures\mainmenu_bg";

        public static bool DrawBoundingBoxes = false;
        public static Color DrawBoundingBoxesColor = Color.Red;
        public static bool DrawCameraBox = false;
        public static Color DrawCameraBoxColor = Color.Gray;
        public static bool IsPlayerTakingInput = true;
        public static Vector2 Up = new Vector2(1,0);

        static Texture2D pixel;
        static Texture2D pixel_halftransparent;
        public static Texture2D Pixel
        {
            get
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(Globals.Device, 1, 1, false, SurfaceFormat.Color);
                    Color[] c = new Color[1 * 1];
                    c[0] = Color.White;
                    pixel.SetData<Color>(c);
                }

                return pixel;
            }
        }
        public static Texture2D Pixel_HalfTransparent
        {
            get
            {
                if (pixel_halftransparent == null)
                {
                    pixel_halftransparent = new Texture2D(Globals.Device, 1, 1, false, SurfaceFormat.Color);
                    Color[] c = new Color[1];
                    c[0] = Color.FromNonPremultiplied(255, 255, 255, 100);
                    pixel_halftransparent.SetData<Color>(c);
                }

                return pixel_halftransparent;
            }
        }

    }
}
