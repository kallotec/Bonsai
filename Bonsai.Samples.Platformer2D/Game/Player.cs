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

namespace Bonsai.Samples.Platformer2D.Game.Actors
{
    public class Player : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.Physics.IPhysicsObject
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
        AnimationOverlay animCurrent;
        AnimationOverlay animStanding;
        AnimationOverlay animWalking;
        AnimationOverlay animJetting;
        SoundEffect sfxDie;
        SoundEffect sfxFire;
        KeyPressListener fireListener;
        Texture2D bulletTexture;

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
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITESHEET_MARIO);

            bulletTexture = loader.Load<Texture2D>(ContentPaths.SPRITE_BULLET);

            sfxDie = loader.Load<SoundEffect>(ContentPaths.SFX_DEATH);
            sfxFire = loader.Load<SoundEffect>(ContentPaths.SFX_FIRE);

            animStanding = new AnimationOverlay(
                name: "standing",
                animType: SpriteAnimationType.SingleFrame,
                origin: SpriteOriginType.TopLeft,
                width: Props.PhysicalRect.Width,
                height: Props.PhysicalRect.Height,
                frames: 1,
                framerateMillisec: 1,
                yOffset: 0);

            animWalking = new AnimationOverlay(
                name: "walking",
                animType: SpriteAnimationType.LoopingAnimation,
                origin: SpriteOriginType.TopLeft,
                width: Props.PhysicalRect.Width,
                height: Props.PhysicalRect.Height,
                frames: 2,
                framerateMillisec: 100,
                yOffset: 20);

            animJetting = new AnimationOverlay(
                name: "jetting",
                animType: SpriteAnimationType.SingleFrame,
                origin: SpriteOriginType.TopLeft,
                width: Props.PhysicalRect.Width,
                height: Props.PhysicalRect.Height,
                frames: 1,
                framerateMillisec: 1,
                yOffset: 40);

            // Default anim
            animCurrent = animWalking;

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
                Props.AddForceX(-acceleration * (isRunning ? runModifier : 1));
            }
            else if (kbState.IsKeyDown(Keys.D))
            {
                Props.AddForceX(acceleration * (isRunning ? runModifier : 1));
            }

            if (kbState.IsKeyDown(Keys.W))
            {
                Props.AddForceY(-acceleration * (isRunning ? runModifier : 1));
            }
            else if (kbState.IsKeyDown(Keys.S))
            {
                Props.AddForceY(acceleration * (isRunning ? runModifier : 1));
            }

            // Anims
            animCurrent.Update(time.ElapsedGameTime.Milliseconds);

            // Ensure correct anim is being used
            if (IsJetPacking)
            {
                if (animCurrent.Name != animJetting.Name)
                    animCurrent = animJetting;
            }
            else if (Props.IsGrounded)
            {
                if (Props.Velocity.X != 0 && animCurrent.Name != animWalking.Name)
                {
                    animCurrent = animWalking;
                }
                else if (Props.Velocity.X == 0 && animCurrent.Name != animStanding.Name)
                {
                    animCurrent = animStanding;
                }
            }
            else
            {
                if (animCurrent.Name != animStanding.Name)
                    animCurrent = animStanding;
            }
            
            /*
            // Jump action
            var canJump = (Props.IsGrounded && Props.Velocity.Y == 0);

            if (kbState.IsKeyDown(Keys.Space) && canJump)
            {
                Props.AddForceY(-jumpPower, overrideTopSpeed: true);

                // raise event
                eventBus.QueueNotification(Events.PlayerJumped);
            }

            // Jetpack
            if (kbState.IsKeyDown(Keys.W))
            {
                IsJetPacking = true;
                Props.AddForceY(-jetPower, overrideTopSpeed: true);
            }
            else
            {
                IsJetPacking = false;
            }
            */

            // projectiles
            fireListener.Update(time);

            var mouseState = Mouse.GetState();
            var mousePos = mouseState.Position;

            // aim
            base.Props.Rotation = Bonsai.Framework.Maths.GameMathHelper
                .GetDirectionInRadians(this.Position, (camera.CurrentFocus - BonsaiGame.Current.ScreenCenter) + new Vector2(mousePos.X, mousePos.Y));
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            // Default direction is Right
            var flip = false; // (Props.Rotation == Direction.Left);

            // Draw sprite
            batch.Draw(Props.Texture,
                       Props.Position,
                       animCurrent.DrawingRectangle,
                       Props.Tint,
                       0f,
                       animCurrent.Origin,
                       1f,
                       flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                       0f);


            var aim = Framework.Maths.GameMathHelper.PlotVector(base.Props.Rotation, 80, base.Position);

            // Draw aim
            batch.Draw(FrameworkGlobals.Pixel,
                       aim,
                       Color.Orange);
        }

        public void OnOverlapping(object actor)
        {
            if (actor == null)
                return;

            if (actor is Coin)
            {
                // play sfx
                // notify
                eventBus.QueueNotification(Events.PlayerPickedUpCoin);
                return;
            }

            if (actor is Platform)
            {
                if ((actor as Platform).IsHazardTile)
                    die();

                return;
            }

            Debug.WriteLine("Player Overlapping: " + actor.GetType().Name);
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

    }
}


