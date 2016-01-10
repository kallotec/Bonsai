using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public static class RectangleCollision
    {
        public static RectangleCollisionDirection GetCollisionDirection(Rectangle aRect, Rectangle bRect, Vector2 depth)
        {
            if (depth == Vector2.Zero)
                throw new InvalidOperationException("Boxes don't intersect");

            float absDepthX = Math.Abs(depth.X);
            float absDepthY = Math.Abs(depth.Y);

            // Resolve the collision along the shallow axis.
            if (absDepthY < absDepthX)
            {
                // [vertical collision]

                //check distances from each side
                var t = aRect.Top - bRect.Bottom;
                var b = aRect.Bottom - bRect.Top;

                if (t < 0) t *= -1;
                if (b < 0) b *= -1;

                //determine normal
                Vector2 normal = Vector2.Zero;

                if (t < b)
                    return RectangleCollisionDirection.Top;
                else if (b < t)
                    return RectangleCollisionDirection.Bottom;

            }
            else if (absDepthY > absDepthX)
            {
                // [horizontal collision]

                //check distances from each side
                var l = aRect.Left - bRect.Right;
                var r = aRect.Right - bRect.Left;

                if (l < 0) l *= -1;
                if (r < 0) r *= -1;

                //determine normal
                Vector2 normal = Vector2.Zero;

                if (l < r)
                    return RectangleCollisionDirection.Left;
                else if (r < l)
                    return RectangleCollisionDirection.Right;
            }

            throw new Exception("Unknown collision direction");
        }

        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }


    }
}
