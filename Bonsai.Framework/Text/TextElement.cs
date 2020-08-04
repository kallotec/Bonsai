using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Utility;
using Bonsai.Framework.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bonsai.Framework.Text
{
    public class TextElement<T> : DrawableBase, ITextElement
    {
        public TextElement(T value, TextElementSettings settings)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Settings = settings;
            this.value = value;

            base.IsAttachedToCamera = settings.IsAttachedToCamera;

            //if (settings.FadesInMillisecs != null)
            //    fadeOutCounter = new MillisecCounter(settings.FadesInMillisecs.Value);
        }

        public TextElement(GameVariable<T> variable, TextElementSettings settings)
        {
            if (variable == null)
                throw new ArgumentNullException("variable");
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Settings = settings;
            this.variable = variable;

            base.IsAttachedToCamera = settings.IsAttachedToCamera;

            //if (settings.FadesInMillisecs != null)
            //    fadeOutCounter = new MillisecCounter(settings.FadesInMillisecs.Value);
        }

        T value;
        GameVariable<T> variable;
        //MillisecCounter fadeOutCounter;
        //float yMovementSpeed = 20f;
        Vector2 textMeasurements;
        public readonly TextElementSettings Settings;
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

        public Color ForegroundColor
        {
            get => Settings.ForegroundColor;
            set => Settings.ForegroundColor = value;
        }
        public Color? BackgroundColor
        {
            get => Settings.BackgroundColor;
            set => Settings.BackgroundColor = value;
        }

        public void Load(IContentLoader loader)
        {
            if (variable != null)
            {
                variable.Changed += handleVariableChanged;
                UpdateText(variable.Value);
            }
            else
            {
                UpdateText(value);
            }
        }

        public void Unload()
        {
            if (variable != null)
                variable.Changed -= handleVariableChanged;
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

            // background
            if (Settings.BackgroundColor != null)
            {
                batch.Draw(FrameworkGlobals.Pixel, BackgroundBox, Settings.BackgroundColor.Value);
            }

            // foreground
            batch.DrawString(Settings.Font, Text, Settings.Position, Settings.ForegroundColor, 0f, this.Origin, 1f, SpriteEffects.None, 0f);
        }

        public void UpdateText(T newValue)
        {
            // compile new value into a string based on options
            if (Settings.HasFormat)
            {
                // ignore display mode setting and use supplied format
                Text = string.Format(Settings.Format, Settings.Label, newValue);
            }
            else
            {
                switch (Settings.DisplayMode)
                {
                    case TextDisplayMode.LabelAndValue:
                        Text = string.Concat(Settings.Label, newValue);
                        break;

                    case TextDisplayMode.LabelOnly:
                        Text = Settings.HasLabel ? Settings.Label : string.Empty;
                        break;

                    case TextDisplayMode.ValueOnly:
                        Text = newValue?.ToString() ?? string.Empty;
                        break;
                }

            }

            // rebuild box
            textMeasurements = Settings.Font.MeasureString(Text);

            // calculate origin based on text
            var dimensions = Settings.Font.MeasureString(Text);

            var originX = 0f;
            var originY = 0f;

            switch (Settings.HorizontalAlignment)
            {
                case TextHorizontalAlignment.Left:
                    originX = 0;
                    break;

                case TextHorizontalAlignment.Center:
                    originX = dimensions.X / 2;
                    break;

                case TextHorizontalAlignment.Right:
                    originX = dimensions.X;
                    break;
            }

            switch (Settings.VerticalAlignment)
            {
                case TextVerticalAlignment.Top:
                    originY = 0;
                    break;

                case TextVerticalAlignment.Center:
                    originY = dimensions.Y / 2;
                    break;

                case TextVerticalAlignment.Bottom:
                    originY = dimensions.Y;
                    break;
            }

            Origin = new Vector2(originX, originY);
        }

        void handleVariableChanged(T newValue)
        {
            // Update text field with new value
            UpdateText(newValue);
        }


        //        //void update_pulsing(GameTime gameTime)
        //        //{
        //        //    //byte channel = 0;

        //        //    //switch (PulseChannel)
        //        //    //{
        //        //    //    case ePulseChannelSelection.R:
        //        //    //        channel = color.R;
        //        //    //        break;
        //        //    //    case ePulseChannelSelection.G:
        //        //    //        channel = color.G;
        //        //    //        break;
        //        //    //    case ePulseChannelSelection.B:
        //        //    //        channel = color.B;
        //        //    //        break;
        //        //    //}

        //        //    //if (channel >= pulse_threshold_high)
        //        //    //    pulse_movement = (int)pulse_speed * -1;

        //        //    //if (channel <= pulse_threshold_low)
        //        //    //    pulse_movement = (int)pulse_speed * 1;

        //        //    //switch (PulseChannel)
        //        //    //{
        //        //    //    case ePulseChannelSelection.R:
        //        //    //        color.R += (byte)pulse_movement;
        //        //    //        break;
        //        //    //    case ePulseChannelSelection.G:
        //        //    //        color.G += (byte)pulse_movement;
        //        //    //        break;
        //        //    //    case ePulseChannelSelection.B:
        //        //    //        color.B += (byte)pulse_movement;
        //        //    //        break;
        //        //    //}

        //        //}

    }
}
