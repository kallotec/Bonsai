using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Level : BonsaiGameObject, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Level()
        {
            DrawOrder = 0;
        }

        IContentLoader content;
        Player player;
        Tile[,] tiles;
        Vector2 start;
        Point exit = invalidPosition;
        static readonly Point invalidPosition = new Point(-1, -1);
        Random random = new Random(354668);
        List<KeyPressListener> keyListeners;

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; private set; }
        public int DrawOrder { get; set; }
        public ICamera Camera { get; set; }

        public delegate void delExit();
        public event delExit Exit;



        public void Load(IContentLoader content)
        {
            this.content = content;

            //key listeners
            configureKeyListeners();

            // Create sample map
            var map1 = "#....................................#\n" +
                        "#....................................#\n" +
                        "#........###...............###.......#\n" +
                        "#....................................#\n" +
                        "#.1......#...........................#\n" +
                        "#....###..#..###.......###.....###...#\n" +
                        "#...........#........................#\n" +
                        "#.........##.....X..........##.......#\n" +
                        "######################################";
            loadMap(map1);

        }

        public void Unload()
        {
        }

        void loadMap(string mapFileContents)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (var sr = new StringReader(mapFileContents))
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
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    var tileType = lines[y][x];
                    tiles[x, y] = createTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == invalidPosition)
                throw new NotSupportedException("A level must have an exit.");

        }

        Tile createTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Player 1 start point
                case '1':
                    if (player != null)
                        throw new NotSupportedException("A level may only have one starting point.");

                    var centerPoint = GetBounds(x, y).Center;
                    start = new Vector2(centerPoint.X, centerPoint.Y);

                    // Create player
                    player = new Player(this, start);
                    player.Load(content);

                    Camera.SetFocus(player);

                    return new Tile(null, TileCollision.Passable, Color.Transparent);

                // Blank space
                case '.':
                    return new Tile(Globals.Pixel_HalfTransparent, TileCollision.Passable, Color.Gray);
                    
                // Exit
                case 'X':
                    if (exit != invalidPosition)
                        throw new NotSupportedException("A level may only have one exit.");

                    exit = GetBounds(x, y).Center;

                    return new Tile(Globals.Pixel, TileCollision.Passable, Color.Green);

                // Impassable block
                case '#':
                    return new Tile(Globals.Pixel, TileCollision.Impassable, Color.Gray);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }


        public void Update(GameFrame frame)
        {
            //update key press listeners
            foreach (var listener in keyListeners)
                listener.Update(frame.GameTime);

            player.Update(frame);

            ////update actors
            //for (int i = 0; i < actors.Count; i++)
            //{
            //    var actor = actors[i];

            //    //skip boundaries
            //    if (actor is GameBoundary)
            //        continue;

            //    var actorLastPosition = actor.Position;

            //    //update
            //    actor.Update(frame);

            //    //clear actors marked for deletion
            //    if (actor.MarkedForDeletion)
            //    {
            //        actors.RemoveAt(i);
            //        i--;
            //        continue;
            //    }

            //    //check collision with other collidable game objects
            //    if (!actor.IsCollidable)
            //        continue;

            //    var collided = false;

            //    for (int j = 0; j < actors.Count; j++)
            //    {
            //        var otherActor = actors[j];

            //        if (i == j || !otherActor.IsCollidable)
            //            continue;

            //        if (actor.CollisionBox.Intersects(otherActor.CollisionBox))
            //        {
            //            actor.CollidedWith(actors[j]);
            //            collided = true;
            //        }
            //    }

            //}

            ////set player position from mouse click location
            //if (frame.MouseState.LeftButton == ButtonState.Pressed)
            //    player.SetPosition(new Vector2(frame.MouseState.X, frame.MouseState.Y));

        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            //draw tile map
            for (var x = 0; x < tiles.GetLength(0); x++)
            {
                for (var y = 0; y < tiles.GetLength(1); y++)
                {
                    var tile = tiles[x, y];
                    var position = new Vector2(x, y) * new Vector2(Tile.Width, Tile.Height);

                    if (tile.Texture == null)
                        continue;

                    batch.Draw(tile.Texture, new Rectangle((int)position.X, (int)position.Y, Tile.Width, Tile.Height), tile.Tint);

                }
            }

            //draw player
            player.Draw(frame, batch);

        }


        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        void configureKeyListeners()
        {
            keyListeners = new List<KeyPressListener>();

            //[esc]
            keyListeners.Add(new KeyPressListener(Keys.Escape, new KeyPressListener.delKeyPressed(keyPressed_Esc)));

        }

        void keyPressed_Esc()
        {
            if (this.Exit != null)
                this.Exit();
        }

    }
}
