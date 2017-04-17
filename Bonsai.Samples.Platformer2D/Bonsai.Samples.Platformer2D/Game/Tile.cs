using Bonsai.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public struct Tile
    {
        public Tile(Texture2D texture, TileCollision collision, Color tint)
        {
            Texture = texture;
            Collision = collision;
            Tint = tint;

            Entities = new List<ICollidable>();
        }

        public Texture2D Texture;
        public TileCollision Collision;
        public Color Tint;

        public List<ICollidable> Entities;

    }
}
