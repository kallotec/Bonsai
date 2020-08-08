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

namespace Bonsai.Samples.Platformer2D.Game.Actors
{
    public class Player : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.ICollidable
    {
        public Player(EventBus eventBus)
        {
            this.eventBus = eventBus;

            DrawOrder = DrawOrderPosition.Foreground;

            // Physical properties
            Props.Direction = Direction.Right;
            Props.TopSpeed = 150f;
            Props.PhysicalRect = new Rectangle(0, 0, 15, 20);
        }

        EventBus eventBus;
        float jumpPower = 130f;
        float jetPower = 10f;
        float jetAcceleration = 10f;
        float acceleration = 20f;
        AnimationOverlay animCurrent;
        AnimationOverlay animStanding;
        AnimationOverlay animWalking;
        AnimationOverlay animJetting;
        SoundEffect sfxDie;

        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public Rectangle CollisionBox => new Rectangle((int)Props.Position.X, (int)Props.Position.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);
        public bool IsJetPacking { get; private set; }
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => true;


        public void Load(IContentLoader loader)
        {
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITESHEET_MARIO);

            sfxDie = loader.Load<SoundEffect>(ContentPaths.SFX_DEATH);

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

            // Movement
            if (kbState.IsKeyDown(Keys.A))
            {
                if (Props.IsGrounded)
                    Props.AddForceX(-acceleration);
                else if (IsJetPacking)
                    Props.AddForceX(-jetAcceleration);
                else
                    Props.AddForceX(-(acceleration / 2));
            }
            else if (kbState.IsKeyDown(Keys.D))
            {
                if (Props.IsGrounded)
                    Props.AddForceX(acceleration);
                else if (IsJetPacking)
                    Props.AddForceX(jetAcceleration);
                else
                    Props.AddForceX((acceleration / 2));
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
            

            // Jump action
            var canJump = (Props.IsGrounded && Props.Velocity.Y == 0);

            if (kbState.IsKeyDown(Keys.Space) && canJump)
            {
                Props.AddForceY(-jumpPower, overrideTopSpeed: true);

                // raise event
                eventBus.QueueNotification("playerJumped");
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

        }


        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            // Default direction is Right
            var flip = (Props.Direction == Direction.Left);

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

        }


        public void OnOverlapping(object actor)
        {
            if (actor == null)
                return;

            if (actor is Coin)
            {
                // play sfx
                // notify
                eventBus.QueueNotification("playerPickedUpCoin");
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

        void die()
        {
            // stop any residual movement
            Props.Velocity = Vector2.Zero;

            // play sfx
            sfxDie.Play(1f, 0f, 0f);

            // notify
            eventBus.QueueNotification("playerDied");
        }

    }
}


