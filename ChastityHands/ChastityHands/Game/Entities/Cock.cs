using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChastityHands.Game.Entities
{
    public class Cock : MoveableActor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Cock(int health, string assetTexSkin)
        {
            DrawOrder = 2;

            this.Health = health;
            this.assetTexSkin = assetTexSkin;
        }

        string assetTexSkin;

        public delegate void delFullyPenetrated();
        public event delFullyPenetrated FullyPenetrated;

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public int DrawOrder { get; set; }
        public int Health { get; set; }


        public void Load(IContentLoader content)
        {
            //load cock skin
            base.Texture = content.Load<Texture2D>(assetTexSkin);

            base.Position = new Vector2(Bonsai.Framework.Globals.Viewport_Centerpoint.X - (150 / 2), -800);
            base.Velocity = new Vector2(0, 1);
        }

        public void Unload()
        {
        }

        public void Update(GameFrame gameFrame)
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

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            batch.Draw(base.Texture, base.Position, base.DrawingTint);
        }

    }
}
