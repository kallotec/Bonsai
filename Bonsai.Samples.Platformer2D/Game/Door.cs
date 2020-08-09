using Bonsai.Framework.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework.Audio;
using Bonsai.Framework;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Door : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IDrawable, Bonsai.Framework.ICollidable
    {
        public Door(EventBus eventBus)
        {
            this.eventBus = eventBus;

            IsCollisionEnabled = true;
            base.Props.PhysicalRect = new Rectangle(0, 0, 14, 20);
        }

        EventBus eventBus;
        SoundEffect sfxOpen;

        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera => false;
        public bool IsHidden { get; set; }
        public bool IsCollisionEnabled { get; set; }
        public RectangleF CollisionBox => new RectangleF(Props.Position.X, Props.Position.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);

        public bool IsOverlappingEnabled => throw new NotImplementedException();

        public void Load(IContentLoader loader)
        {
            base.Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_DOOR);
            sfxOpen = loader.Load<SoundEffect>(ContentPaths.SFX_DOOR_OPEN);
        }

        public void Unload()
        {
        }


        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(Props.Texture, Props.Position, Props.PhysicalRect, Props.Tint);
        }


        public void OnOverlapping(object actor)
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

            eventBus.QueueNotification(Events.PlayerEnteredDoor);

        }

    }
}
