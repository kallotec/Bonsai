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
using Skavenger.Game.Loot;
using Bonsai.Framework.Variables;

namespace Skavenger.Game
{
    public class Player : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.Physics.IPhysicsObject
    {
        public Player(EventBus eventBus, GameVariable<bool> varCanOpenBox)
        {
            this.eventBus = eventBus;
            this.canOpenBox = varCanOpenBox;

            DrawOrder = DrawOrderPosition.Foreground;

            // Physical properties
            Props.TopSpeed = 150f;
            Props.PhysicalRect = new Rectangle(0, 0, 15, 20);
            Props.Weight = 1f;
            Props.HasGravity = true;

            fireListener = new KeyPressListener(Keys.Space, () => fireProjectile(), 0100);
        }

        EventBus eventBus;
        SoundEffect sfxDie;
        SoundEffect sfxFire;
        KeyPressListener fireListener;
        Texture2D bulletTexture;
        float? lastMouseX = null;
        GameVariable<bool> canOpenBox;

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
                var leftAngle = Props.Rotation + MathHelper.ToRadians(-90);
                var force = Bonsai.Framework.Maths.GameMathHelper.UpdateVelocity(
                    leftAngle,
                    5f);

                Props.AddForce(force);
            }
            else if (kbState.IsKeyDown(Keys.D))
            {
                var rightAngle = Props.Rotation + MathHelper.ToRadians(90);
                var force = Bonsai.Framework.Maths.GameMathHelper.UpdateVelocity(
                    rightAngle,
                    5f);

                Props.AddForce(force);
            }

            if (kbState.IsKeyDown(Keys.W))
            {
                var force = Bonsai.Framework.Maths.GameMathHelper.UpdateVelocity(
                    Props.Rotation,
                    10f * (isRunning ? 1.5f : 1f));

                Props.AddForce(force);
            }
            else if (kbState.IsKeyDown(Keys.S))
            {
                var force = Bonsai.Framework.Maths.GameMathHelper.UpdateVelocity(
                    Props.Rotation,
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

            var dir = base.Props.Rotation;
            
            dir = dir + (mouseHorizontalMovement * 0.01f);

            if (dir < minRadians)
                dir = maxRadians;
            else if (dir > maxRadians)
                dir = minRadians;

            base.Props.Rotation = dir;

            // show tip
            canOpenBox.Value = Props.OverlappingObjects.OfType<LootBox>().Any();
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

        public void OnOverlapping(object actor)
        {
            if (actor == null)
                return;

            Debug.WriteLine("Player Overlapping: " + actor.GetType().Name);

            if (actor is LootBox)
            {
                if (!canOpenBox.Value)
                {
                    canOpenBox.Value = true;
                }

                var lootBox = actor as LootBox;

                var kbState = Keyboard.GetState();
                if (kbState.IsKeyDown(Keys.E))
                {
                    var items = lootBox.OpenBox();
                }
            }
            
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

    }
}


