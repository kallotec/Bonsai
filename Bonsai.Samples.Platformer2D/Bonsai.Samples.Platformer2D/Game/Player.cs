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

            physical = new PhysicalProperties
            {
                TerminalVelocity = 10f,
                Position = StartPosition,
                DrawingTint = Color.Red,
                Gravity = 0.2f,
                CollisionRect = new Rectangle(0, 0, collisionWidth, collisionHeight)
            };

            collision = new CollisionDetector(level);
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
        int tileWidth
        {
            get { return level.TileMap.TileSize.X; }
        }
        int tileHeight
        {
            get { return level.TileMap.TileSize.Y; }
        }
        PhysicalProperties physical;
        CollisionDetector collision;
        
        public override Vector2 Position { get { return physical.Position; } } //HACK:
        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public bool IsDisabled { get; set; }


        public void Load(IContentLoader loader)
        {
            physical.Texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            physical.DrawingTint = Color.Red;
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            var kbState = Keyboard.GetState();

            // Do physics
            var collisions = collision.ApplyPhysics(physical, (float)time.ElapsedGameTime.TotalMilliseconds);

            // Handle collisions
            // Landing
            if (collisions.Contains(CollisionDetector.CollisionDirection.Bottom))
            {
                isJumping = false;
            }

            // Move left/right
            if (kbState.IsKeyDown(Keys.Left))
                physical.Velocity.X = MathHelper.Clamp((physical.Velocity.X + -moveAcceleration), -moveSpeedMax, moveSpeedMax);
            else if (kbState.IsKeyDown(Keys.Right))
                physical.Velocity.X = MathHelper.Clamp((physical.Velocity.X - -moveAcceleration), -moveSpeedMax, moveSpeedMax);
            else
                physical.Velocity.X = 0f;

            // Jump action, only jump when landed and not already jumping
            if (physical.Velocity.Y == 0 && kbState.IsKeyDown(Keys.Up) && !isJumping)
            {
                isJumping = true;
                physical.Velocity.Y = -jumpAcceleration;

                // Update level variable
                level.Jumps.Value++;
            }

        }


        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Draw tinted box
            batch.Draw(physical.Texture, physical.Position, this.drawingBox, physical.DrawingTint.Value);

        }

    }
}


//public void Update(GameTime time)
//{
//    var lastEdges = getEdges();
//    var kbState = Keyboard.GetState();

//    // [ Y ]

//    base.Position.Y = (float)Math.Round(base.Position.Y + base.Velocity.Y); //* (float)frame.GameTime.ElapsedGameTime.TotalSeconds;
//    var newEdges = getEdges();

//    //jumping
//    if (base.Velocity.Y < 0
//        && (level.TileMap.GetCollision(newEdges.LeftIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.TopIndex) == TileCollision.Impassable))
//    {
//        //project out of collision
//        base.Position.Y = (lastEdges.TopIndex * tileHeight);
//        base.Velocity.Y = 0;
//    }
//    //falling
//    else if (base.Velocity.Y > 0
//        && (getCollision(newEdges.LeftIndex, newEdges.BottomIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.BottomIndex) == TileCollision.Impassable))
//    {
//        //project out of collision
//        base.Position.Y = (newEdges.BottomIndex * tileHeight) - (this.collisionHeight + 1);
//        base.Velocity.Y = 0;
//        isJumping = false;
//    }

//    //jump action, only jump when landed and not already jumping
//    if (base.Velocity.Y == 0 && kbState.IsKeyDown(Keys.Up) && !isJumping)
//    {
//        isJumping = true;
//        base.Velocity.Y = -jumpAcceleration;

//        // Update level variable
//        level.Jumps.Value++;
//    }

//    //apply gravity, todo: clamp
//    base.Velocity.Y = MathHelper.Clamp(base.Velocity.Y + (gravity * 1), -maxFallSpeed, maxFallSpeed);



//    // [ X ]

//    base.Position.X = (float)Math.Round(base.Position.X + base.Velocity.X);
//    newEdges = getEdges();

//    // Left movement collision
//    if (base.Velocity.X < 0
//        && (getCollision(newEdges.LeftIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.LeftIndex, newEdges.BottomIndex) == TileCollision.Impassable))
//    {
//        //project out of collision
//        base.Position.X = (lastEdges.LeftIndex * tileWidth) + 1;
//        base.Velocity.X = 0;
//    }
//    // Right movement collision
//    else if (base.Velocity.X > 0
//        && (getCollision(newEdges.RightIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.BottomIndex) == TileCollision.Impassable))
//    {
//        //project out of collision
//        base.Position.X = (newEdges.RightIndex * tileWidth) - (this.collisionWidth + 1);
//        base.Velocity.X = 0;
//    }

//    //move action
//    if (kbState.IsKeyDown(Keys.Left))
//        base.Velocity.X = MathHelper.Clamp((base.Velocity.X + -moveAcceleration), -moveSpeedMax, moveSpeedMax);
//    else if (kbState.IsKeyDown(Keys.Right))
//        base.Velocity.X = MathHelper.Clamp((base.Velocity.X - -moveAcceleration), -moveSpeedMax, moveSpeedMax);
//    else
//        base.Velocity.X = 0f;

//}

