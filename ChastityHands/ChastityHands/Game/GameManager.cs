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
using ChastityHands.Game.Screens;
using Bonsai.Framework.Content;

namespace ChastityHands.Game
{
    enum GameState { Menu, InGame, End }

    public class GameManager : BonsaiGame
    {
        public GameManager()
        {
            state = GameState.Menu;
            instance = this;
        }

        GameState state;
        MenuScreen menuScreen;
        InGameScreen gameScreen;
        EndScreen endScreen;
        static GameManager instance;

        public static GameManager Instance { get { return instance; } }

        
        protected override void Init()
        {
            // Setup game window
            base.SetWindow("Chastity Hands!", width: 800, height: 600, showMouse: false);

            // Menu screen
            menuScreen = new MenuScreen();
            menuScreen.StartGame += menuScreen_StartGame;
            menuScreen.QuitGame += menuScreen_QuitGame;

            // InGame screen
            gameScreen = new InGameScreen();
            gameScreen.GameOver += gameScreen_GameOver;
            gameScreen.BackToMenu += gameScreen_BackToMenu;

            // EndGame screen
            endScreen = new EndScreen();
            endScreen.StartGame += menuScreen_StartGame;
            endScreen.BackToMenu += endScreen_BackToMenu;

            // Display start menu
            state = GameState.Menu;

        }

        protected override void Load(IContentLoader content)
        {
            // Fonts
            Globals.GeneralFont = content.Load<SpriteFont>("UI/ui_regular");

            // Screens
            menuScreen.Load(content);
            gameScreen.Load(content);
            endScreen.Load(content);

        }

        protected override void Update(GameFrame frame)
        {
            switch (state)
            {
                case GameState.Menu:
                    menuScreen.Update(frame);
                    break;

                case GameState.InGame:
                    gameScreen.Update(frame);
                    break;

                case GameState.End:
                    endScreen.Update(frame);
                    break;
            }

        }

        protected override void Draw(GameFrame frame)
        {
            base.SpriteBatch.Begin();

            switch (state)
            {
                case GameState.Menu:
                    menuScreen.Draw(frame, SpriteBatch);
                    break;

                case GameState.InGame:
                    gameScreen.Draw(frame, SpriteBatch);
                    break;

                case GameState.End:
                    endScreen.Draw(frame, SpriteBatch);
                    break;
            }

            base.SpriteBatch.End();
        }


        void menuScreen_StartGame()
        {
            // Reset game state
            gameScreen.ResetGameState();

            // Display InGame screen
            state = GameState.InGame;

        }

        void menuScreen_QuitGame()
        {
            // Kill app
            this.Exit();
        }

        void gameScreen_BackToMenu()
        {
            // Display menu
            state = GameState.Menu;
        }

        void gameScreen_GameOver(int score, bool completedGame)
        {
            // Pass score to endgame screen
            endScreen.RefreshResults(score, completedGame);

            // Display endgame screen
            state = GameState.End;
        }

        void endScreen_BackToMenu()
        {
            // Display menu
            state = GameState.Menu;
        }

    }
}
