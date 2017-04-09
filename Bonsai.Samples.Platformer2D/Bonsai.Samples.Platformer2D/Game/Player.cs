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
        float jumpPower = 180f;
        float acceleration = 20f;

        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsCollisionEnabled { get; set; }
        public Rectangle CollisionBox => new Rectangle((int)Props.Position.X, (int)Props.Position.Y, Props.CollisionRect.Width, Props.CollisionRect.Height);


        public void Load(IContentLoader loader)
        {
            Props.Texture = loader.Load<Texture2D>(ContentPaths.SPRITE_MARIO);
        }

        public void Unload()
        {
        }


        public void Update(GameTime time)
        {
            var kbState = Keyboard.GetState();

            // Move left/right
            if (kbState.IsKeyDown(Keys.Left))
            {
                Props.AddForceX(-acceleration);
            }
            else if (kbState.IsKeyDown(Keys.Right))
            {
                Props.AddForceX(acceleration);
            }
            else
            {
                // TODO: Deceleration
                Props.Velocity.X = 0;
            }


            // Jump action
            var canJump = (Props.Grounded && Props.Velocity.Y == 0);

            if (kbState.IsKeyDown(Keys.Up) && canJump)
            {
                Props.AddForceY(-jumpPower, overrideTopSpeed: true);

                // Update level variable
                level.Jumps.Value++;
            }

        }


        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Default direction is Right
            var flip = (Props.Direction == Direction.Left);

            batch.Draw(Props.Texture, Props.Position, Props.CollisionRect, Props.Tint, 0f, new Vector2(), 1f, (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);

            // Draw tinted box
            // batch.Draw(Props.Texture, Props.Position, , Props.DrawingTint.Value);
        }


        public void Overlapping(object actor)
        {
            if (actor == null)
                return;

            Debug.WriteLine("Player Overlapping: " + actor.GetType().Name);
        }

    }
}


