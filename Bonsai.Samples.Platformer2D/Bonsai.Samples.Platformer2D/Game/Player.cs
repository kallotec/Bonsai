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

namespace Bonsai.Samples.Platformer2D.Game.Actors
{
    public class Player : MoveableActor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable
    {
        public Player(Level level, Vector2 startPosition)
        {
            DrawOrder = 1;

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
        int tileWidth
        {
            get { return level.TileMap.TileSize.X; }
        }
        int tileHeight
        {
            get { return level.TileMap.TileSize.Y; }
        }

        public bool IsDisabled { get; set; }


        public void Load(IContentLoader loader)
        {
            base.Texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            base.DrawingTint = Color.Red;
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            var lastEdges = getEdges();
            var kbState = Keyboard.GetState();

            //move action
            if (kbState.IsKeyDown(Keys.Left))
                base.Velocity.X = MathHelper.Clamp((base.Velocity.X + -moveAcceleration), -moveSpeedMax, moveSpeedMax);
            else if (kbState.IsKeyDown(Keys.Right))
                base.Velocity.X = MathHelper.Clamp((base.Velocity.X - -moveAcceleration), -moveSpeedMax, moveSpeedMax);
            else
                base.Velocity.X = 0f;


            // [ Y ]

            base.Position.Y = (float)Math.Round(base.Position.Y + base.Velocity.Y); //* (float)frame.GameTime.ElapsedGameTime.TotalSeconds;
            var newEdges = getEdges();

            //jumping
            if (base.Velocity.Y < 0
                && (level.TileMap.GetCollision(newEdges.LeftIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.TopIndex) == TileCollision.Impassable))
            {
                //project out of collision
                base.Position.Y = (lastEdges.TopIndex * tileHeight);
                base.Velocity.Y = 0;
            }
            //falling
            else if (base.Velocity.Y > 0
                && (getCollision(newEdges.LeftIndex, newEdges.BottomIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.BottomIndex) == TileCollision.Impassable))
            {
                //project out of collision
                base.Position.Y = (newEdges.BottomIndex * tileHeight) - (this.collisionHeight + 1);
                base.Velocity.Y = 0;
                isJumping = false;
            }

            //jump action, only jump when landed and not already jumping
            if (base.Velocity.Y == 0 && kbState.IsKeyDown(Keys.Up) && !isJumping)
            {
                isJumping = true;
                base.Velocity.Y = -jumpAcceleration;

                // Update level variable
                level.Jumps.Value++;
            }

            //apply gravity, todo: clamp
            base.Velocity.Y = MathHelper.Clamp(base.Velocity.Y + (gravity * 1), -maxFallSpeed, maxFallSpeed);



            // [ X ]

            base.Position.X = (float)Math.Round(base.Position.X + base.Velocity.X);
            newEdges = getEdges();

            // Left movement collision
            if (base.Velocity.X < 0
                && (getCollision(newEdges.LeftIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.LeftIndex, newEdges.BottomIndex) == TileCollision.Impassable))
            {
                //project out of collision
                base.Position.X = (lastEdges.LeftIndex * tileWidth) + 1;
                base.Velocity.X = 0;
            }
            // Right movement collision
            else if (base.Velocity.X > 0
                && (getCollision(newEdges.RightIndex, newEdges.TopIndex) == TileCollision.Impassable || getCollision(newEdges.RightIndex, newEdges.BottomIndex) == TileCollision.Impassable))
            {
                //project out of collision
                base.Position.X = (newEdges.RightIndex * tileWidth) - (this.collisionWidth + 1);
                base.Velocity.X = 0;
            }

            
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            // Draw tinted box
            batch.Draw(base.Texture, base.Position, this.drawingBox, base.DrawingTint);

        }

        TileMap.TileEdges getEdges()
        {
            return level.TileMap.GetEdges(base.Position, collisionWidth, collisionHeight);
        }

        TileCollision getCollision(int xIndex, int yIndex)
        {
            return level.TileMap.GetCollision(xIndex, yIndex);
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

