using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Utility;
using Bonsai.Framework.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bonsai.Framework.Images
{
    public class ImageElement : DrawableBase, IUpdateable, IDrawable, IDeletable
    {
        public ImageElement(Texture2D image, ImageElementSettings settings)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            this.Settings = settings;
            this.image = image;

            calculate();
        }

        Texture2D image;
        Vector2 position;
        public readonly ImageElementSettings Settings;
        public Vector2 Origin;
        public string Text;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
            }
        }
        public Rectangle BoundingBox
        {
            get
            {
                var positionedBox = new Rectangle(
                    (int)position.X - (int)Origin.X, 
                    (int)position.Y - (int)Origin.Y, 
                    image.Width, 
                    image.Height);

                positionedBox.Inflate((int)Settings.Padding.X, (int)Settings.Padding.Y);

                return positionedBox;
            }
        }
        public bool IsDisabled => false;



        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            if (DeleteMe || IsDisabled)
                return;
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            if (DeleteMe)
                return;
            /*
            batch.Draw(image, position, Color.White);
             */
            batch.Draw(texture: image,
                    position: position, 
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 0f,
                    origin: this.Origin,
                    scale: new Vector2(1,1),
                    effects: SpriteEffects.None,
                    layerDepth: 1f);
                   
        }

        void calculate()
        {
            var dimensions = this.image.Bounds;

            var originX = 0f;
            var originY = 0f;

            switch (Settings.HorizontalAlignment)
            {
                case ImageHorizontalAlignment.Left:
                    originX = 0;
                    break;

                case ImageHorizontalAlignment.Center:
                    originX = dimensions.Width / 2;
                    break;

                case ImageHorizontalAlignment.Right:
                    originX = dimensions.Width;
                    break;
            }

            switch (Settings.VerticalAlignment)
            {
                case ImageVerticalAlignment.Top:
                    originY = 0;
                    break;

                case ImageVerticalAlignment.Center:
                    originY = dimensions.Height / 2;
                    break;

                case ImageVerticalAlignment.Bottom:
                    originY = dimensions.Height;
                    break;
            }

            Origin = new Vector2(originX, originY);
        }

    }
}
