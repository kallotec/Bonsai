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


        public void ApplyPhysics(PhysicalProperties entityProps, IPhysicsObject entity, GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var lastPos = entityProps.Position;
            var neighbours = new IPhysicsObject[0];

            var overlappingObjs = new List<IPhysicsObject>();

            // [ Y ]

            entityProps.Position.Y += (entityProps.Velocity.Y * delta);

            // get new neighbours since objects have probably moved
            neighbours = chunkMap.GetNearbyCollidables(entity);

            // overlaps
            if (entity.IsOverlappingEnabled)
            {
                var overlaps = neighbours.Where(n => n.IsOverlappingEnabled && entity.OverlapBox.IntersectsWith(n.OverlapBox));

                foreach (var overlap in overlaps)
                {
                    overlap.OnOverlapping(entity);
                    entity.OnOverlapping(overlap);

                    if (!overlappingObjs.Contains(overlap))
                        overlappingObjs.Add(overlap);
                }
            }
            
            // collision
            if (entity.IsCollisionEnabled)
            {
                var collisions = neighbours.Where(n => n.IsCollisionEnabled && entity.CollisionBox.IntersectsWith(n.CollisionBox));

                var hasCollided = false;

                foreach (var collision in collisions)
                {
                    if (hasCollided)
                        continue;

                    hasCollided = true;

                    var up = entityProps.Velocity.Y < 0;
                    entityProps.Velocity.Y = 0;

                    if (up)
                        entityProps.Position.Y = collision.CollisionBox.Bottom + 1;
                    else
                        entityProps.Position.Y = collision.CollisionBox.Top - entity.CollisionBox.Height;

                    collision.OnCollision(entity);
                    entity.OnCollision(collision);
                }
            }


            // [ X ]

            entityProps.Position.X = entityProps.Position.X + (entityProps.Velocity.X * delta);
            // get new neighbours since objects have probably moved
            neighbours = chunkMap.GetNearbyCollidables(entity);

            if (entity.IsOverlappingEnabled)
            {
                // overlaps
                var overlaps = neighbours.Where(n => n.IsOverlappingEnabled && entity.OverlapBox.IntersectsWith(n.OverlapBox));

                foreach (var overlap in overlaps)
                {
                    overlap.OnOverlapping(entity);
                    entity.OnOverlapping(overlap);

                    if (!overlappingObjs.Contains(overlap))
                        overlappingObjs.Add(overlap);
                }
            }

            if (entity.IsCollisionEnabled)
            {
                // collision
                var collisions = neighbours.Where(n => n.IsCollisionEnabled && entity.CollisionBox.IntersectsWith(n.CollisionBox));

                var hasCollided = false;

                foreach (var collision in collisions)
                {
                    if (hasCollided)
                        continue;

                    hasCollided = true;

                    entityProps.Velocity.X = 0;
                    entityProps.Position.X = lastPos.X;

                    collision.OnCollision(entity);
                    entity.OnCollision(collision);
                }
            }

            // track overlappers
            entityProps.OverlappingObjects = overlappingObjs;

            // get new neighbours since objects have probably moved
            neighbours = chunkMap.GetNearbyCollidables(entity);

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

        void handlePlatformerAdditionalPhysics(PhysicalProperties entityProps, IPhysicsObject entity, IPhysicsObject[] neighbours, GameTime gameTime)
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

        void handleTopDownAdditionalPhysics(PhysicalProperties entityProps, IPhysicsObject entity, IPhysicsObject[] neighbours, GameTime gameTime)
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
