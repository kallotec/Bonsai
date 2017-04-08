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
using Bonsai.Framework.Components;
using Bonsai.Samples.Platformer.Components;

namespace Bonsai.Samples.Platformer2D.Game.Actors
{
    public class Player : GameEntity, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Player(Level Level, Vector2 StartPosition)
        {
            DrawOrder = 1;
            collisionWidth = 20;
            collisionHeight = 20;
            drawingBox = new Rectangle(0, 0, collisionWidth, collisionHeight);

            level = Level;

            props = new PhysicalProperties
            {
                Position = StartPosition,
                DrawingTint = Color.Red,
                Acceleration = 20f,
                TopSpeed = 150f,
                Gravity = 5f,
                TerminalVelocity = 200f,
                JumpPower = 180f,
                CollisionRect = new Rectangle(0, 0, collisionWidth, collisionHeight)
            };

            collision = new CollisionDetector(level);
        }

        int collisionWidth;
        int collisionHeight;
        Level level;
        bool isJumping;
        Rectangle drawingBox;
        int tileWidth
        {
            get { return level.TileMap.TileSize.X; }
        }
        int tileHeight
        {
            get { return level.TileMap.TileSize.Y; }
        }
        PhysicalProperties props;
        CollisionDetector collision;

        public override Vector2 Position { get { return props.Position; } } //HACK:
        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }


        public void Load(IContentLoader loader)
        {
            props.Texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            props.DrawingTint = Color.Red;
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            var kbState = Keyboard.GetState();

            // Do physics
            var collisions = collision.ApplyPhysics(props, time);

            // Handle collisions
            // Landing
            if (collisions.Contains(CollisionDetector.CollisionDirection.Bottom))
            {
                isJumping = false;
            }

            // Move left/right
            if (kbState.IsKeyDown(Keys.Left))
            {
                props.AddForceX(-props.Acceleration);
            }
            else if (kbState.IsKeyDown(Keys.Right))
            {
                props.AddForceX(props.Acceleration);
            }
            else
            {
                props.Velocity.X = 0;
            }

            //Console.WriteLine(props.Velocity.X);
            //Console.WriteLine(props.Velocity.Y);
            //Console.WriteLine(props.Position.Y);
            //Console.WriteLine(time.ElapsedGameTime.TotalSeconds);

            // Jump action
            var isOnGround = (props.Velocity.Y == 0 && !isJumping);
            if (kbState.IsKeyDown(Keys.Up) && isOnGround)
            {
                isJumping = true;

                props.AddForceY(-props.JumpPower, overrideTopSpeed: true);

                // Update level variable
                level.Jumps.Value++;
            }

        }


        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Draw tinted box
            batch.Draw(props.Texture, props.Position, this.drawingBox, props.DrawingTint.Value);
        }

    }
}


