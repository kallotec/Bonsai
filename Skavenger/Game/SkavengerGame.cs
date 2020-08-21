using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using System.Diagnostics;
using Bonsai.Framework.ContentLoading;

namespace Skavenger.Game
{
    public enum GameScreen { Menu, InGame, EndGame }

    public class SkavengerGame : BonsaiGame
    {
        public SkavengerGame()
        {
            Content.RootDirectory = "Content";
        }

        Level level;
        StartScreen startScreen;
        Screen currentScreen;
        EventBus eventBus;
        List<string> eventBusSubscriptionIds = new List<string>();


        protected override void Init()
        {
            // Set up the game window
            base.SetWindow("Platformer Demo", width: 800, height: 600, showMouse: true);

            // events
            eventBus = new EventBus();
            eventBusSubscriptionIds.AddRange(new[] {
                eventBus.Subscribe(Events.BackToStartScreen, action: (p) => goBackToStartMenu())
            });

            startScreen = new StartScreen(this);
            startScreen.StartGame += (s, e) => startGame();
            startScreen.ExitGame += (s, e) => this.Exit();

            level = new Level(this, eventBus);
            
            currentScreen = startScreen;
        }

        protected override void Load(IContentLoader loader)
        {
            startScreen.Load(loader);
        }

        protected override void Unload()
        {
            level?.Unload();
            startScreen.Unload();
            eventBus.Unsubscribe(eventBusSubscriptionIds);
        }

        protected override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            eventBus.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScreen.Draw(gameTime);
        }

        private void startGame()
        {
            level = new Level(this, eventBus);
            level.Load(base.Loader);
            currentScreen = level;
        }

        void goBackToStartMenu()
        {
            level.Unload();
            currentScreen = startScreen;
        }

    }
}
