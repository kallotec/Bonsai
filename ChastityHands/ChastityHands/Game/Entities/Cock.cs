using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChastityHands.Game.Entities
{
    public class Cock : MoveableActor
    {
        public Cock(int health, string assetTexSkin) : base(0,0)
        {
            this.Health = health;
            this.assetTexSkin = assetTexSkin;
        }

        public delegate void delFullyPenetrated();
        public event delFullyPenetrated FullyPenetrated;

        string assetTexSkin;

        public int Health { get; set; }
        

        public override void LoadContent(ContentManager content)
        {
            //load cock skin
            base.Texture = content.Load<Texture2D>(assetTexSkin);

            base.Position = new Vector2(Bonsai.Framework.Globals.Viewport_Centerpoint.X - (150 / 2), -800);
            base.Velocity = new Vector2(0, 1);
        }

        public override void Update(GameFrame gameFrame)
        {
            //move
            base.Position += base.Velocity;

            //test complete penetration
            if (base.Position.Y > 0)
            {
                //stop moving
                base.Position = new Vector2(base.Position.X, 0);
                base.Velocity = Vector2.Zero;

                //signal penetration
                if (FullyPenetrated != null)
                    FullyPenetrated();
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(base.Texture, base.Position, base.DrawingTint);
        }

        public override void CollidedWith(IGameObject actor)
        {
        }

        public override void TouchedBy(IGameObject actor)
        {
        }

    }

}
