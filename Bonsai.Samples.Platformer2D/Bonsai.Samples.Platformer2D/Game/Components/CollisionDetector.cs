﻿using Bonsai.Framework.Components;
using Bonsai.Samples.Platformer2D.Game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer.Components
{
    public class CollisionDetector
    {
        public CollisionDetector(Level level)
        {
            _level = level;
        }

        Level _level;
        int tileWidth
        {
            get { return _level.TileMap.TileSize.X; }
        }
        int tileHeight
        {
            get { return _level.TileMap.TileSize.Y; }
        }


        public CollisionDirection[] ApplyPhysics(PhysicalProperties props, float delta)
        {
            var allCollisions = new List<CollisionDirection>();
            var lastEdges = GetEdges(props);

            // [ Y ]

            // Apply gravity
            props.Velocity.Y = MathHelper.Clamp(props.Velocity.Y + (props.Gravity * 1), -props.TerminalVelocity, props.TerminalVelocity);
            props.Position.Y = (float)Math.Round(props.Position.Y + props.Velocity.Y); // * (float)frame.GameTime.ElapsedGameTime.TotalSeconds;

            var newEdges = GetEdges(props);
            var collisionsY = CheckForCollisions(props);
            allCollisions.AddRange(collisionsY);

            // Jumping
            if (collisionsY.Contains(CollisionDirection.Top))
            {
                //project out of collision
                props.Position.Y = (lastEdges.TopIndex * tileHeight);
                props.Velocity.Y = 0;
            }
            // Falling
            else if (collisionsY.Contains(CollisionDirection.Bottom))
            {
                //project out of collision
                props.Position.Y = (newEdges.BottomIndex * tileHeight) - (props.CollisionRect.Height + 1);
                props.Velocity.Y = 0;
            }

            // [ X ]

            props.Position.X = (float)Math.Round(props.Position.X + props.Velocity.X);
            newEdges = GetEdges(props);
            var collisionsX = CheckForCollisions(props);
            allCollisions.AddRange(collisionsX);

            // Left movement collision
            if (collisionsX.Contains(CollisionDirection.Left))
            {
                //project out of collision
                props.Position.X = (lastEdges.LeftIndex * tileWidth) + 1;
                props.Velocity.X = 0;
            }
            // Right movement collision
            else if (collisionsX.Contains(CollisionDirection.Right))
            {
                //project out of collision
                props.Position.X = (newEdges.RightIndex * tileWidth) - (props.CollisionRect.Width + 1);
                props.Velocity.X = 0;
            }


            return allCollisions.ToArray();
        }

        public CollisionDirection[] CheckForCollisions(PhysicalProperties props)
        {
            var collisions = new List<CollisionDirection>();
            var edges = GetEdges(props);

            // Jumping
            if (props.Velocity.Y < 0)
            {
                var hitRoof = (getCollision(edges.LeftIndex, edges.TopIndex) == TileCollision.Impassable || getCollision(edges.RightIndex, edges.TopIndex) == TileCollision.Impassable);
                if (hitRoof)
                    collisions.Add(CollisionDirection.Top);
            }
            // Falling
            else if (props.Velocity.Y > 0)
            {
                var hitFloor = (getCollision(edges.LeftIndex, edges.BottomIndex) == TileCollision.Impassable || getCollision(edges.RightIndex, edges.BottomIndex) == TileCollision.Impassable);
                if (hitFloor)
                    collisions.Add(CollisionDirection.Bottom);
            }

            // Left movement
            if (props.Velocity.X < 0)
            {
                var hitLeftWall = (getCollision(edges.LeftIndex, edges.TopIndex) == TileCollision.Impassable || getCollision(edges.LeftIndex, edges.BottomIndex) == TileCollision.Impassable);
                if (hitLeftWall)
                    collisions.Add(CollisionDirection.Left);
            }
            // Right movement collision
            else if (props.Velocity.X > 0)
            {
                var hitRightWall = (getCollision(edges.RightIndex, edges.TopIndex) == TileCollision.Impassable || getCollision(edges.RightIndex, edges.BottomIndex) == TileCollision.Impassable);
                if (hitRightWall)
                    collisions.Add(CollisionDirection.Right);
            }

            return collisions.ToArray();
        }

        public TileMap.TileEdges GetEdges(PhysicalProperties props)
        {
            return _level.TileMap.GetEdges(props.Position, props.CollisionRect.Width, props.CollisionRect.Height);
        }

        TileCollision getCollision(int xIndex, int yIndex)
        {
            return _level.TileMap.GetCollision(xIndex, yIndex);
        }

        public enum CollisionDirection { Left, Top, Right, Bottom }

    }
}
