using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D
{
    public static class ContentPaths
    {
        // Basics
        public static string TEX_PIXEL = "pixel";
        public static string TEX_PIXEL_HALFTRANS = "pixel-halftrans";

        // Fonts
        public static string FONT_UI_GENERAL = @"Fonts/ui-general";

        // Sprites
        public static string SPRITE_MARIO = @"GameObjects/mario";
        public static string SPRITE_COIN = @"GameObjects/coin";
        internal static string SPRITE_DOOR = @"GameObjects/door";

        // Maps
        public static string PATH_MAP_1 = @"Content/map1.txt";
        public static string PATH_MAP_2 = @"Content/map2.txt";

        // Sfx
        public static string SFX_COIN_COLLECT = @"Sound/coinCollect";
        internal static string SFX_DOOR_OPEN = @"Sound/openDoor";
    }
}
