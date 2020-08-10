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
        }

        public Vector2 Position;

        // Visuals
        public Color Tint;
        public Texture2D Texture;
        public float DirectionAim;
        public Direction Direction
        {
            get
            {
                if (DirectionAim >= 0 && DirectionAim <= 180)
                    return Framework.Direction.Right;
                else
                    return Framework.Direction.Left;
            }
        }

        // Movement
        public Vector2 Velocity;
        public float TopSpeed;
        public float Weight;
        public bool HasGravity;

        // Object
        public Rectangle PhysicalRect;
        public bool IsGrounded;


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
