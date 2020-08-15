using Bonsai.Framework;
using Bonsai.Framework.Chunks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public class MapPhysics
    {
        public MapPhysics(ChunkMap chunkMap, PhysicsSettings physSettings)
        {
            this.chunkMap = chunkMap;
            this.physSettings = physSettings ?? DefaultSettings;
        }

        public static PhysicsSettings DefaultSettings = new PhysicsSettings
        {
            Gravity = 5f,
            Friction = 0.1f,
            TerminalVelocity = 200f,
        };

        ChunkMap chunkMap;
        PhysicsSettings physSettings;


        public void ApplyPhysics(PhysicalProperties entityProps, ICollidable entity, GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var lastPos = entityProps.Position;
            var neighbours = new ICollidable[0];

            // [ Y ]

            entityProps.Position.Y += (entityProps.Velocity.Y * delta);

            if (entity.IsOverlappingEnabled)
            {
                neighbours = chunkMap.GetNearbyCollidables(entity);
                var intersections = neighbours.Where(n => entity.CollisionBox.IntersectsWith(n.CollisionBox));
                if (intersections.Any())
                {
                    var collided = false;

                    foreach (var intersection in intersections)
                    {
                        intersection.OnOverlapping(entity);
                        entity.OnOverlapping(intersection);

                        if (!collided && entity.IsCollisionEnabled && intersection.IsCollisionEnabled)
                        {
                            collided = true;

                            var up = entityProps.Velocity.Y < 0;
                            entityProps.Velocity.Y = 0;

                            var collision = intersections.First().CollisionBox;

                            if (up)
                                entityProps.Position.Y = collision.Bottom + 1;
                            else
                                entityProps.Position.Y = collision.Top - entity.CollisionBox.Height;

                        }
                    }

                }
            }

            // [ X ]

            entityProps.Position.X = entityProps.Position.X + (entityProps.Velocity.X * delta);

            if (entity.IsOverlappingEnabled)
            {
                neighbours = chunkMap.GetNearbyCollidables(entity);

                var intersections = neighbours.Where(n => entity.CollisionBox.IntersectsWith(n.CollisionBox));
                if (intersections.Any())
                {
                    var collided = false;

                    foreach (var intersection in intersections)
                    {
                        intersection.OnOverlapping(entity);
                        entity.OnOverlapping(intersection);

                        if (!collided && entity.IsCollisionEnabled && intersection.IsCollisionEnabled)
                        {
                            collided = true;

                            entityProps.Velocity.X = 0;
                            entityProps.Position.X = lastPos.X;
                        }

                    }

                }
            }

            switch (physSettings.PhysicsType)
            {
                case PhysicsType.Platformer:
                    handlePlatformerAdditionalPhysics(entityProps, entity, neighbours, gameTime);
                    break;

                case PhysicsType.Topdown:
                    handleTopDownAdditionalPhysics(entityProps, entity, neighbours, gameTime);
                    break;
            }

            // [ Chunk map update ]

            chunkMap.UpdateEntity(entity);

        }

        void handlePlatformerAdditionalPhysics(PhysicalProperties entityProps, ICollidable entity, ICollidable[] neighbours, GameTime gameTime)
        {
            // [ Grounded flag ]

            if (physSettings.HasGravity && entity.IsCollisionEnabled)
            {
                var groundedArea = new RectangleF(
                    entity.CollisionBox.X + (entity.CollisionBox.Width / 2),
                    entity.CollisionBox.Y + entity.CollisionBox.Height,
                    1, 1);

                var groundingIntersections = neighbours.Where(n => groundedArea.IntersectsWith(n.CollisionBox));

                var triggerOverlapEvents = false;

                if (!entityProps.IsGrounded && groundingIntersections.Any())
                {
                    entityProps.IsGrounded = true;
                    triggerOverlapEvents = true;
                }
                else if (entityProps.IsGrounded && !groundingIntersections.Any())
                {
                    entityProps.IsGrounded = false;
                    triggerOverlapEvents = true;
                }

                if (triggerOverlapEvents)
                {
                    foreach (var groundings in groundingIntersections)
                    {
                        groundings.OnOverlapping(entity);
                        entity.OnOverlapping(groundings);
                    }
                }

            }

            // [ Gravity ]

            if (physSettings.HasGravity && entityProps.HasGravity && !entityProps.IsGrounded)
            {
                // factors weight into equation
                entityProps.Velocity.Y = MathHelper.Clamp(
                    entityProps.Velocity.Y + (physSettings.Gravity * entityProps.Weight),
                    -physSettings.TerminalVelocity,
                    physSettings.TerminalVelocity);
            }

            // [ Friction ]

            if (physSettings.HasGravity &&
                physSettings.HasFriction &&
                entityProps.IsGrounded &&
                entityProps.Velocity.X != 0)
            {
                var lerped = MathHelper.Lerp(entityProps.Velocity.X, 0, physSettings.Friction);
                if (Math.Abs(lerped) < 2)
                    lerped = 0;

                entityProps.Velocity.X = lerped;
            }

        }

        void handleTopDownAdditionalPhysics(PhysicalProperties entityProps, ICollidable entity, ICollidable[] neighbours, GameTime gameTime)
        {
            // Gravity

            if (entityProps.HasGravity)
            {
                if (entityProps.Velocity.X != 0)
                {
                    var lerped = MathHelper.Lerp(entityProps.Velocity.X, 0, physSettings.Friction);

                    if (Math.Abs(lerped) < 2)
                        lerped = 0;

                    entityProps.Velocity.X = lerped;
                }

                if (entityProps.Velocity.Y != 0)
                {
                    var lerped = MathHelper.Lerp(entityProps.Velocity.Y, 0, physSettings.Friction);

                    if (Math.Abs(lerped) < 2)
                        lerped = 0;

                    entityProps.Velocity.Y = lerped;
                }
            }

        }



    }
}
