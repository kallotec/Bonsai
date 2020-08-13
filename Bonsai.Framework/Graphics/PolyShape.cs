using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Bonsai.Framework.Graphics
{
    public class PolyShape : ILoadable, IDrawable
    {
        public PolyShape(Vector2[] positions, Color tint)
        {
            this.Tint = tint;
            processPoints(positions);
            this.Texture = createTexture();
        }

        Vector2[] points;
        Vector2[] pointsRelative;

        public Color Tint { get; set; }
        public Texture2D Texture { get; private set; }

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
            batch.Draw(Texture, (Rectangle)Bounds, this.Tint);
            batch.DrawPolygon(new Vector2(), points.ToList(), this.Tint);
        }

        void processPoints(Vector2[] list)
        {
            // trim off any 0,0's
            var pList = list.ToList();
            for (var x = 0; x < list.Length; x++)
            {
                var p = list[x];

                if (p.X == 0 && p.Y == 0)
                    pList.RemoveAt(x);
            }
            this.points = pList.ToArray();

            // get bounds
            var minX = this.points.Min(x => x.X);
            var maxX = this.points.Max(x => x.X);
            var minY = this.points.Min(x => x.Y);
            var maxY = this.points.Max(x => x.Y);
            Bounds = new RectangleF(minX, minY, Math.Abs(maxX - minX), Math.Abs(maxY - minY));

            var xOffset = minX;
            var yOffset = minY;

            // relative
            this.pointsRelative = this.points.Select(p => p + new Vector2(-xOffset, -yOffset)).ToArray();
        }

        Texture2D createTexture()
        {
            var w = (int)Bounds.Width;
            var h = (int)Bounds.Height;

            var t = new Texture2D(BonsaiGame.Current.GraphicsDevice, w, h);

            var array = new Color[w * h];
            var poly = new Polygon(pointsRelative);

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var v = new Vector2(x, y);
                    var insidePoly = poly.Contains(v);

                    array[(w * y) + x] = (insidePoly ? Color.White : Color.Transparent);
                }
            }

            t.SetData(array);
            return t;
        }

    }
}
