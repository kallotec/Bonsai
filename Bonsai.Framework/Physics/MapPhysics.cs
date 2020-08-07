using Bonsai.Framework;
using Bonsai.Framework.Chunks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public class MapPhysics
    {
        public MapPhysics(ChunkMap chunkMap, PhysicsSettings physSettings = null)
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
                var intersections = neighbours.Where(n => entity.CollisionBox.Intersects(n.CollisionBox));
                if (intersections.Any())
                {
                    foreach (var intersection in intersections)
                    {
                        intersection.OnOverlapping(entity);
                        entity.OnOverlapping(intersection);
                    }

                    if (entity.IsCollisionEnabled)
                    {
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

            // [ X ]

            entityProps.Position.X = entityProps.Position.X + (entityProps.Velocity.X * delta);

            if (entity.IsOverlappingEnabled)
            {
                neighbours = chunkMap.GetNearbyCollidables(entity);
                var intersections = neighbours.Where(n => entity.CollisionBox.Intersects(n.CollisionBox));
                if (intersections.Any())
                {
                    foreach (var intersection in intersections)
                    {
                        intersection.OnOverlapping(entity);
                        entity.OnOverlapping(intersection);
                    }

                    if (entity.IsCollisionEnabled)
                    {
                        entityProps.Velocity.X = 0;
                        entityProps.Position.X = lastPos.X;
                    }
                }
            }

            // [ Grounded flag ]

            if (entity.IsCollisionEnabled)
            {
                var groundedArea = new Rectangle(
                    entity.CollisionBox.X + (entity.CollisionBox.Width / 2), 
                    entity.CollisionBox.Y + entity.CollisionBox.Height, 
                    1, 1);

                var groundingIntersections = neighbours.Where(n => groundedArea.Intersects(n.CollisionBox));

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

            if (!entityProps.IsGrounded)
            {
                if (physSettings.HasGravity)
                {
                    entityProps.Velocity.Y = MathHelper.Clamp(
                        entityProps.Velocity.Y + physSettings.Gravity,
                        -physSettings.TerminalVelocity,
                        physSettings.TerminalVelocity);
                }
                else
                {
                    entityProps.Velocity.Y = (entityProps.Velocity.Y + physSettings.Gravity);
                }
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

            // [ Chunk map update ]

            chunkMap.UpdateEntity(entity);

        }

    }
}
