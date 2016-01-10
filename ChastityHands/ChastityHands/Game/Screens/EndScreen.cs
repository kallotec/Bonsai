using Bonsai.Framework;
using Bonsai.Framework.Common;
using Bonsai.Framework.Screens;
using Bonsai.Framework.UI;
using ChastityHands.Game.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChastityHands.Game.Screens
{
    public class EndScreen : IScreen
    {
        public delegate void delStartGame();
        public delegate void delBackToMenu();
        public event delStartGame StartGame;
        public event delBackToMenu BackToMenu;

        List<KeyPressListener> keyListeners;
        Field<string> fieldScore;
        Field<string> fieldSuccess;
        Field<string> retryMessage;


        public void Load(ContentManager content)
        {
            //enter key listener
            keyListeners = new List<KeyPressListener>
            {
                new KeyPressListener(Keys.Enter, keyPressed_Enter),
                new KeyPressListener(Keys.Escape, keyPressed_Esc),
            };

            //score
            fieldScore = new Field<string>(Globals.GeneralFont, "Score: ", "0", eDisplayMode.LabelAndValue)
            {
                Alignment = eTextAlignment.Center,
                Position = new Vector2(Bonsai.Framework.Globals.Viewport_Centerpoint.X, Bonsai.Framework.Globals.Viewport.Top + 60),
                Color = Color.SkyBlue,
            };

            //success
            fieldSuccess = new Field<string>(Globals.GeneralFont, "Success: ", "false", eDisplayMode.LabelAndValue)
            {
                Alignment = eTextAlignment.Center,
                Position = fieldScore.Position + new Vector2(0, 60),
                Color = Color.SkyBlue,
            }; 

            //retry msg
            retryMessage = new Field<string>(Globals.GeneralFont, "Press <Enter> to retry or <Esc> to menu", string.Empty, eDisplayMode.LabelOnly)
            {
                Alignment = eTextAlignment.Center,
                Position = Bonsai.Framework.Globals.Viewport_Centerpoint,
                Color = Color.SkyBlue,
            }; 

        }


        public void Unload()
        {
        }

        public void Update(GameFrame frame)
        {
            //key listeners
            keyListeners.ForEach(l => l.Update(frame.GameTime));
        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            //ui
            fieldScore.Draw(batch);
            fieldSuccess.Draw(batch);
            retryMessage.Draw(batch);
        }

        public void RefreshResults(int score, bool completed)
        {
            //update values on ui field
            fieldScore.Value = score.ToString();
            fieldSuccess.Value = completed.ToString();
        }

        void keyPressed_Enter()
        {
            if (StartGame != null)
                StartGame();
        }

        void keyPressed_Esc()
        {
            if (BackToMenu != null)
                BackToMenu();
        }

    }
}
