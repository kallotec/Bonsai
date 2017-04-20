using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using System.Diagnostics;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Bonsai.Samples.Platformer2D.Game;
using Bonsai.Framework.Content;
using Bonsai.Samples.Platformer2D.Game.Components;

namespace Bonsai.Samples.Platformer2D
{
    public enum GameScreen { Menu, InGame, EndGame }

    public class PlatformerGame : BonsaiGame
    {
        public PlatformerGame()
        {
            Content.RootDirectory = "Content";
        }

        Level level;
        Screen currentScreen;


        protected override void Init()
        {
            // Set up the game window
            base.SetWindow("Platformer Demo", width: 800, height: 600, showMouse: true);

            // Create level
            level = new Level(this);
            level.Exit += (s, e) => this.Exit();

            currentScreen = level;
        }

        protected override void Load(IContentLoader loader)
        {
            level.Load(loader);
        }

        protected override void Unload()
        {
            level.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(gameTime);
        }

    }
}
