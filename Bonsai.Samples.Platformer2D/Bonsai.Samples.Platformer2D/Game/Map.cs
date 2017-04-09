using Bonsai.Framework;
using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Map : Bonsai.Framework.IDrawable
    {
        public Map(int tileWidth, int tileHeight)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        int tileWidth;
        int tileHeight;

        public Tile[,] Tiles { get; set; }
        public Point TileSize => new Point(tileWidth, tileHeight);
        public int TilesWide => Tiles.GetLength(0);
        public int TilesHigh => Tiles.GetLength(1);
        public bool IsHidden { get; set; }
        public int DrawOrder => -1;
        public bool IsAttachedToCamera => false;
        public bool IsDisabled => false;


        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Tiles
            for (var x = 0; x < Tiles.GetLength(0); x++)
            {
                for (var y = 0; y < Tiles.GetLength(1); y++)
                {
                    var tile = Tiles[x, y];
                    var position = new Vector2(x, y) * new Vector2(TileSize.X, TileSize.Y);

                    if (tile.Texture == null)
                        continue;

                    batch.Draw(tile.Texture, new Rectangle((int)position.X, (int)position.Y, TileSize.X, TileSize.Y), tile.Tint);
                }
            }

        }


        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= TilesWide)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= TilesHigh)
                return TileCollision.Passable;

            return Tiles[x, y].Collision;
        }

        public TileEdges GetEdges(Vector2 position, int objWidth, int objHeight)
        {
            return new TileEdges(position, objWidth, objHeight, tileWidth, tileHeight);
        }


        public struct TileEdges
        {
            public TileEdges(Vector2 position, int objWidth, int objHeight, int tileWidth, int tileHeight)
            {
                this.position = position;
                this.objWidth = objWidth;
                this.objHeight = objHeight;
                this.tileWidth = tileWidth;
                this.tileHeight = tileHeight;
            }

            Vector2 position;
            int objWidth;
            int objHeight;
            int tileWidth;
            int tileHeight;

            public int LeftIndex { get { return (int)Math.Round(position.X) / tileWidth; } }
            public int RightIndex { get { return (int)(Math.Round(position.X) + objWidth) / tileWidth; } }
            public int TopIndex { get { return (int)Math.Round(position.Y) / tileHeight; } }
            public int BottomIndex { get { return (int)(Math.Round(position.Y) + objHeight) / tileHeight; } }

        }
    }
}
