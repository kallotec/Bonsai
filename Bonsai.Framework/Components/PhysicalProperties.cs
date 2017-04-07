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
        public float Gravity;
        public float TerminalVelocity;

        // Object
        public Rectangle CollisionRect;

    }
}
