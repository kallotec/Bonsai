using Bonsai.Framework;
using Bonsai.Samples.Platformer2D.Game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer.Components
{
    public class MapPhysics
    {
        public MapPhysics(Level level)
        {
            _level = level;
        }

        Level _level;
        int tileWidth
        {
            get { return _level.Map.TileSize.X; }
        }
        int tileHeight
        {
            get { return _level.Map.TileSize.Y; }
        }


        public CollisionDirection[] ApplyPhysics(PhysicalProperties props, GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var allCollisions = new List<CollisionDirection>();
            var lastEdges = getEdges(props);

            // Apply gravity

            if (_level.HasGravity && props.Velocity.Y > 0)
            {
                props.Velocity.Y = MathHelper.Clamp(
                    props.Velocity.Y + _level.Gravity,
                    -_level.TerminalVelocity,
                    _level.TerminalVelocity);
            }
            else
            {
                props.Velocity.Y = (props.Velocity.Y + _level.Gravity);
            }

            // [ Y ]

            // Move

            props.Position.Y = props.Position.Y + (props.Velocity.Y * delta);

            var newEdges = getEdges(props);
            var collisionsY = checkForCollisions(props);
            allCollisions.AddRange(collisionsY);

            // Jumping map collision
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

            props.Position.X = props.Position.X + (props.Velocity.X * delta);

            newEdges = getEdges(props);
            var collisionsX = checkForCollisions(props);
            allCollisions.AddRange(collisionsX);

            // Left movement - map collision
            if (collisionsX.Contains(CollisionDirection.Left))
            {
                //project out of collision
                props.Position.X = (lastEdges.LeftIndex * tileWidth) + 1;
                props.Velocity.X = 0;
            }
            // Right movement - map collision
            else if (collisionsX.Contains(CollisionDirection.Right))
            {
                //project out of collision
                props.Position.X = (newEdges.RightIndex * tileWidth) - (props.CollisionRect.Width + 1);
                props.Velocity.X = 0;
            }
            
            // Apply friction and slow horizxontal movement
            if (collisionsY.Contains(CollisionDirection.Bottom) && _level.HasGravity && props.Velocity.X != 0)
            {
                // TODO:
            }

            return allCollisions.ToArray();
        }

        CollisionDirection[] checkForCollisions(PhysicalProperties props)
        {
            var collisions = new List<CollisionDirection>();
            var edges = getEdges(props);

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

        Map.TileEdges getEdges(PhysicalProperties props)
        {
            return _level.Map.GetEdges(props.Position, props.CollisionRect.Width, props.CollisionRect.Height);
        }

        TileCollision getCollision(int xIndex, int yIndex)
        {
            return _level.Map.GetCollision(xIndex, yIndex);
        }

        public enum CollisionDirection { Left, Top, Right, Bottom }

    }
}
