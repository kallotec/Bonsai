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

            base.IsAttachedToCamera = settings.IsAttachedToCamera;

            //if (settings.FadesInMillisecs != null)
            //    fadeOutCounter = new MillisecCounter(settings.FadesInMillisecs.Value);
        }

        Texture2D image;
        MillisecCounter fadeOutCounter;
        float yMovementSpeed = 20f;
        Vector2 textMeasurements;
        int valueCount;

        public readonly ImageElementSettings Settings;
        public Vector2 Origin;
        public string Text;
        public Rectangle BackgroundBox
        {
            get
            {
                var positionedBox = new Rectangle(
                    (int)Settings.Position.X - (int)Origin.X, 
                    (int)Settings.Position.Y - (int)Origin.Y, 
                    (int)textMeasurements.X, 
                    (int)textMeasurements.Y);

                positionedBox.Inflate((int)Settings.Padding.X, (int)Settings.Padding.Y);

                return positionedBox;
            }
        }
        public bool IsDisabled => false;


        public void Load(IContentLoader loader)
        {
            var dimensions = this.image.Bounds;

            switch (Settings.Alignment)
            {
                case ImageHorizontalAlignMode.Left:
                    Origin = Vector2.Zero;
                    break;

                case ImageHorizontalAlignMode.Right:
                    Origin = new Vector2(dimensions.X, 0);
                    break;

                case ImageHorizontalAlignMode.Center:
                    Origin = new Vector2(dimensions.X / 2, 0);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (DeleteMe || IsDisabled)
                return;
            //if (Settings.FadesInMillisecs == null)
            //    return;

            //if (Settings.FadeDirection != null)
            //{
            //    if (fadeOutCounter.Completed)
            //    {
            //        DeleteMe = true;
            //        return;
            //    }

            //    // float upwards slowly
            //    if (Settings.FadeDirection == FadeDirection.Up)
            //        Settings.Position.Y -= (float)(yMovementSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            //    fadeOutCounter.Update(gameTime.ElapsedGameTime.Milliseconds);
            //}
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            if (DeleteMe)
                return;

            batch.Draw(this.image, Settings.Position, Color.White);
        }

    }
}
