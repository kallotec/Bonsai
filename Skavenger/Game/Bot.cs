using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Bonsai.Framework.Collision;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Animation;
using Microsoft.Xna.Framework.Audio;
using Bonsai.Framework.Input;
using Bonsai.Framework.Maths;

namespace Skavenger.Game
{
    public class Bot : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.Physics.IPhysicsObject
    {
        public Bot(EventBus eventBus, ICamera camera)
        {
            this.eventBus = eventBus;

            DrawOrder = DrawOrderPosition.Foreground;

            // Physical properties
            Props.TopSpeed = 150f;
            Props.PhysicalRect = new Rectangle(0, 0, 15, 20);
            Props.Weight = 1f;
            Props.HasGravity = true;

            allWaypoints = new List<Vector2>();
            waypointsQueue = new Queue<Vector2>();
        }

        List<Vector2> allWaypoints;
        Queue<Vector2> waypointsQueue;
        EventBus eventBus;
        SoundEffect sfxDie;
        SoundEffect sfxFire;
        Texture2D bulletTexture;
        Vector2 movingTo;

        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public RectangleF CollisionBox => new RectangleF(Props.Position.X, Props.Position.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);
        public RectangleF OverlapBox => CollisionBox;
        public bool IsJetPacking { get; private set; }
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => true;

        public void Load(IContentLoader loader)
        {
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_PLAYER);
            bulletTexture = loader.Load<Texture2D>(ContentPaths.SPRITE_BULLET);
            sfxDie = loader.Load<SoundEffect>(ContentPaths.SFX_DEATH);
            sfxFire = loader.Load<SoundEffect>(ContentPaths.SFX_FIRE);

            allWaypoints = new List<Vector2>
            {
                new Vector2(50,50),
                new Vector2(400,50),
            };
            waypointsQueue = new Queue<Vector2>(allWaypoints);
            movingTo = waypointsQueue.Dequeue();
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            var kbState = Keyboard.GetState();

            // move to point
            var isAtPoint = Math.Abs(GameMathHelper.CalculateDistanceBetween(Props.Position, movingTo)) < 2f;

            if (!isAtPoint)
            {
                // look at point
                Props.Rotation = GameMathHelper.GetDirectionInRadians(Props.Position, movingTo);

                var force = GameMathHelper.UpdateVelocity(
                    Props.Rotation,
                    5f);

                Props.AddForce(force);
            } 
            else
            {
                if (waypointsQueue.Count == 0)
                {
                    allWaypoints.Reverse();
                    waypointsQueue = new Queue<Vector2>(allWaypoints);
                }

                movingTo = waypointsQueue.Dequeue();
            }

        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            var aim = Bonsai.Framework.Maths.GameMathHelper.PlotVector(base.Props.Rotation, 80, base.Position);

            // draw player
            batch.Draw(Props.Texture, 
                position: Props.Position, 
                sourceRectangle: null, 
                color: Props.Tint,
                rotation: base.Props.Rotation,
                origin: new Vector2(Props.Texture.Width / 2, Props.Texture.Height / 2),
                scale: Vector2.One,
                effects: SpriteEffects.None, 
                layerDepth: 0f);

            // Draw aim
            batch.Draw(FrameworkGlobals.Pixel,
                       aim,
                       Color.Orange);
        }

        public void OnCollision(object actor)
        {
        }

        void fireProjectile()
        {
            var projectile = new Projectile(base.Props.Position, base.Props.Rotation, 1000, bulletTexture);

            eventBus.QueueNotification(Events.CreateProjectile, projectile);

            // sfx
            sfxFire.Play(1f, 0f, 0f);
        }

        void die()
        {
            // stop any residual movement
            Props.Velocity = Vector2.Zero;

            // play sfx
            sfxDie.Play(1f, 0f, 0f);

            // notify
            eventBus.QueueNotification(Events.PlayerDied);
        }

        public void OnOverlapping(object actor)
        {
        }

    }
}


