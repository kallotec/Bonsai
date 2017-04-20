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
using Bonsai.Framework.Content;
using Bonsai.Samples.Platformer.Components;
using Bonsai.Framework.Animation;

namespace Bonsai.Samples.Platformer2D.Game.Actors
{
    public class Player : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.ICollidable
    {
        public Player(Level Level)
        {
            DrawOrder = 1;
            level = Level;

            // Physical properties
            Props.Direction = Direction.Right;
            Props.TopSpeed = 150f;
            Props.CollisionRect = new Rectangle(0, 0, 15, 20);
        }

        Level level;
        float jumpPower = 130f;
        float jetPower = 10f;
        float jetAcceleration = 10f;
        float acceleration = 20f;
        AnimationOverlay animCurrent;
        AnimationOverlay animStanding;
        AnimationOverlay animWalking;
        AnimationOverlay animJetting;

        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsCollisionEnabled { get; set; }
        public Rectangle CollisionBox => new Rectangle((int)Props.Position.X, (int)Props.Position.Y, Props.CollisionRect.Width, Props.CollisionRect.Height);
        public bool IsJetPacking { get; private set; }


        public void Load(IContentLoader loader)
        {
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITESHEET_MARIO);

            animStanding = new AnimationOverlay(
                name: "standing",
                animType: SpriteAnimationType.SingleFrame,
                origin: SpriteOriginType.TopLeft,
                width: Props.CollisionRect.Width,
                height: Props.CollisionRect.Height,
                frames: 1,
                framerateMillisec: 1,
                yOffset: 0);

            animWalking = new AnimationOverlay(
                name: "walking",
                animType: SpriteAnimationType.LoopingAnimation,
                origin: SpriteOriginType.TopLeft,
                width: Props.CollisionRect.Width,
                height: Props.CollisionRect.Height,
                frames: 2,
                framerateMillisec: 100,
                yOffset: 20);

            animJetting = new AnimationOverlay(
                name: "jetting",
                animType: SpriteAnimationType.SingleFrame,
                origin: SpriteOriginType.TopLeft,
                width: Props.CollisionRect.Width,
                height: Props.CollisionRect.Height,
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
            if (kbState.IsKeyDown(Keys.Left))
            {
                if (Props.Grounded)
                    Props.AddForceX(-acceleration);
                else if (IsJetPacking)
                    Props.AddForceX(-jetAcceleration);
            }
            else if (kbState.IsKeyDown(Keys.Right))
            {
                if (Props.Grounded)
                    Props.AddForceX(acceleration);
                else if (IsJetPacking)
                    Props.AddForceX(jetAcceleration);
            }

            // Anims
            animWalking.Update(time.ElapsedGameTime.Milliseconds);

            // Ensure correct anim is being used
            if (IsJetPacking)
            {
                if (animCurrent.Name != animJetting.Name)
                    animCurrent = animJetting;
            }
            else if (Props.Grounded)
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
            var canJump = (Props.Grounded && Props.Velocity.Y == 0);

            if (kbState.IsKeyDown(Keys.Up) && canJump)
            {
                Props.AddForceY(-jumpPower, overrideTopSpeed: true);

                // Update level variable
                level.Jumps.Value++;
            }

            // Jetpack
            if (kbState.IsKeyDown(Keys.Space))
            {
                IsJetPacking = true;
                Props.AddForceY(-jetPower, overrideTopSpeed: true);
            }
            else
            {
                IsJetPacking = false;
            }

        }


        public void Draw(GameTime time, SpriteBatch batch)
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

            // Draw tinted box
            // batch.Draw(Props.Texture, Props.Position, , Props.DrawingTint.Value);
        }


        public void OnOverlapping(object actor)
        {
            if (actor == null)
                return;

            Debug.WriteLine("Player Overlapping: " + actor.GetType().Name);
        }

    }
}


