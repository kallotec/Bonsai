using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Components
{
    public class PhysicalProperties
    {
        public Vector2 Position;

        // Visuals
        public Color? DrawingTint;
        public Texture2D Texture;

        // Movement
        public Vector2 Velocity;
        public float Acceleration;
        public float JumpPower;
        public float TopSpeed;
        public float Gravity;
        public float TerminalVelocity;
        public bool HasGravity => Gravity != 0;

        // Object
        public Rectangle CollisionRect;


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
