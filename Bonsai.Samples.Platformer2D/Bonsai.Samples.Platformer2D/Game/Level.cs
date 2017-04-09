using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.UI.Widgets.Popups;
using Bonsai.Samples.Platformer.Components;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Level : DrawableBase, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Level()
        {
            base.DrawOrder = 0;
            popupManager = new PopupManager();
            keyListeners = new List<KeyPressListener>();
            phys = new MapPhysics(this);

            coins = new List<Coin>();
        }

        Player player;
        PopupManager popupManager;
        List<KeyPressListener> keyListeners;
        SpriteFont font;
        Texture2D pixel_half_trans;
        Texture2D pixel;
        MapPhysics phys;
        List<Coin> coins;
        Door door;
        Vector2? playerStart;
        Vector2? playerExit;

        public GameVariable<int> Jumps;
        public GameVariable<int> CoinsCount;
        public bool IsDisabled { get; set; }
        public ICamera Camera { get; set; }
        public Map Map { get; private set; }
        public float Gravity;
        public float TerminalVelocity;
        public bool HasGravity => Gravity != 0;


        public void Load(IContentLoader loader)
        {
            // Physics
            Gravity = 5f;
            TerminalVelocity = 200f;

            // Content
            font = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);
            pixel = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            pixel_half_trans = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL_HALFTRANS);

            // Create tile map
            Map = new Map(tileWidth: 22, tileHeight: 22);
            // Create map tiles
            var tileGrid = generateTileGrid(loader);
            Map.Tiles = tileGrid;

            // Verify that the level has a beginning and an end.
            if (playerStart == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (playerExit == null)
                throw new NotSupportedException("A level must have an exit.");

            // Create player
            player = new Player(this, playerStart.Value);
            player.Load(loader);

            // Focus camera on player
            Camera.SetFocus(player);

            // Setup game variables
            Jumps = new GameVariable<int>();
            CoinsCount = new GameVariable<int>();

            // Key listeners
            keyListeners = new List<KeyPressListener>
            {
                // [M] key generates a text popup at the players position
                new KeyPressListener(Keys.M, () =>
                {
                    popupManager.AddMessage(
                        "Hey!",
                        new PopupSettings
                        {
                            LifeInMillisecs = 1000,
                            Velocity = new Vector2(0, -10),
                            Font = font,
                            Position = player.Props.Position + new Vector2(0,-20)
                        });
                })
            };

        }

        Tile[,] generateTileGrid(IContentLoader loader)
        {
            // Read map data
            var mapData = string.Empty;
            using (var stream = TitleContainer.OpenStream(ContentPaths.PATH_MAP_1))
            using (var rdr = new StreamReader(stream))
                mapData = rdr.ReadToEnd();

            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (var sr = new StringReader(mapData))
            {
                string line = sr.ReadLine();
                line = line.Trim();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(string.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = sr.ReadLine();
                }
            }

            var tileGrid = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < tileGrid.GetLength(1); ++y)
            {
                for (var x = 0; x < tileGrid.GetLength(0); ++x)
                {
                    // to load each tile.
                    var tileType = lines[y][x];
                    tileGrid[x, y] = createTile(tileType, x, y, loader);
                }
            }

            return tileGrid;
        }

        Tile createTile(char tileType, int x, int y, IContentLoader loader)
        {
            var tileRect = new Rectangle(x * Map.TileSize.X, y * Map.TileSize.Y, Map.TileSize.X, Map.TileSize.Y);
            var tileCenter = new Vector2(tileRect.Center.X, tileRect.Center.Y);

            switch (tileType)
            {
                // Player 1 start point
                case '1':
                    if (playerStart != null)
                        throw new NotSupportedException("A level may only have one starting point.");

                    playerStart = tileCenter;

                    return new Tile(null, TileCollision.Passable, Color.Transparent);

                // Blank space
                case '.':
                    return new Tile(pixel_half_trans, TileCollision.Passable, Color.Gray);

                case 'c':
                    addCoinToLevel(loader, tileCenter);
                    return new Tile(pixel_half_trans, TileCollision.Passable, Color.Gray);

                // Exit
                case 'X':
                    if (playerExit != null)
                        throw new NotSupportedException("A level may only have one exit.");

                    playerExit = tileCenter;

                    addDoorToLevel(loader, playerExit.Value);

                    return new Tile(pixel, TileCollision.Passable, Color.Gray);

                // Impassable block
                case '#':
                    return new Tile(pixel, TileCollision.Impassable, Color.Gray);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position [{1},{2}].", tileType, x, y));
            }
        }

        void addCoinToLevel(IContentLoader loader, Vector2 position)
        {
            var coin = new Coin();
            coin.Load(loader);
            coin.Props.Position = position;
            coins.Add(coin);
        }

        void addDoorToLevel(IContentLoader loader, Vector2 position)
        {
            // Create door
            door = new Door();
            position.X -= door.CollisionBox.Center.X;
            position.Y -= door.CollisionBox.Center.Y;
            door.Props.Position = position;
            door.Load(loader);
        }

        public void Unload()
        {
            Jumps.Unload();
            popupManager.Clear();
        }


        public void Update(GameTime time)
        {
            // Misc
            foreach (var listener in keyListeners)
                listener.Update(time);

            // Game objects
            updatePlayer(time);
            updateCoins();
            updateDoor();

            // UI
            popupManager.Update(time);
        }

        void updatePlayer(GameTime time)
        {
            player.Update(time);

            // Map collisions
            var wallCollisions = phys.ApplyPhysics(player.Props, time);

            player.Props.Grounded = (wallCollisions.Contains(MapPhysics.CollisionDirection.Bottom) == true);
        }

        void updateCoins()
        {
            // Actor -> coins collisions
            foreach (var coin in coins.Where(c => c.IsCollisionEnabled))
            {
                var overlapping = coin.CollisionBox.Intersects(player.CollisionBox);
                if (overlapping)
                {
                    player.Overlapping(coin);
                    coin.Overlapping(player);

                    // Increment count
                    CoinsCount.Value += 1;
                }
            }

        }

        void updateDoor()
        {
            if (!door.IsCollisionEnabled)
                return;

            // Collision
            var overlapping = door.CollisionBox.Intersects(player.CollisionBox);
            if (overlapping)
            {
                door.Overlapping(player);

                resetLevel();
            }

        }

        void resetLevel()
        {
            Jumps.Value = 0;
            CoinsCount.Value = 0;

            player.Props.Position = playerStart.Value;

            // Reset coins
            foreach (var coin in coins)
            {
                coin.IsCollisionEnabled = true;
                coin.IsHidden = false;
            }

            door.IsCollisionEnabled = true;

        }


        public void Draw(GameTime time, SpriteBatch batch)
        {
            Map.Draw(time, batch);

            foreach (var coin in coins.Where(c => !c.IsHidden))
                coin.Draw(time, batch);

            door.Draw(time, batch);

            player.Draw(time, batch);

            popupManager.Draw(time, batch);

        }

    }
}
