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
    public enum CollisionDirection { Left, Top, Right, Bottom }

    public class MapPhysics
    {
        public MapPhysics(Level level)
        {
            _level = level;
        }

        Level _level;
        int tileWidth => _level.Map.TileSize.X;
        int tileHeight => _level.Map.TileSize.Y;


        public Dictionary<CollisionDirection, TileCollision> ApplyPhysics(PhysicalProperties props, GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var allCollisions = new Dictionary<CollisionDirection, TileCollision>();
            var lastEdges = getEdges(props);

            // [ Y ]

            // Move
            props.Position.Y = props.Position.Y + (props.Velocity.Y * delta);

            var newEdges = getEdges(props);
            var collisionsY = collisionCheck(props);
            mergeCollisionDictionary(allCollisions, collisionsY);

            // Jumping map collision
            if (collisionsY.ContainsKey(CollisionDirection.Top))
            {
                //project out of collision
                props.Position.Y = (lastEdges.TopIndex * tileHeight);
                props.Velocity.Y = 0;
            }
            // Falling
            else if (collisionsY.ContainsKey(CollisionDirection.Bottom))
            {
                //project out of collision
                props.Position.Y = (newEdges.BottomIndex * tileHeight) - (props.CollisionRect.Height + 1);
                props.Velocity.Y = 0;
            }

            // [ X ]

            props.Position.X = props.Position.X + (props.Velocity.X * delta);

            newEdges = getEdges(props);
            var collisionsX = collisionCheck(props);
            mergeCollisionDictionary(allCollisions, collisionsX);

            // Left movement - map collision
            if (collisionsX.ContainsKey(CollisionDirection.Left))
            {
                // Project out of collision
                props.Position.X = (lastEdges.LeftIndex * tileWidth) + 1;
                props.Velocity.X = 0;
            }
            // Right movement - map collision
            else if (collisionsX.ContainsKey(CollisionDirection.Right))
            {
                // Project out of collision
                props.Position.X = (newEdges.RightIndex * tileWidth) - (props.CollisionRect.Width + 1);
                props.Velocity.X = 0;
            }


            // [ Floor check ]

            if (!allCollisions.ContainsKey(CollisionDirection.Bottom))
            {
                if (floorCollisionCheck(props) == TileCollision.Impassable)
                    allCollisions.Add(CollisionDirection.Bottom, TileCollision.Impassable);
            }

            // Grounded flag
            var grounded = (allCollisions.ContainsKey(CollisionDirection.Bottom) &&
                            allCollisions[CollisionDirection.Bottom] == TileCollision.Impassable);
            props.Grounded = grounded;

            // [ Gravity ]
            if (!allCollisions.ContainsKey(CollisionDirection.Bottom))
            {
                if (_level.HasGravity)
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
            }


            // [ Friction ]

            if (_level.HasGravity &&
                _level.HasFriction &&
                props.Grounded &&
                props.Velocity.X != 0)
            {
                var lerped = MathHelper.Lerp(props.Velocity.X, 0, _level.Friction);
                if (Math.Abs(lerped) < 2)
                    lerped = 0;

                props.Velocity.X = lerped;
            }

            return allCollisions;
        }

        Dictionary<CollisionDirection, TileCollision> collisionCheck(PhysicalProperties props)
        {
            var collisions = new Dictionary<CollisionDirection, TileCollision>();
            var edges = getEdges(props);

            // Jumping
            if (props.Velocity.Y < 0)
            {
                var leftTopCollisionType = getCollision(edges.LeftIndex, edges.TopIndex);
                var rightTopCollisionType = getCollision(edges.RightIndex, edges.TopIndex);

                var hitRoof = (leftTopCollisionType == TileCollision.Impassable || rightTopCollisionType == TileCollision.Impassable);
                var isDeath = (leftTopCollisionType == TileCollision.Death || rightTopCollisionType == TileCollision.Death);

                if (hitRoof || isDeath)
                    collisions.Add(CollisionDirection.Top, isDeath ? TileCollision.Death : TileCollision.Impassable);
            }
            // Falling
            else if (props.Velocity.Y > 0)
            {
                var leftBottomCollisionType = getCollision(edges.LeftIndex, edges.BottomIndex);
                var rightBottomCollisionType = getCollision(edges.RightIndex, edges.BottomIndex);

                var hitFloor = (leftBottomCollisionType == TileCollision.Impassable || rightBottomCollisionType == TileCollision.Impassable);
                var isDeath = (leftBottomCollisionType == TileCollision.Death || rightBottomCollisionType == TileCollision.Death);

                if (hitFloor || isDeath)
                    collisions.Add(CollisionDirection.Bottom, isDeath ? TileCollision.Death : TileCollision.Impassable);
            }

            // Left movement
            if (props.Velocity.X < 0)
            {
                var leftTopCollisionType = getCollision(edges.LeftIndex, edges.TopIndex);
                var leftBottomCollisionType = getCollision(edges.LeftIndex, edges.BottomIndex);

                var hitLeftWall = (leftTopCollisionType == TileCollision.Impassable || leftBottomCollisionType == TileCollision.Impassable);
                var isDeath = (leftTopCollisionType == TileCollision.Death || leftBottomCollisionType == TileCollision.Death);

                if (hitLeftWall || isDeath)
                    collisions.Add(CollisionDirection.Left, isDeath ? TileCollision.Death : TileCollision.Impassable);
            }
            // Right movement collision
            else if (props.Velocity.X > 0)
            {
                var rightTopCollisionType = getCollision(edges.RightIndex, edges.TopIndex);
                var rightBottomCollisionType = getCollision(edges.RightIndex, edges.BottomIndex);

                var hitRightWall = (rightTopCollisionType == TileCollision.Impassable || rightBottomCollisionType == TileCollision.Impassable);
                var isDeath = (rightTopCollisionType == TileCollision.Death || rightBottomCollisionType == TileCollision.Death);

                if (hitRightWall || isDeath)
                    collisions.Add(CollisionDirection.Right, isDeath ? TileCollision.Death : TileCollision.Impassable);
            }

            return collisions;
        }

        TileCollision floorCollisionCheck(PhysicalProperties props)
        {
            props.Position.Y += 1;
            var edges2 = getEdges(props);
            props.Position.Y -= 1;

            var leftBottomCollisionType = getCollision(edges2.LeftIndex, edges2.BottomIndex);
            var rightBottomCollisionType = getCollision(edges2.RightIndex, edges2.BottomIndex);

            var hitFloor = (leftBottomCollisionType == TileCollision.Impassable || rightBottomCollisionType == TileCollision.Impassable);
            var isDeath = (leftBottomCollisionType == TileCollision.Death || rightBottomCollisionType == TileCollision.Death);

            if (hitFloor || isDeath)
                return isDeath ? TileCollision.Death : TileCollision.Impassable;
            else
                return TileCollision.Passable;

        }

        Map.TileEdges getEdges(PhysicalProperties props)
        {
            return _level.Map.GetEdges(props.Position, props.CollisionRect.Width, props.CollisionRect.Height);
        }

        TileCollision getCollision(int xIndex, int yIndex)
        {
            return _level.Map.GetCollision(xIndex, yIndex);
        }

        void mergeCollisionDictionary(Dictionary<CollisionDirection, TileCollision> target, Dictionary<CollisionDirection, TileCollision> source)
        {
            foreach (var kvp in source)
            {
                if (target.ContainsKey(kvp.Key))
                    target[kvp.Key] = kvp.Value;
                else
                    target.Add(kvp.Key, kvp.Value);
            }
        }

    }
}
