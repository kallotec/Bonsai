using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChastityHands.Game.Entities
{
    public enum Handedness { Left, Right }

    public class Hand : MoveableActor
    {
        public Hand(Handedness handedness, string texHandPath, string sfxSlapPath) : base(0, 0)
        {
            this.texHandPath = texHandPath;
            this.sfxSlapPath = sfxSlapPath;
            this.handedness = handedness;
        }

        string sfxSlapPath;
        string texHandPath;
        MillisecCounter timerSlap;
        Handedness handedness;
        SoundEffect sfxSlap;


        public override void LoadContent(ContentManager content)
        {
            //create timer for slap animation
            timerSlap = new MillisecCounter(0100);
            //set timer into completed state
            timerSlap.Update(timerSlap.Max);

            // load hand tex
            base.Texture = content.Load<Texture2D>(texHandPath);

            //load slap audio
            sfxSlap = content.Load<SoundEffect>(sfxSlapPath);
        }

        public override void Draw(SpriteBatch batch)
        {
            //draw hand if slapping
            if (!timerSlap.Completed)
            {
                batch.Draw(base.Texture, base.Position, null, 
                            Color.White, 0f, Vector2.Zero, 1,
                            (handedness == Handedness.Left ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 
                            0);
            }

        }

        public override void Update(GameFrame gameFrame)
        {
            //update slap display timer
            timerSlap.Update(gameFrame.GameTime.ElapsedGameTime.Milliseconds);
        }

        public override void CollidedWith(IGameObject actor)
        {
        }

        public override void TouchedBy(IGameObject actor)
        {
        }

        public void HandleSlap()
        {
            //slap sfx
            sfxSlap.Play();

            //reset display timer
            timerSlap.Reset();

        }

    }
}
