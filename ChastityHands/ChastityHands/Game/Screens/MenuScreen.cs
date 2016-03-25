using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
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
    public class MenuScreen : BonsaiGameObject, IHUD
    {
        public MenuScreen()
        {
            DrawOrder = 2;
        }

        public delegate void delStartGame();
        public delegate void delQuit();
        public event delStartGame StartGame;
        public event delQuit QuitGame;

        Field<string> fieldInstructions;
        List<KeyPressListener> keyListeners;

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public int DrawOrder { get; set; }


        public void Load(IContentLoader content)
        {
            //key listeners
            keyListeners = new List<KeyPressListener>
            {
                new KeyPressListener(Keys.Enter, keyPressed_Enter),
                new KeyPressListener(Keys.Escape, keyPressed_Esc),
            };

            //ui
            fieldInstructions = new Field<string>(Globals.GeneralFont, "[Chastity Hands]\n\nDefend yourself with <Left Arrow> and <Right Arrow>\n\n-------------\n\nPress <Enter> to start game or <Esc> to quit", string.Empty, FieldDisplayMode.LabelOnly)
            {
                Position = Bonsai.Framework.Globals.Viewport_Centerpoint + new Vector2(0, -70),
                Alignment = FieldAlignmentMode.Center,
                Color = Color.White,
            };
        }

        public void Unload()
        {
        }

        public void Update(GameFrame frame)
        {
            //key listener
            keyListeners.ForEach(l => l.Update(frame.GameTime));

            //fields
            fieldInstructions.Update(frame.GameTime);
        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            //ui
            fieldInstructions.Draw(batch);
        }


        void keyPressed_Enter()
        {
            if (StartGame != null)
                StartGame();
        }

        private void keyPressed_Esc()
        {
            if (QuitGame != null)
                QuitGame();
        }


    }
}
