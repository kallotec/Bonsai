using Bonsai.Framework;
using Bonsai.Framework.Chunks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public class MapPhysics
    {
        public MapPhysics(ChunkMap tileMap, PhysicsSettings physSettings)
        {
            this.tileMap = tileMap;
            this.physSettings = physSettings;
        }

        ChunkMap tileMap;
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
                neighbours = tileMap.GetNearbyCollidables(entity);
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
                neighbours = tileMap.GetNearbyCollidables(entity);
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
                var groundedArea = new Rectangle(entity.CollisionBox.X, entity.CollisionBox.Y + entity.CollisionBox.Height, entity.CollisionBox.Width, 1);
                var groundingIntersections = neighbours.Any(n => groundedArea.Intersects(n.CollisionBox));
                entityProps.Grounded = groundingIntersections;
            }

            // [ Gravity ]

            if (!entityProps.Grounded)
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
                entityProps.Grounded &&
                entityProps.Velocity.X != 0)
            {
                var lerped = MathHelper.Lerp(entityProps.Velocity.X, 0, physSettings.Friction);
                if (Math.Abs(lerped) < 2)
                    lerped = 0;

                entityProps.Velocity.X = lerped;
            }

            // [ Tile map update ]

            tileMap.UpdateEntity(entity);

        }

    }
}
