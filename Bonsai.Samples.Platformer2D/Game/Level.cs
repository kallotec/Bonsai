using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.Text.Managers;
using Bonsai.Framework.UI;
using Bonsai.Framework.Variables;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
        TextPopupManager textPopupManager;
        List<KeyPressListener> keyListeners;
        SpriteFont font;
        Texture2D pixel_half_trans;
        Texture2D pixel;
        MapPhysics phys;
        List<Coin> coins;
        Door door;
        Vector2 playerStart;
        Vector2 playerExit;
        IContentLoader _loader;
        string currentMapPath;
        ChunkMap chunkMap;
        int map_yoffset_correction;

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
            player.DrawOrder = DrawOrderPosition.Foreground;
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
            textPopupManager = new TextPopupManager(ContentPaths.FONT_UI_GENERAL, StackingMethod.Parallel);
            textPopupManager.Load(loader);
            GameObjects.Add(textPopupManager);

            // HUD
            hud = new HUD(this);
            hud.ScreenBounds = base.Game.GraphicsDevice.Viewport.Bounds;
            hud.Exit += (s, e) => { Exit?.Invoke(s, e); };
            hud.DrawOrder = DrawOrderPosition.HUD;
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
            eventBus.Subscribe("playerEnteredDoor", () => playerTouchedDoor());
            eventBus.Subscribe("playerDied", () => playerDied());

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
                    textPopupManager.AddMessage("Hey!", 
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
            GameObjects.RemoveAll(o => o is Coin || o is Platform || o is Door);
            coins.Clear();

            // Create map tiles
            var doc = SvgDocument.Open(mapPath);
            var elements = doc.Children.FindSvgElementsOf<SvgPath>();

            // Load new platforms
            var platformsToLoad = new List<Platform>();

            var maxMapX = 0;
            var maxMapY = 0;
            playerStart = new Vector2(0, 0);
            playerExit = new Vector2(0, 0);

            foreach (var elem in elements)
            {
                //// Color
                var fillColor = elem.Fill?.ToString() ?? "#FF0000";
                var strokeColor = elem.Stroke?.ToString();

                if (fillColor == null || !fillColor.StartsWith("#"))
                    fillColor = "#FF0000";
                if (strokeColor == null || !strokeColor.StartsWith("#"))
                    strokeColor = null;

                // Shape
                var x = (int)Math.Round(elem.Bounds.X, 1);
                var y = (int)Math.Round(elem.Bounds.Y - map_yoffset_correction, 1);
                var w = (int)Math.Round(elem.Bounds.Width, 1);
                var h = (int)Math.Round(elem.Bounds.Height, 1);

                Debug.WriteLine($"{x} {y} - {w} x {h}");

                if (fillColor == "#000001")
                {
                    playerStart.X = x;
                    playerStart.Y = y + map_yoffset_correction;
                }
                else if (fillColor == "#000002")
                {
                    // Door
                    playerExit.X = x;
                    playerExit.Y = y + map_yoffset_correction;
                }
                else
                {
                    // Platform
                    var platform = new Platform(x, y, w, h, fillColor, strokeColor);
                    platform.Load(_loader);
                    platformsToLoad.Add(platform);
                }

                maxMapX = Math.Max(maxMapX, x + w);
                maxMapY = Math.Max(maxMapY, y + h);
            }

            Debug.WriteLine($"Map size in px: {maxMapX} x {maxMapY}");

            // Setup chunks
            chunkMap = new ChunkMap(chunkWidth: 100, chunkHeight: 100, mapWidth: maxMapX, mapHeight: maxMapY);

            // Physics reset
            var physSettings = new PhysicsSettings
            {
                Gravity = 5f,
                Friction = 0.1f,
                TerminalVelocity = 200f,
            };
            phys = new MapPhysics(chunkMap, physSettings);

            foreach (var platform in platformsToLoad)
            {
                if (!GameObjects.Contains(platform))
                    GameObjects.Add(platform);

                chunkMap.UpdateEntity(platform);
            }

            // Verify that the level has a beginning and an end.
            if (playerStart == Vector2.Zero)
                throw new NotSupportedException("A level must have a starting point.");
            if (playerExit == Vector2.Zero)
                throw new NotSupportedException("A level must have an exit.");

            // Set player position for new map
            player.Props.Position = playerStart;

            // Add end point
            addDoorToLevel(playerExit);
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
            door = new Door(eventBus);
            position.X -= door.CollisionBox.Center.X;
            position.Y -= door.CollisionBox.Center.Y;
            door.Props.Position = position;
            door.Load(_loader);
            GameObjects.Add(door);

            chunkMap.UpdateEntity(door);
        }

        void playerTouchedDoor()
        {
            IsDisabled = true;
            // play victory sfx

            // load map again
            loadMap(currentMapPath);
        }

        void playerDied()
        {
            IsDisabled = true;
            // play victory sfx

            // load map again
            loadMap(currentMapPath);
        }


        public override void Update(GameTime time)
        {
            eventBus.FlushNotifications();

            // Update gameobjects
            base.Update(time);

            foreach (var listener in keyListeners)
                listener.Update(time);

            // Map collisions
            phys.ApplyPhysics(player.Props, player, time);
        }

    }
}
