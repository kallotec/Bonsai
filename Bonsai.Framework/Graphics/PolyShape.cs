using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bonsai.Framework.Graphics
{
    public class PolyShape : ILoadable, IDrawable
    {
        public PolyShape(Vector2[] points, GraphicsDevice device)
        {
            processPoints(points, device);
        }

        GraphicsDevice device;
        BasicEffect basicEffect;
        short[] indexes;
        VertexPositionColor[] vertexes;

        public bool IsHidden { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DrawOrder => throw new NotImplementedException();
        public bool IsAttachedToCamera => throw new NotImplementedException();

        public void Load(IContentLoader loader)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                device.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList, vertexes, 0, vertexes.Length, indexes, 0, indexes.Length / 3);
            }
        }

        void processPoints(Vector2[] points, GraphicsDevice device)
        {
            basicEffect = new BasicEffect(device);
            indexes = new short[points.Length];
            vertexes = new VertexPositionColor[points.Length];

            for (var x = 0; x < points.Length; x++)
            {
                var point = points[x];
                vertexes[x].Position = new Vector3(point.X, point.Y, 0);
                vertexes[x].Color = Color.Orange;
                indexes[x] = (short)x;
            }

        }

    }
}
