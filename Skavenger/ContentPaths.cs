using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skavenger
{
    public static class ContentPaths
    {
        // Basics
        public static string TEX_PIXEL = "pixel";
        public static string TEX_PIXEL_HALFTRANS = "pixel-halftrans";
        public static string LOGO_START_SCREEN = @"logo";

        // Fonts
        public static string FONT_UI_GENERAL = @"Fonts/ui-general";

        // Sprites
        internal static string SPRITE_PLAYER = @"Sprites/player";
        internal static string SPRITE_BULLET = @"Sprites/bullet";
        internal static string SPRITE_COIN = @"Sprites/coin";

        // Maps
        public static string PATH_MAP_1 = @"Content/Maps/office-v1.svg";

        // Sfx
        internal static string SFX_DOOR_OPEN = @"Sound/openDoor";
        internal static string SFX_DEATH = @"Sound/death";
        internal static string SFX_FIRE = @"Sound/fire";
        internal static string SFX_COIN_COLLECT = @"Sound/coinCollect";
        internal static string SFX_BOX_OPEN = @"Sound/openDoor";

    }
}
