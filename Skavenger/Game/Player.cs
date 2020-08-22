﻿using Bonsai.Framework;
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

namespace Skavenger.Game
{
    public class Player : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.ICollidable
    {
        public Player(EventBus eventBus, ICamera camera)
        {
            this.eventBus = eventBus;
            this.camera = camera;

            DrawOrder = DrawOrderPosition.Foreground;

            // Physical properties
            Props.TopSpeed = 150f;
            Props.PhysicalRect = new Rectangle(0, 0, 15, 20);
            Props.Weight = 1f;
            Props.HasGravity = true;

            fireListener = new KeyPressListener(Keys.E, () => fireProjectile(), 0100);
        }

        EventBus eventBus;
        ICamera camera;
        float acceleration = 5f;
        float runModifier = 2f;
        SoundEffect sfxDie;
        SoundEffect sfxFire;
        KeyPressListener fireListener;
        Texture2D bulletTexture;
        float? lastMouseX = null;

        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public RectangleF CollisionBox => new RectangleF(Props.Position.X, Props.Position.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);
        public bool IsJetPacking { get; private set; }
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => true;


        public void Load(IContentLoader loader)
        {
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_PLAYER);
            bulletTexture = loader.Load<Texture2D>(ContentPaths.SPRITE_BULLET);
            sfxDie = loader.Load<SoundEffect>(ContentPaths.SFX_DEATH);
            sfxFire = loader.Load<SoundEffect>(ContentPaths.SFX_FIRE);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            var kbState = Keyboard.GetState();

            var isRunning = kbState.IsKeyDown(Keys.LeftShift);

            // Movement
            if (kbState.IsKeyDown(Keys.A))
            {
                var leftAngle = Props.DirectionAim + MathHelper.ToRadians(-90);
                var force = Bonsai.Framework.Maths.MathHelper.UpdateVelocity(
                    leftAngle,
                    5f);

                Props.AddForce(force);
            }
            else if (kbState.IsKeyDown(Keys.D))
            {
                var rightAngle = Props.DirectionAim + MathHelper.ToRadians(90);
                var force = Bonsai.Framework.Maths.MathHelper.UpdateVelocity(
                    rightAngle,
                    5f);

                Props.AddForce(force);
            }

            if (kbState.IsKeyDown(Keys.W))
            {
                var force = Bonsai.Framework.Maths.MathHelper.UpdateVelocity(
                    Props.DirectionAim,
                    10f);

                Props.AddForce(force);
            }
            else if (kbState.IsKeyDown(Keys.S))
            {
                var force = Bonsai.Framework.Maths.MathHelper.UpdateVelocity(
                    Props.DirectionAim,
                    -5f);

                Props.AddForce(force);
            }
            
            // projectiles
            fireListener.Update(time);

            var mouseState = Mouse.GetState();
            var mousePos = mouseState.Position;
            if (lastMouseX == null)
                lastMouseX = mousePos.X;

            var mouseHorizontalMovement = mousePos.X - lastMouseX.Value;
            lastMouseX = mousePos.X;

            // aim
            /*base.Props.DirectionAim = Bonsai.Framework.Maths.MathHelper
                .GetDirectionInRadians(this.Position, (camera.CurrentFocus - BonsaiGame.Current.ScreenCenter) + new Vector2(mousePos.X, mousePos.Y));
                */

            var minRadians = 0f;
            var maxRadians = MathHelper.ToRadians(360);

            var dir = base.Props.DirectionAim;
            
            dir = dir + (mouseHorizontalMovement * 0.01f);

            if (dir < minRadians)
                dir = maxRadians;
            else if (dir > maxRadians)
                dir = minRadians;

            base.Props.DirectionAim = dir;
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            var aim = Bonsai.Framework.Maths.MathHelper.PlotVector(base.Props.DirectionAim, 80, base.Position);

            // draw player
            batch.Draw(Props.Texture, 
                position: Props.Position, 
                sourceRectangle: null, 
                color: Props.Tint,
                rotation: base.Props.DirectionAim,
                origin: new Vector2(Props.Texture.Width / 2, Props.Texture.Height / 2),
                scale: Vector2.One,
                effects: SpriteEffects.None, 
                layerDepth: 0f);

            // Draw aim
            batch.Draw(FrameworkGlobals.Pixel,
                       aim,
                       Color.Orange);
        }

        public void OnOverlapping(object actor)
        {
            if (actor == null)
                return;

            Debug.WriteLine("Player Overlapping: " + actor.GetType().Name);

            if (actor is Coin)
            {
                eventBus.QueueNotification(Events.PlayerPickedUpCoin);
                return;
            }

        }

        void fireProjectile()
        {
            var projectile = new Projectile(base.Props.Position, base.Props.DirectionAim, 1000, bulletTexture);

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

    }
}


