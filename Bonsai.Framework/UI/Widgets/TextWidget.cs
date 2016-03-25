using Bonsai.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Widgets
{
    public class TextWidget<T> : WidgetBase, IWidget
    {
        public TextWidget(T value, WidgetSettings settings)
            : base(settings)
        {
            UpdateValue(value);
        }


        public virtual void Load(IContentLoader loader)
        {
        }

        public virtual void Unload()
        {
        }

        public void Draw(GameFrame frame, SpriteBatch batch)
        {
            batch.DrawString(base.Settings.Font, base.Text, base.Settings.Position, base.Settings.ForegroundColor, 0f, this.Origin, 1f, SpriteEffects.None, 0f);
        }

        protected void UpdateValue(T newValue)
        {
            // Compile new value into a string based on options

            if (base.Settings.HasFormat)
            {
                base.Text = string.Format(base.Settings.Format, base.Settings.Label, newValue);
            }
            else if (base.Settings.HasLabel)
            {
                base.Text = string.Concat(base.Settings.Label, newValue);
            }
            else
            {
                base.Text = newValue.ToString();
            }

            // Calculate origin based on parameters

            var dimensions = base.Settings.Font.MeasureString(base.Text);

            switch (base.Settings.Alignment)
            {
                case FieldAlignmentMode.Left:
                    Origin = Vector2.Zero;
                    break;

                case FieldAlignmentMode.Right:
                    Origin = new Vector2(dimensions.X, 0);
                    break;

                case FieldAlignmentMode.Center:
                    Origin = new Vector2(dimensions.X / 2, 0);
                    break;
            }

        }

    }
}
