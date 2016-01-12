using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    struct Tile
    {
        public Tile(Texture2D texture, TileCollision collision, Color tint)
        {
            Texture = texture;
            Collision = collision;
            Tint = tint;
        }

        public Texture2D Texture;
        public TileCollision Collision;
        public Color Tint;

        public const int Width = 40;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

    }
}
