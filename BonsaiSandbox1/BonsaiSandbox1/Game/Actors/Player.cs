using Bonsai.Framework;
using Bonsai.Framework.Common;
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
using Bonsai.Framework.Physics;

namespace BonsaiSandbox1.Game.Actors
{
    public class Player : MoveableActor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Player(Level level, Vector2 startPosition)
        {
            IsVisible = true;

            collisionWidth = 20;
            collisionHeight = 20;
            drawingBox = new Rectangle(0, 0, collisionWidth, collisionHeight);

            this.level = level;
            base.Position = startPosition;
            base.DrawingTint = Color.Red;
        }

        int collisionWidth;
        int collisionHeight;
        Level level;
        float maxFallSpeed = 10f;
        float moveAcceleration = .2f;
        float moveSpeedMax = 2f;
        float gravity = 0.2f;
        float jumpAcceleration = 5f;
        bool isJumping;
        Rectangle drawingBox;

        public bool IsVisible { get; set; }


        public void Load(ContentManager content)
        {
            base.Texture = Globals.Pixel;
            base.DrawingTint = Color.Red;
        }

        public void Unload()
        {
        }

        public void Update(GameFrame frame)
        {
            //move
            //TODO: time
            base.Position.X = (float)Math.Round(base.Position.X + base.Velocity.X);
            base.Position.Y = (float)Math.Round(base.Position.Y + base.Velocity.Y); //* (float)frame.GameTime.ElapsedGameTime.TotalSeconds;

            //get all adjacent tile indexes
            var tyi = (int)base.Position.Y / Tile.Height;
            var byi = (int)(base.Position.Y + this.collisionHeight) / Tile.Height;
            var lxi = (int)base.Position.X / Tile.Width;
            var rxi = (int)(base.Position.X + this.collisionWidth) / Tile.Width;


            //// [Y axis movement]

            //jump
            if (frame.KeyboardState.IsKeyDown(Keys.Up) && !isJumping)
            {
                isJumping = true;
                base.Velocity.Y = -jumpAcceleration;
            }
            if (frame.KeyboardState.IsKeyUp(Keys.Up))
                isJumping = false;

            var jtyi = (int)(base.Position.Y + base.Velocity.Y) / Tile.Height;


            // [Y axis collision]

            //jumping - resolve on y axis
            if (base.Velocity.Y < 0 && (level.GetCollision(lxi, jtyi) == TileCollision.Impassable || level.GetCollision(rxi, jtyi) == TileCollision.Impassable))
            {
                //project out of collision
                base.Position.Y = (tyi * Tile.Height);
                base.Velocity.Y = 0;

                //update bb corners
                tyi = (int)(base.Position.Y) / Tile.Height;
                byi = (int)(base.Position.Y + this.collisionHeight) / Tile.Height;

            }

            //falling - resolve on y axis
            if (level.GetCollision(lxi, byi) == TileCollision.Impassable || level.GetCollision(rxi, byi) == TileCollision.Impassable)
            {
                if (base.Velocity.Y > 0)
                    base.Velocity.Y = 0;

                //project out of collision
                base.Position.Y = (byi * Tile.Height) - (this.collisionHeight + 1);
                base.Velocity.Y = 0;

                //update bb corners
                tyi = (int)(base.Position.Y) / Tile.Height;
                byi = (int)(base.Position.Y + this.collisionHeight) / Tile.Height;
            }
            else
            {
                //apply gravity
                //TODO: clamp
                base.Velocity.Y += gravity * 1;
            }


            // [X movement]

            //horizontal movement
            if (frame.KeyboardState.IsKeyDown(Keys.Left))
                base.Velocity.X = MathHelper.Clamp(base.Velocity.X += -moveAcceleration, -moveSpeedMax, moveSpeedMax);
            else if (frame.KeyboardState.IsKeyDown(Keys.Right))
                base.Velocity.X = MathHelper.Clamp(base.Velocity.X -= -moveAcceleration, -moveSpeedMax, moveSpeedMax);
            else
                base.Velocity.X = 0f;


            // [X collision]

            var wlxi = (int)(base.Position.X + base.Velocity.X) / Tile.Width;
            var wrxi = (int)(base.Position.X + base.Velocity.X + this.collisionWidth) / Tile.Width;

            //resolve collision on y axis
            if (level.GetCollision(wlxi, tyi) == TileCollision.Impassable || level.GetCollision(wlxi, byi) == TileCollision.Impassable)
            {
                //project out of collision
                base.Position.X = (wlxi * Tile.Width) + Tile.Width;
                base.Velocity.X = 0;


            }
            if (level.GetCollision(wrxi, tyi) == TileCollision.Impassable || level.GetCollision(wrxi, byi) == TileCollision.Impassable)
            {
                //project out of collision
                base.Position.X = (wrxi * Tile.Width) - (this.collisionWidth + 2);
                base.Velocity.X = 0;
            }


            return;
        }

        //void handleCollision()
        //{
        //    for (int y = tyIndex; y <= byIndex; ++y)
        //    {
        //        for (int x = tlIndex; x <= trIndex; ++x)
        //        {
        //            // If this tile is collidable
        //            if (level.GetCollision(x, y) == TileCollision.Impassable)
        //            {
        //                // Determine collision depth (with direction) and magnitude.
        //                var tileBounds = level.GetBounds(x, y);


        //                //doesnt intersect
        //                if (depth == Vector2.Zero)
        //                    continue;

        //                float absDepthX = Math.Abs(depth.X);
        //                float absDepthY = Math.Abs(depth.Y);

        //                if (absDepthY < absDepthX)
        //                {
        //                    //resolve collision
        //                    base.Position.Y += depth.Y;
        //                }
        //                else
        //                {
        //                    //resolve collision
        //                    base.Position.X += depth.X;
        //                }

        //            }
        //        }
        //    }

        //}

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            if (!IsVisible)
                return;

            // Draw tinted box
            batch.Draw(base.Texture, base.Position, this.drawingBox, base.DrawingTint);

        }

    }
}
