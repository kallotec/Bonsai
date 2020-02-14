using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Text;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.Variables;
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
            keyListeners = new List<KeyPressListener>();
            coins = new List<Coin>();
            eventBus = new EventBus();
        }

        EventBus eventBus;
        Player player;
        HUD hud;
        UIMessageManager msgManager;
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
        ChunkMap chunkMap;

        public event EventHandler Exit;
        public event EventHandler GameOver;

        public GameVariable<int> Jumps;
        public GameVariable<int> CoinsCount;
        public bool IsDisabled { get; set; }


        public override void Load(IContentLoader loader)
        {
            _loader = loader;

            // Content
            font = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);
            pixel = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            pixel_half_trans = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL_HALFTRANS);

            // Player
            player = new Player(eventBus);
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

            // Message manager
            msgManager = new UIMessageManager(ContentPaths.FONT_UI_GENERAL, StackingMethod.Parallel);
            msgManager.Load(loader);
            GameObjects.Add(msgManager);

            // HUD
            hud = new HUD(this);
            hud.ScreenBounds = base.Game.GraphicsDevice.Viewport.Bounds;
            hud.Exit += (s, e) => { Exit?.Invoke(s, e); };
            hud.DrawOrder = 1;
            hud.Load(loader);
            GameObjects.Add(hud);

            // Physics
            phys = new MapPhysics(chunkMap);

            // Camera
            Camera.SetFocus(player);
            GameObjects.Add(Camera);

            // Services
            setupKeyListeners();

            // Event listeners
            eventBus.Subscribe("playerPickedUpCoin", () => CoinsCount.Value += 1);
            eventBus.Subscribe("playerJumped", () => Jumps.Value += 1);

            // Load first map
            loadMap(ContentPaths.PATH_MAP_1);
        }

        void setupKeyListeners()
        {
            // Key listeners
            keyListeners = new List<KeyPressListener>
            {
                // [M] key generates a text popup at the players position
                new KeyPressListener(Keys.M, () =>
                {
                    msgManager.AddMessage("Hey!", 
                        player.Props.Position + new Vector2(0,-20),
                        MessageType.FadingText_Fast);
                }),
                // [ESC] Exit
                new KeyPressListener(Keys.Escape, () => Exit?.Invoke(this,null))
            };
        }

        void loadMap(string mapPath)
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

            // Setup chunks
            chunkMap = new ChunkMap(chunkWidth: 100, chunkHeight: 100, mapWidth: 1000, mapHeight: 1000);


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
            phys.ApplyPhysics(player.Props, player, time);

            // Handle death tiles
            var death = mapCollisions.ContainsValue(TileCollision.Death);
            if (death)
            {
                onDeath();
                return;
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
