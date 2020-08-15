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
        public PolyShape(Vector2[] positions, Color tint, Texture2D tileTexture)
        {
            this.Tint = tint;
            processPoints(positions);
            this.Texture = createPolygonTexture(tileTexture);

            debugTileTex = tileTexture;
        }

        Vector2[] points;
        Vector2[] pointsRelative;

        Texture2D debugTileTex;

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
            //if (debugTileTex != null)
            //    batch.Draw(debugTileTex, new Vector2(Bounds.X, Bounds.Y), Color.White);

            batch.Draw(Texture, (Rectangle)Bounds, Color.White);
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

        Texture2D createPolygonTexture(Texture2D tile)
        {
            var w = (int)Bounds.Width;
            var h = (int)Bounds.Height;

            var baseTexture = (tile != null 
                ? tileTexture(w, h, tile) 
                : tileTextureWithColor(w, h, Color.White));

            cutOutPolygon(baseTexture, pointsRelative);

            return baseTexture;
        }

        void cutOutPolygon(Texture2D baseTexture, Vector2[] pointsRelative)
        {
            var baseData = new Color[baseTexture.Width * baseTexture.Height];
            baseTexture.GetData(baseData);

            var h = baseTexture.Height;
            var w = baseTexture.Width;

            var poly = new Polygon(pointsRelative);

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var v = new Vector2(x, y);
                    var insidePoly = poly.Contains(v);

                    // chop colors out if outside polygon
                    if (!insidePoly)
                        baseData[(w * y) + x] = Color.Transparent;
                }
            }

            baseTexture.SetData(baseData);
        }

        Texture2D tileTextureWithColor(int w, int h, Color color)
        {
            var t = new Texture2D(BonsaiGame.Current.GraphicsDevice, w, h);
            var d = new Color[w * h];

            for (int x = 0; x < d.Length; x++)
                d[x] = color;

            t.SetData(d);
            return t;
        }

        Texture2D tileTexture(int w, int h, Texture2D tile)
        {
            var isWidthBiggerThanTile = (w > tile.Width);
            var isHeightBiggerThanTile = (h > tile.Height);

            var tileTexRect = new Rectangle(0, 0,
                (isWidthBiggerThanTile ? tile.Width : w),
                (isHeightBiggerThanTile ? tile.Height : h));

            var tileData = new Color[tileTexRect.Width * tileTexRect.Height];
            tile.GetData(0, tileTexRect, tileData, 0, tileData.Length);

            var t = new Texture2D(BonsaiGame.Current.GraphicsDevice, w, h);
            t.SetData(0, tileTexRect, tileData, 0, tileData.Length);

            return t;
        }

    }
}
