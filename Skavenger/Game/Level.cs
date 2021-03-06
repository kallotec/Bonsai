﻿using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Graphics;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.Text;
using Bonsai.Framework.Text.Managers;
using Bonsai.Framework.UI;
using Bonsai.Framework.Utility;
using Bonsai.Framework.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Skavenger.Game.Loot;
using Svg;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Skavenger.Game
{
    public class Level : Screen
    {
        public Level(BonsaiGame game, EventBus eventBus) : base(game)
        {
            keyListeners = new List<KeyPressListener>();

            this.eventBus = eventBus;
            this.eventBusSubscriptionIds = new List<string>();

            chunkMap = new ChunkMap(100, 100, 100, 100);

            physSettings = new PhysicsSettings
            {
                Gravity = 5f,
                PhysicsType = PhysicsType.Topdown,
                Friction = 0.1f,
                TerminalVelocity = 200f,
            };

        }

        GameVariable<bool> varShowLootTip;
        TextElement<string> txtDisplayTipLootBoxOpen;
        EventBus eventBus;
        List<string> eventBusSubscriptionIds;
        Player player;
        Bot bot;
        TextPopupManager textPopupManager;
        List<KeyPressListener> keyListeners;
        MapPhysics phys;
        PhysicsSettings physSettings;
        Vector2 playerStart;
        Vector2 playerExit;
        IContentLoader _loader;
        ChunkMap chunkMap;

        public bool IsDisabled { get; set; }


        public override void Load(IContentLoader loader)
        {
            _loader = loader;

            var defaultFont = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);

            varShowLootTip = new GameVariable<bool>();
            varShowLootTip.Load(loader);
            GameObjects.Add(varShowLootTip);

            // Player
            player = new Player(eventBus, varShowLootTip);
            player.Load(loader);
            GameObjects.Add(player);

            // Bot
            bot = new Bot(eventBus, Camera);
            bot.Load(loader);
            GameObjects.Add(bot);

            // Message manager
            textPopupManager = new TextPopupManager(ContentPaths.FONT_UI_GENERAL, StackingMethod.Parallel);
            textPopupManager.Load(loader);
            GameObjects.Add(textPopupManager);

            // Physics
            phys = new MapPhysics(chunkMap, physSettings);
            GameObjects.Add(chunkMap);

            // Camera
            GameObjects.Add(Camera);

            // Services
            setupKeyListeners();

            // Event listeners
            eventBusSubscriptionIds.AddRange(new[] {
                eventBus.Subscribe(Events.PlayerPickedUpCoin, (p) => Debug.WriteLine("picked up coin!") ),
                eventBus.Subscribe(Events.PlayerDied, (p) => playerDied()),
                eventBus.Subscribe(Events.CreateProjectile, (p) => playerCreatedProjectile(p as Projectile)),
            });

            // Game variables
            var tipLootBoxOpen = "Press <E> to open loot box";
            txtDisplayTipLootBoxOpen = new TextElement<string>(tipLootBoxOpen, defaultFont, new TextElementSettings())
            {
                Position = new Vector2(10, 10),
                IsAttachedToCamera = true,
                IsHidden = true,
            };
            txtDisplayTipLootBoxOpen.Load(loader);
            GameObjects.Add(txtDisplayTipLootBoxOpen);

            // Map
            loadMap();

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

            // key listeners
            foreach (var listener in keyListeners)
                listener.Update(time);

            // physics
            phys.ApplyPhysics(player.Props, player, time);
            phys.ApplyPhysics(bot.Props, bot, time);

            foreach (var p in GameObjects.OfType<Projectile>())
                phys.ApplyPhysics(p.Props, p, time);

            txtDisplayTipLootBoxOpen.IsHidden = !varShowLootTip.Value;
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

        void loadMap()
        {
            // reset game objects
            GameObjects.RemoveAll(o => o is LootBox || o is Wall);

            // setup chunks
            chunkMap.Reset(chunkWidth: 100, chunkHeight: 100, mapWidth: 1000, mapHeight: 1000);

            // physics reset
            phys = new MapPhysics(chunkMap, physSettings);

            // read map elements
            var doc = SvgDocument.Open(ContentPaths.PATH_MAP_1);
            var rects = doc.Children.FindSvgElementsOf<SvgRectangle>();

            foreach (var rect in rects)
            {
                if (rect.Fill.ToString() == "#000001")
                {
                    playerStart = new Vector2(rect.X, rect.Y);
                } 
                else if (rect.Fill.ToString() == "#000002")
                {
                    playerExit = new Vector2(rect.X, rect.Y);
                }
                else if (rect.Fill.ToString() == "#000010")
                {
                    var lootBox = new LootBox();
                    lootBox.Props.Position = new Vector2(rect.X, rect.Y);
                    lootBox.Load(_loader);
                    GameObjects.Add(lootBox);
                    chunkMap.UpdateEntity(lootBox);
                }
                else
                {
                    // create wall
                    var fill = getRgba(rect.Fill.ToString());
                    var wall = new Wall(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), fill);
                    wall.Load(_loader);
                    GameObjects.Add(wall);
                    chunkMap.UpdateEntity(wall);
                }
            }

            // Verify that the level has a beginning and an end.
            //if (playerStart == Vector2.Zero)
            //    throw new NotSupportedException("A level must have a starting point.");
            if (playerExit == Vector2.Zero)
                throw new NotSupportedException("A level must have an exit.");

            // Set player position for new map
            player.Props.Position = playerStart;
            bot.Props.Position = new Vector2(10, 10);
            chunkMap.UpdateEntity(player);
            chunkMap.UpdateEntity(bot);

            addCoinToLevel(new Vector2(200, 200));

            Camera.SetFocus(player, immediateFocus: true);
        }

        void addCoinToLevel(Vector2 position)
        {
            var coin = new Coin();
            coin.Load(_loader);
            coin.Props.Position = position;
            GameObjects.Add(coin);
            chunkMap.UpdateEntity(coin);
        }

        void playerDied()
        {
            IsDisabled = true;
            // play victory sfx

            // load map again
            loadMap();
        }

        void playerCreatedProjectile(Projectile projectile)
        {
            projectile.Load(_loader);
            GameObjects.Add(projectile);
            chunkMap.UpdateEntity(projectile);
        }

        Color getRgba(string hex)
        {
            hex = hex.TrimStart('#');
            var r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b);
        }
    }
}
