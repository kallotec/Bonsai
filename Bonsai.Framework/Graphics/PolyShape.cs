using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Bonsai.Framework.Graphics
{
    public class PolyShape : ILoadable, IDrawable
    {
        public PolyShape(Vector2[] positions)
        {
            device = BonsaiGame.Current.GraphicsDevice;
            processPoints(positions);
        }

        GraphicsDevice device;
        BasicEffect basicEffect;
        int[] indexes;
        Vector2[] points;
        VertexPositionColor[] vertices;
        VertexPositionColor[] triangulatedVerticies;

        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
        public Vector2 Position { get; set; }
        public RectangleF Bounds { get; private set; }


        public void Load(IContentLoader loader)
        {
        }

        public void Unload()
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            for (var x = 1; x < points.Length + 1; x++)
            {
                if (x == points.Length)
                {
                    // complete line
                    batch.DrawLine(points[x - 1], points[0], Color.Orange);
                }
                else
                {
                    batch.DrawLine(points[x - 1], points[x], Color.Orange);
                }

            }

        }

        void processPoints(Vector2[] points)
        {
            var pList = points.ToList();

            for (var x = 0; x < points.Length; x++)
            {
                var p = points[x];

                if (p.X == 0 && p.Y == 0)
                    pList.RemoveAt(x);
            }

            this.points = pList.ToArray();

            var minX = 0f;
            var minY = 0f;
            var maxX = 0f;
            var maxY = 0f;

            // convert to verticies
            for (var x = 0; x < this.points.Length; x++)
            {
                var point = this.points[x];

                if (x == 0)
                {
                    minX = point.X;
                    minY = point.Y;
                    maxX = point.X;
                    maxY = point.Y;
                } 
                else
                {
                    minX = Math.Min(point.X, minX);
                    minY = Math.Min(point.Y, minY);
                    maxX = Math.Max(point.X, maxX);
                    maxY = Math.Max(point.Y, maxY);
                }
            }

            Bounds = new RectangleF(minX, minY, Math.Abs(maxX - minX), Math.Abs(maxY - minY));
        }

    }
}
