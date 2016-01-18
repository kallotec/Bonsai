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
        Random random = new Random(354668);
        List<KeyPressListener> keyListeners;

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; private set; }
        public int DrawOrder { get; set; }
        public ICamera Camera { get; set; }
        public TileMap TileMap { get; private set; }
        public delegate void delExit();
        public event delExit Exit;


        public void Load(IContentLoader content)
        {
            this.content = content;

            //key listeners
            configureKeyListeners();

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
            TileMap.LoadMap(map1);

            // Create player
            player = new Player(this, TileMap.Start.Value);
            player.Load(content);

            // Focus camera on player
            Camera.SetFocus(player);

        }

        public void Unload()
        {
        }

        public void Update(GameFrame frame)
        {
            // Key listeners
            foreach (var listener in keyListeners)
                listener.Update(frame.GameTime);

            player.Update(frame);

        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            //draw tile map
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

            //draw player
            player.Draw(frame, batch);

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
