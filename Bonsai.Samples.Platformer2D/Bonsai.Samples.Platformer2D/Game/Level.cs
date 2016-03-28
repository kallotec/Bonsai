using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.UI.Widgets.Popups;
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
    public class Level : DrawableBase, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Level()
        {
            base.DrawOrder = 0;
            popupManager = new PopupManager();
            keyListeners = new List<KeyPressListener>();
            //particles = new List<ParticleGroup>();
        }

        Player player;
        PopupManager popupManager;
        List<KeyPressListener> keyListeners;
        SpriteFont font;
        //List<ParticleGroup> particles;

        public GameVariable<int> Jumps;
        public bool IsDisabled { get; set; }
        public ICamera Camera { get; set; }
        public TileMap TileMap { get; private set; }


        public void Load(IContentLoader loader)
        {
            // Fonts
            font = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);

            // Create sample map
            var map1 =  "#....................................#\n" +
                        "#.1..................................#\n" +
                        "#........###...............###.......#\n" +
                        "#.#....................#.............#\n" +
                        "#........#....#.........#............#\n" +
                        "#....###..#..###.......###.....###...#\n" +
                        "#...........#.....#..#....#..........#\n" +
                        "#........##......###........##...X...#\n" +
                        "######################################";

            TileMap = new TileMap(tileWidth: 22, tileHeight: 22);
            TileMap.LoadContent(loader, map1);

            // Create player
            player = new Player(this, TileMap.Start.Value);
            player.Load(loader);

            // Focus camera on player
            Camera.SetFocus(player);
            
            // Setup game variables
            Jumps = new GameVariable<int>();

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
                            Position = player.Position + new Vector2(0,-20)
                        });
                })
            };

        }

        public void Unload()
        {
            Jumps.Unload();
            popupManager.Clear();
        }

        public void Update(GameTime time)
        {
            // Keys
            foreach (var listener in keyListeners)
                listener.Update(time);

            player.Update(time);
            popupManager.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Draw map
            for (var x = 0; x < TileMap.Tiles.GetLength(0); x++)
            {
                for (var y = 0; y < TileMap.Tiles.GetLength(1); y++)
                {
                    var tile = TileMap.Tiles[x, y];
                    var position = new Vector2(x, y) * new Vector2(TileMap.TileSize.X, TileMap.TileSize.Y);

                    if (tile.Texture == null)
                        continue;

                    batch.Draw(tile.Texture, new Rectangle((int)position.X, (int)position.Y, TileMap.TileSize.X, TileMap.TileSize.Y), tile.Tint);
                }
            }

            // Player
            player.Draw(time, batch);

            //// Particles
            //foreach (var particle in particles)
            //    particle.Draw(batch);

            // Messages
            popupManager.Draw(time, batch);

        }

    }
}
