using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Bonsai.Samples.Platformer2D.Game.Actors;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Coin : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.Physics.IPhysicsObject, Bonsai.Framework.IDeletable
    {
        public Coin()
        {
            base.Props.PhysicalRect = new Rectangle(0, 0, 10, 10);
        }

        SoundEffect sfxCollect;

        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera => false;
        public bool IsDisabled => false;
        public bool IsHidden { get; set; }
        public RectangleF CollisionBox => new RectangleF(Props.Position.X, Props.Position.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => false;
        public RectangleF OverlapBox => CollisionBox;

        public void Load(IContentLoader loader)
        {
            base.Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_COIN);
            sfxCollect = loader.Load<SoundEffect>(ContentPaths.SFX_COIN_COLLECT);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(Props.Texture, Props.Position, Props.PhysicalRect, Props.Tint);
        }

        public void OnOverlapping(object actor)
        {
            if (DeleteMe || actor == null)
                return;

            if (actor is Player)
                onCoinCollected();
        }

        void onCoinCollected()
        {
            DeleteMe = true;
            sfxCollect.Play(0.5f, 0f, 0f);
        }

    }
}
