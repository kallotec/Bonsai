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

namespace ChastityHands.Game
{
    enum GameState { Menu, InGame, End }

    public class GameManager : BonsaiGameBase
    {
        public GameManager()
        {
            state = GameState.Menu;
        }

        GameState state;
        MenuScreen menuScreen;
        InGameScreen gameScreen;
        EndScreen endScreen;


        protected override void Init()
        {
            //setup
            base.SetWindow(width: 800, height: 600);
            base.SetMouse(isVisible: false);
        }

        protected override void Load(ContentManager content)
        {
            //fonts
            Globals.GeneralFont = content.Load<SpriteFont>("UI/ui_regular");

            //menu
            menuScreen = new MenuScreen();
            menuScreen.StartGame += menuScreen_StartGame;
            menuScreen.QuitGame += menuScreen_QuitGame;
            menuScreen.Load(content);

            //ingame screen created on start command from menu

            //end
            endScreen = new EndScreen();
            endScreen.StartGame += menuScreen_StartGame;
            endScreen.BackToMenu += endScreen_BackToMenu;
            endScreen.Load(content);

            //display start menu
            state = GameState.Menu;

        }

        protected override void Unload()
        {
            if (menuScreen != null) 
                menuScreen.Unload();

            if (gameScreen != null) 
                gameScreen.Unload();

            if (endScreen != null) 
                endScreen.Unload();
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
            //create game screen
            gameScreen = new InGameScreen();
            gameScreen.GameOver += gameScreen_GameOver;
            gameScreen.BackToMenu += gameScreen_BackToMenu;
            gameScreen.Load(base.Content);

            state = GameState.InGame;

        }

        void menuScreen_QuitGame()
        {
            //kill app
            this.Exit();
        }

        void gameScreen_BackToMenu()
        {
            state = GameState.Menu;
        }

        void gameScreen_GameOver(int score, bool completedGame)
        {
            //pass score to screen
            endScreen.RefreshResults(score, completedGame);

            state = GameState.End;
        }

        void endScreen_BackToMenu()
        {
            state = GameState.Menu;
        }

    }
}
