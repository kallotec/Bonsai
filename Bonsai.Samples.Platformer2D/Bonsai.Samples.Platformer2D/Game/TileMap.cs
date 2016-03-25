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
    public class TileMap
    {
        public TileMap(int tileWidth, int tileHeight)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }
        
        int tileWidth;
        int tileHeight;
        public Tile[,] Tiles { get; private set; }
        public Vector2? Start { get; private set; }
        public Vector2? Exit { get; private set; }
        Texture2D pixel_half_trans;
        Texture2D pixel;

        public Point TileSize
        {
            get { return new Point(tileWidth, tileHeight); }
        }

        public int TilesWide
        {
            get { return Tiles.GetLength(0); }
        }

        public int TilesHigh
        {
            get { return Tiles.GetLength(1); }
        }


        public void LoadContent(IContentLoader loader, string mapFileData)
        {
            // Textures

            pixel = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            pixel_half_trans = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL_HALFTRANS);

            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (var sr = new StringReader(mapFileData))
            {
                string line = sr.ReadLine();
                line = line.Trim();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = sr.ReadLine();
                }
            }

            // Allocate the tile grid.
            Tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < TilesHigh; ++y)
            {
                for (var x = 0; x < TilesWide; ++x)
                {
                    // to load each tile.
                    var tileType = lines[y][x];
                    Tiles[x, y] = createTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Start == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (Exit == null)
                throw new NotSupportedException("A level must have an exit.");

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

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        Tile createTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Player 1 start point
                case '1':
                    if (Start != null)
                        throw new NotSupportedException("A level may only have one starting point.");

                    var startPoint = GetBounds(x, y).Center;
                    Start = new Vector2(startPoint.X, startPoint.Y);

                    return new Tile(null, TileCollision.Passable, Color.Transparent);

                // Blank space
                case '.':
                    return new Tile(pixel_half_trans, TileCollision.Passable, Color.Gray);

                // Exit
                case 'X':
                    if (Exit != null)
                        throw new NotSupportedException("A level may only have one exit.");

                    var exitPoint = GetBounds(x, y).Center;
                    Exit = new Vector2(exitPoint.X, exitPoint.Y);

                    return new Tile(pixel, TileCollision.Passable, Color.Green);

                // Impassable block
                case '#':
                    return new Tile(pixel, TileCollision.Impassable, Color.Gray);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position [{1},{2}].", tileType, x, y));
            }
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

            public int LeftIndex { get { return (int)position.X / tileWidth; } }
            public int RightIndex { get { return (int)(position.X + objWidth) / tileWidth; } }
            public int TopIndex { get { return (int)position.Y / tileHeight; } }
            public int BottomIndex { get { return (int)(position.Y + objHeight) / tileHeight; } }

        }
    }
}
