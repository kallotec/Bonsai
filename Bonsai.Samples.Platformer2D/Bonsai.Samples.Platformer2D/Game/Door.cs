using Bonsai.Framework.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework.Audio;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Door : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IDrawable, Bonsai.Framework.ICollidable
    {
        public Door()
        {
            IsCollisionEnabled = true;
            base.Props.CollisionRect = new Rectangle(0, 0, 14, 20);
        }

        SoundEffect sfxOpen;

        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera => false;
        public bool IsHidden { get; set; }
        public bool IsCollisionEnabled { get; set; }
        public Rectangle CollisionBox => new Rectangle((int)Props.Position.X, (int)Props.Position.Y, Props.CollisionRect.Width, Props.CollisionRect.Height);


        public void Load(IContentLoader loader)
        {
            base.Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_DOOR);
            sfxOpen = loader.Load<SoundEffect>(ContentPaths.SFX_DOOR_OPEN);
        }

        public void Unload()
        {
        }


        public void Draw(GameTime time, SpriteBatch batch)
        {
            batch.Draw(Props.Texture, Props.Position, Props.CollisionRect, Props.Tint);
        }


        public void Overlapping(object actor)
        {
            if (actor is Player)
                onDoorEnteredByPlayer();
        }

        void onDoorEnteredByPlayer()
        {
            // Prevent further collisions
            IsCollisionEnabled = false;

            // Play sfx - 50% vol
            sfxOpen.Play(0.5f, 0f, 0f);

        }

    }
}
