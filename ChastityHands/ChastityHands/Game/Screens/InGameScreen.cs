using Bonsai.Framework;
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
    public class InGameScreen : IScreen
    {
        public InGameScreen()
        {
            DrawOrder = 0;
        }

        public delegate void delStartGame(int score, bool completedGame);
        public delegate void delBackToMenu();
        public event delStartGame GameOver;
        public event delBackToMenu BackToMenu;

        Field<string> health;
        Field<string> score;
        List<KeyPressListener> keyListeners;
        Queue<Cock> cocksAll;
        Queue<Cock> cocksActive;
        Cock activeCock;
        Hand leftHand;
        Hand rightHand;
        int slapCount = 0;
        int handDamage = 2;
        Texture2D texBackground;

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public int DrawOrder { get; set; }


        public void Load(IContentLoader content)
        {
            //background
            texBackground = content.Load<Texture2D>("open-legs");

            //key listeners
            keyListeners = new List<KeyPressListener>
            {
                new KeyPressListener(Keys.Left, keyPressed_Left),
                new KeyPressListener(Keys.Right, keyPressed_Right),
                new KeyPressListener(Keys.D, keyPress_D),
                new KeyPressListener(Keys.Escape, keyPress_Escape),
            };

            //ui elements
            health = new Field<string>(Globals.GeneralFont, "Health:", string.Empty, eDisplayMode.LabelAndValue)
            {
                Position = new Microsoft.Xna.Framework.Vector2(10, 10),
            };
            score = new Field<string>(Globals.GeneralFont, "Score:", "0", eDisplayMode.LabelAndValue)
            {
                Position = new Microsoft.Xna.Framework.Vector2(10, 50),
            };

            //hands
            leftHand = new Hand(Handedness.Left, "chastity-hand", "SFX/slap-1");
            rightHand = new Hand(Handedness.Right, "chastity-hand", "SFX/slap-1");
            leftHand.Load(content);
            rightHand.Load(content);

            //position hands
            leftHand.Position = Vector2.Zero - new Vector2(37,0);
            rightHand.Position = new Vector2((Bonsai.Framework.Globals.Viewport.Right - 430) + 37, 0);

            //create lots of cocks
            var cock1 = new Cock(90, "Cocks/cock-1");
            cock1.FullyPenetrated += cock_FullyPenetrated;
            cock1.Load(content);
            var cock2 = new Cock(130, "Cocks/cock-2");
            cock2.FullyPenetrated += cock_FullyPenetrated;
            cock2.Load(content);
            var cock3 = new Cock(160, "Cocks/cock-3");
            cock3.FullyPenetrated += cock_FullyPenetrated;
            cock3.Load(content);
            var cock4 = new Cock(190, "Cocks/cock-4");
            cock4.FullyPenetrated += cock_FullyPenetrated;
            cock4.Load(content);

            cocksActive = new Queue<Cock>();
            cocksAll = new Queue<Cock>();
            cocksAll.Enqueue(cock1);
            cocksAll.Enqueue(cock2);
            cocksAll.Enqueue(cock3);
            cocksAll.Enqueue(cock4);

            ResetGameState();
        }

        public void Unload()
        {
        }

        public void Update(GameFrame frame)
        {
            //key listeners
            keyListeners.ForEach(l => l.Update(frame.GameTime));

            //hands
            leftHand.Update(frame);
            rightHand.Update(frame);

            //ui
            health.Update(frame.GameTime);
            score.Update(frame.GameTime);

            //active cock
            activeCock.Update(frame);
        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            //bg
            batch.Draw(texBackground, Vector2.Zero, Color.White);

            //cock
            activeCock.Draw(frame, batch);

            //hands
            leftHand.Draw(frame, batch);
            rightHand.Draw(frame, batch);

            //ui
            health.Draw(batch);
            score.Draw(batch);

        }

        public void ResetGameState()
        {
            // Clear cock queue
            cocksActive.Clear();

            // Load up the line of cocks
            foreach (var cock in cocksAll)
                cocksActive.Enqueue(cock);

            // Whip out the first cock
            nextCock();

        }

        void nextCock()
        {
            // Get next cock
            activeCock = cocksActive.Dequeue();

            // Update health value
            health.Value = activeCock.Health.ToString();

        }

        void hitCock()
        {
            //update score
            slapCount++;
            score.Value = slapCount.ToString();

            //apply damage to cock
            activeCock.Health -= handDamage;
            health.Value = activeCock.Health.ToString();

            //cock still alive, don't continue
            if (activeCock.Health > 0)
                return;

            //-- dead cock! --

            //any left in cock queue?
            if (cocksActive.Count > 0)
            {
                //load next cock
                nextCock();
            }
            else
            {
                //all cocks defeated

                //TODO: play success sound

                //move to gameover screen
                if (GameOver != null)
                    GameOver(slapCount, true);

            }

        }

        void cock_FullyPenetrated()
        {
            //TODO: play fail sound

            //move to gameover screen
            if (GameOver != null)
                GameOver(slapCount, false);

        }

        void keyPressed_Left()
        {
            leftHand.HandleSlap();

            hitCock();
        }

        void keyPressed_Right()
        {
            rightHand.HandleSlap();

            hitCock();
        }

        void keyPress_D()
        {
            cock_FullyPenetrated();
        }

        void keyPress_Escape()
        {
            if (BackToMenu != null)
                BackToMenu();
        }


    }
}
