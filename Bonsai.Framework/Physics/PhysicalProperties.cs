using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public class PhysicalProperties
    {
        public PhysicalProperties()
        {
            Tint = Color.White;
            OverlappingObjects = new List<IPhysicsObject>();
        }

        public Vector2 Position;

        // Visuals
        public Color Tint;
        public Texture2D Texture;
        public float Rotation;

        // Movement
        public Vector2 Velocity;
        public float TopSpeed;
        public float Weight;
        public bool HasGravity;

        // Physics
        public Rectangle PhysicalRect;
        Rectangle? overlapRect;
        public Rectangle OverlapRect
        {
            get => overlapRect ?? PhysicalRect;
            set => overlapRect = value;
        }
        public bool IsGrounded { get; set; }
        public List<IPhysicsObject> OverlappingObjects { get; set; }

        public void AddForce(Vector2 power, bool overrideTopSpeed = false)
        {
            if (overrideTopSpeed)
            {
                Velocity += power;
            }
            else
            {
                Velocity.X = MathHelper.Clamp(
                    (Velocity.X + power.X),
                    -TopSpeed,
                    TopSpeed);

                Velocity.Y = MathHelper.Clamp(
                    (Velocity.Y + power.Y),
                    -TopSpeed,
                    TopSpeed);
            }

        }

        public void AddForceX(float power, bool overrideTopSpeed = false)
        {
            if (overrideTopSpeed)
            {
                Velocity.X = (Velocity.X + power);
            }
            else
            {
                Velocity.X = MathHelper.Clamp(
                    (Velocity.X + power),
                    -TopSpeed,
                    TopSpeed);
            }

        }

        public void AddForceY(float power, bool overrideTopSpeed = false)
        {
            if (overrideTopSpeed)
            {
                Velocity.Y = (Velocity.Y + power);
            }
            else
            {
                Velocity.Y = MathHelper.Clamp(
                    (Velocity.Y + power),
                    -TopSpeed,
                    TopSpeed);
            }
        }

    }
}
