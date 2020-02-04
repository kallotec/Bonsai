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
    public class Level : Screen
    {
        public Level(BonsaiGame game) : base(game)
        {
            popupManager = new PopupManager();
            keyListeners = new List<KeyPressListener>();

            coins = new List<Coin>();
        }

        Player player;
        HUD hud;
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
        IContentLoader _loader;
        string currentMapPath;

        public event EventHandler Exit;
        public event EventHandler GameOver;

        public GameVariable<int> Jumps;
        public GameVariable<int> CoinsCount;
        public bool IsDisabled { get; set; }
        public Map Map { get; private set; }
        /// <summary>
        /// Intensity of gravity affecting player while not grounded
        /// </summary>
        public float Gravity;
        /// <summary>
        /// 0f to 1f to represent intensity of friction when entity grounded
        /// </summary>
        public float Friction;
        public float TerminalVelocity;
        public bool HasGravity => Gravity != 0;
        public bool HasFriction => Friction != 0;


        public override void Load(IContentLoader loader)
        {
            _loader = loader;

            // Content
            font = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);
            pixel = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            pixel_half_trans = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL_HALFTRANS);

            // Map
            Map = new Map(tileWidth: 22, tileHeight: 22);
            Map.DrawOrder = -1;
            GameObjects.Add(Map);

            // Player
            player = new Player(this);
            player.DrawOrder = 0;
            player.Load(loader);
            GameObjects.Add(player);

            // Game variables
            Jumps = new GameVariable<int>();
            Jumps.Load(loader);
            GameObjects.Add(Jumps);
            CoinsCount = new GameVariable<int>();
            CoinsCount.Load(loader);
            GameObjects.Add(CoinsCount);

            // HUD
            hud = new HUD(this);
            hud.ScreenBounds = base.Game.GraphicsDevice.Viewport.Bounds;
            hud.Exit += (s, e) => { Exit?.Invoke(s, e); };
            hud.DrawOrder = 1;
            hud.Load(loader);
            GameObjects.Add(hud);

            // Physics
            phys = new MapPhysics(this);
            Gravity = 5f;
            Friction = 0.1f;
            TerminalVelocity = 200f;

            // Camera
            Camera.SetFocus(player);
            GameObjects.Add(Camera);

            // Services
            setupKeyListeners();

            // Load first map
            loadMap(ContentPaths.PATH_MAP_1, loader);
        }

        void setupKeyListeners()
        {
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
                }),
                // [ESC] Exit
                new KeyPressListener(Keys.Escape, () => Exit?.Invoke(this,null))
            };
        }

        void loadMap(string mapPath, IContentLoader loader)
        {
            currentMapPath = mapPath;

            // Variables
            Jumps.Value = 0;
            CoinsCount.Value = 0;

            // Reset game objects
            var existingCoinGameObjects = GameObjects.Where(g => g is Coin);
            GameObjects.RemoveAll(o => existingCoinGameObjects.Contains(o));
            coins.Clear();
            playerStart = null;
            playerExit = null;

            // Create map tiles
            var mapData = getMapData(mapPath);
            var tileGrid = generateTileGrid(mapData);
            Map.Tiles = tileGrid;

            // Set player position for new map
            player.Props.Position = playerStart.Value;

            // Verify that the level has a beginning and an end.
            if (playerStart == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (playerExit == null)
                throw new NotSupportedException("A level must have an exit.");

        }

        string getMapData(string mapPath)
        {
            var mapData = string.Empty;

            // Read map data from file
            using (var stream = TitleContainer.OpenStream(mapPath))
                using (var rdr = new StreamReader(stream))
                    mapData = rdr.ReadToEnd();

            return mapData;
        }

        Tile[,] generateTileGrid(string mapData)
        {
            // Load the level and ensure all of the lines are the same length.
            int width = 0;

            var lines = new List<string>();

            using (var sr = new StringReader(mapData))
            {
                var line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    width = Math.Max(width, line.Length);
                    lines.Add(line);
                }
            }

            var tileGrid = new Tile[width, lines.Count];

            // Loop over every tile position
            for (var y = 0; y < tileGrid.GetLength(1); ++y)
            {
                for (var x = 0; x < tileGrid.GetLength(0); ++x)
                {
                    // to load each tile
                    var tileType = ' ';
                    var line = lines[y];

                    if (line.Length > x)
                        tileType = line[x];

                    tileGrid[x, y] = createTileData(tileType, x, y);
                }
            }

            return tileGrid;
        }

        Tile createTileData(char tileType, int x, int y)
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
                case ' ':
                    return new Tile(pixel_half_trans, TileCollision.Passable, Color.Gray);

                // Coin
                case 'c':
                    addCoinToLevel(tileCenter);
                    return new Tile(pixel_half_trans, TileCollision.Passable, Color.Gray);

                // Death
                case '^':
                    return new Tile(pixel_half_trans, TileCollision.Death, Color.Red);

                // Exit
                case 'X':
                    if (playerExit != null)
                        throw new NotSupportedException("A level may only have one exit.");

                    playerExit = tileCenter;

                    addDoorToLevel(playerExit.Value);

                    return new Tile(pixel, TileCollision.Passable, Color.Gray);

                // Impassable block
                case '#':
                    return new Tile(pixel, TileCollision.Impassable, Color.Gray);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position [{1},{2}].", tileType, x, y));
            }
        }

        void addCoinToLevel(Vector2 position)
        {
            var coin = new Coin();
            coin.Load(_loader);
            coin.Props.Position = position;
            coins.Add(coin);
            GameObjects.Add(coin);
        }

        void addDoorToLevel(Vector2 position)
        {
            if (door != null && GameObjects.Contains(door))
                GameObjects.Remove(door);

            // Create door
            door = new Door();
            position.X -= door.CollisionBox.Center.X;
            position.Y -= door.CollisionBox.Center.Y;
            door.Props.Position = position;
            door.Load(_loader);
            GameObjects.Add(door);
        }


        public override void Update(GameTime time)
        {
            // Update gameobjects
            base.Update(time);

            foreach (var listener in keyListeners)
                listener.Update(time);

            // Map collisions
            var mapCollisions = phys.ApplyPhysics(player.Props, time);

            // Handle death tiles
            var death = mapCollisions.ContainsValue(TileCollision.Death);
            if (death)
            {
                onDeath();
                return;
            }

            checkPlayerCollisions();
        }

        void checkPlayerCollisions()
        {
            // Actor -> coin collision
            foreach (var coin in coins.Where(c => c.IsCollisionEnabled))
            {
                if (coin.CollisionBox.Intersects(player.CollisionBox))
                {
                    player.OnOverlapping(coin);
                    coin.OnOverlapping(player);

                    // Increment count
                    CoinsCount.Value += 1;
                }
            }


            if (door.IsCollisionEnabled)
            {
                // Actor -> door collision
                if (door.CollisionBox.Intersects(player.CollisionBox))
                {
                    door.OnOverlapping(player);

                    loadMap(ContentPaths.PATH_MAP_2, _loader);
                }
            }


        }

        void onDeath()
        {
            // TODO: Play sfx, etc

            // Reload current map
            loadMap(currentMapPath, _loader);
        }

    }
}
