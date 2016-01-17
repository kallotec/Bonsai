using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Collision;

namespace Bonsai.Framework.Maths
{
    public class EntityMathHelper
    {
        public static float CalculateDistanceBetween(Vector2 a, Vector2 b)
        {
            Vector2 delta = b - a;
            return delta.Length();
        }

        public static void UpdateVelocity(int millisecs, float speed, Vector2 from, Vector2 to, out Vector2 velocity)
        {
            //calculate movement increment
            velocity.X = to.X - from.X;
            velocity.Y = to.Y - from.Y;
            velocity.Normalize(); //cut down to lowest unit of movement
            velocity *= speed * (millisecs / 1000f);
        }

        public static Vector2 UpdateVelocity(float direction, float speed)
        {
            //setup velocity
            Vector2 up = new Vector2(1, 0);
            Matrix rotationMatrix = Matrix.CreateRotationZ(direction);
            return Vector2.Transform(up, rotationMatrix) * speed;
        }

        public static void LerpDirection(ref float currentDirection, float newDirection)
        {
            //capture the difference in current and new
            float diff = MathHelper.WrapAngle(newDirection - currentDirection);
            float step = 0.0625f;
            
            //if the difference is too small to matter, snap to new direction
            if ((diff > 0 && diff < step) || (diff < 0 && diff > (step * -1)))
            {
                currentDirection = newDirection;
                return;
            }

            //calculate changes
            float x = (MathHelper.WrapAngle(newDirection - currentDirection) * step);
            currentDirection += x;
        }

        public static Vector2 PlotVector(float direction, int distance, Vector2 currentPosition)
        {
            Matrix rotMatrix = Matrix.CreateRotationZ(direction);
            Vector2 dest = Vector2.Transform(Globals.Up, rotMatrix);
            dest *= distance;
            dest += currentPosition;
            return dest;
        }

        public static float GetDirectionInRadians(Vector2 from, Vector2 to)
        {
            Vector2 delta = new Vector2(to.X - from.X, to.Y - from.Y);
            return (float)Math.Atan2(delta.Y, delta.X);
        }
        
        public static Vector2 GetReflectionNormal(Rectangle p, Rectangle o)
        {
            //Test which side was hit
            float l, t, r, b;

            //check distances from each side
            l = o.Left - p.Right;
            t = o.Top - p.Bottom;
            r = o.Right - p.Left;
            b = o.Bottom - p.Top;

            if (l < 0) l *= -1;
            if (t < 0) t *= -1;
            if (r < 0) r *= -1;
            if (b < 0) b *= -1;

            //determine normal
            Vector2 normal = Vector2.Zero;

            if (l < t && l < r && l < b)
                normal = AABBNormals.Left;
            else if (t < l && t < r && t < b)
                normal = AABBNormals.Top;
            else if (r < l && r < t && r < b)
                normal = AABBNormals.Right;
            else if (b < l && b < t && b < r)
                normal = AABBNormals.Bottom;
            else
            {
                if (l == b || l == t)
                    normal = AABBNormals.Left;
                else if (r == b || r == t)
                    normal = AABBNormals.Right;
            }

            return normal;
        }

        public static Color LerpColor(Color from, Color to, byte amount)
        {
            //Red channel
            if (from.R > to.R)
                from.R = (byte)(from.R - amount);
            else if (from.R < to.R)
                from.R = (byte)(from.R + amount);
            //if equals, do nothing

            //Green channel
            if (from.G > to.G)
                from.G = (byte)(from.G - amount);
            else if (from.G < to.G)
                from.G = (byte)(from.G + amount);
            //if equals, do nothing

            //Blue channel
            if (from.B > to.B)
                from.B = (byte)(from.B - amount);
            else if (from.B < to.B)
                from.B = (byte)(from.B + amount);
            //if equals, do nothing

            //return changes
            return from;
        }

    }
}
