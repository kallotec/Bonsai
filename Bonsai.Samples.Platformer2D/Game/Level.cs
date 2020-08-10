using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.Text.Managers;
using Bonsai.Framework.UI;
using Bonsai.Framework.Utility;
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
        public Level(BonsaiGame game, EventBus eventBus) : base(game)
        {
            keyListeners = new List<KeyPressListener>();

            this.eventBus = eventBus;
            this.eventBusSubscriptionIds = new List<string>();

            chunkMap = new ChunkMap(10, 10, 10, 10);
        }

        EventBus eventBus;
        List<string> eventBusSubscriptionIds;
        Player player;
        HUD hud;
        TextPopupManager textPopupManager;
        List<KeyPressListener> keyListeners;
        MillisecCounter startGameTimer;
        MapPhysics phys;
        Door door;
        Vector2 playerStart;
        Vector2 playerExit;
        IContentLoader _loader;
        string currentMapPath;
        ChunkMap chunkMap;
        bool isPlayerFocused;

        public GameVariable<int> Jumps;
        public GameVariable<int> CoinsCount;
        public bool IsDisabled { get; set; }


        public override void Load(IContentLoader loader)
        {
            _loader = loader;

            // Player
            player = new Player(eventBus, Camera);
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
            hud.DrawOrder = DrawOrderPosition.HUD;
            hud.Load(loader);
            GameObjects.Add(hud);

            // Physics
            phys = new MapPhysics(chunkMap);
            GameObjects.Add(chunkMap);

            // Camera
            GameObjects.Add(Camera);

            // Services
            setupKeyListeners();

            // Event listeners
            eventBusSubscriptionIds.AddRange(new[] {
                eventBus.Subscribe(Events.PlayerPickedUpCoin, (p) => CoinsCount.Value += 1),
                eventBus.Subscribe(Events.PlayerJumped, (p) => Jumps.Value += 1),
                eventBus.Subscribe(Events.PlayerEnteredDoor, (p) => playerTouchedDoor()),
                eventBus.Subscribe(Events.PlayerDied, (p) => playerDied()),
                eventBus.Subscribe(Events.CreateProjectile, (p) => playerCreatedProjectile(p as Projectile)),
            });

            // Load first map
            loadMap(ContentPaths.PATH_MAP_1);

            startGameTimer = new MillisecCounter(1500);
            Camera.SetFocus(playerExit, immediateFocus: true);
            isPlayerFocused = false;
        }

        public override void Unload()
        {
            base.Unload();
            GameObjects.Clear();
            eventBus.Unsubscribe(eventBusSubscriptionIds);
        }

        public override void Update(GameTime time)
        {
            // Update gameobjects
            base.Update(time);

            if (isPlayerFocused)
            {
                // key listeners
                foreach (var listener in keyListeners)
                    listener.Update(time);

                // physics
                phys.ApplyPhysics(player.Props, player, time);

                foreach (var p in GameObjects.OfType<Projectile>())
                {
                    phys.ApplyPhysics(p.Props, p, time);
                } 
            }

            if (!startGameTimer.Completed)
                startGameTimer.Update(time.ElapsedGameTime.Milliseconds);

            if (!isPlayerFocused && startGameTimer.Completed)
            {
                isPlayerFocused = true;
                Camera.SetFocus(player, immediateFocus: false);
            }

            chunkMap.RemoveDeletedEntities();
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
                new KeyPressListener(Keys.Escape, () => eventBus.QueueNotification(Events.BackToStartScreen))
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
                // Color
                var fillColor = elem.Fill?.ToString() ?? "#FF0000";
                var strokeColor = elem.Stroke?.ToString();

                if (fillColor == null || !fillColor.StartsWith("#"))
                    fillColor = "#FF0000";
                if (strokeColor == null || !strokeColor.StartsWith("#"))
                    strokeColor = null;

                // Shape
                var x = (int)Math.Round(elem.Bounds.X, 1);
                var y = (int)Math.Round(elem.Bounds.Y, 1);
                var w = (int)Math.Round(elem.Bounds.Width, 1);
                var h = (int)Math.Round(elem.Bounds.Height, 1);

                Debug.WriteLine($"{x} {y} - {w} x {h}");

                if (fillColor == "#000001")
                {
                    playerStart.X = x;
                    playerStart.Y = y;
                }
                else if (fillColor == "#000002")
                {
                    // Door
                    playerExit.X = x;
                    playerExit.Y = y;
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

            // setup chunks
            chunkMap.Reset(chunkWidth: 100, chunkHeight: 100, mapWidth: maxMapX, mapHeight: maxMapY);

            // physics reset
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

            addCoinToLevel(new Vector2(300,0));

            // Add end point
            addDoorToLevel(playerExit);
        }

        void addCoinToLevel(Vector2 position)
        {
            var coin = new Coin();
            coin.Load(_loader);
            coin.Props.Position = position;
            GameObjects.Add(coin);
            chunkMap.UpdateEntity(coin);
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

        void playerCreatedProjectile(Projectile projectile)
        {
            projectile.Load(_loader);
            GameObjects.Add(projectile);
            chunkMap.UpdateEntity(projectile);
        }

    }
}
