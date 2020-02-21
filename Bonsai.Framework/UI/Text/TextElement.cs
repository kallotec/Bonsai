using Bonsai.Framework.Content;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.Utility;
using Bonsai.Framework.Variables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bonsai.Framework.UI.Text
{
    public class TextElement<T> : DrawableBase, IWidget
    {
        public TextElement(T value, WidgetSettings settings)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Settings = settings;
            this.value = value;

            base.IsAttachedToCamera = settings.IsAttachedToCamera;

            if (settings.FadesInMillisecs != null)
                fadeOutCounter = new MillisecCounter(settings.FadesInMillisecs.Value);
        }

        public TextElement(GameVariable<T> variable, WidgetSettings settings)
        {
            if (variable == null)
                throw new ArgumentNullException("variable");
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Settings = settings;
            this.variable = variable;

            base.IsAttachedToCamera = settings.IsAttachedToCamera;

            if (settings.FadesInMillisecs != null)
                fadeOutCounter = new MillisecCounter(settings.FadesInMillisecs.Value);
        }

        GameVariable<T> variable;
        T value;
        MillisecCounter fadeOutCounter;
        float yMovementSpeed = 20f;

        public readonly WidgetSettings Settings;
        public Vector2 Origin;
        public string Text;
        public Rectangle Box { get; private set; }
        public bool IsDisabled => false;

        public FieldAlignmentMode Alignment
        {
            get => Settings.Alignment;
            set
            {
                Settings.Alignment = value;
                UpdateText(this.value);
            }
        }

        public T Value
        {
            get
            {
                if (variable != null)
                    return variable.Value;
                else
                    return value;
            }
        }


        public void Load(IContentLoader loader)
        {
            // Initialize text field based on the type of field supplied
            if (variable != null)
            {
                UpdateText(variable.Value);

                variable.Changed += handleVariableChanged;
            }
            else
            {
                // Static text field
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
            if (Settings.FadesInMillisecs == null)
                return;

            if (Settings.FadeDirection != null)
            {
                if (fadeOutCounter.Completed)
                {
                    DeleteMe = true;
                    return;
                }

                // float upwards slowly
                if (Settings.FadeDirection == FadeDirection.Up)
                    Settings.Position.Y -= (float)(yMovementSpeed * gameTime.ElapsedGameTime.TotalSeconds);

                fadeOutCounter.Update(gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            if (DeleteMe)
                return;

            //var positionedBox = new Rectangle((int)Settings.Position.X, (int)Settings.Position.Y, Box.Width, Box.Height);
            // background
            //batch.Draw(FrameworkGlobals.Pixel, positionedBox, Settings.BackgroundColor);

            // foreground
            batch.DrawString(Settings.Font, Text, Settings.Position, Settings.ForegroundColor, 0f, this.Origin, 1f, SpriteEffects.None, 0f);
        }

        public void UpdateText(T newValue)
        {
            // Compile new value into a string based on options

            if (Settings.HasFormat)
            {
                Text = string.Format(Settings.Format, Settings.Label, newValue);
            }
            else if (Settings.HasLabel)
            {
                Text = string.Concat(Settings.Label, newValue);
            }
            else
            {
                Text = newValue.ToString();
            }

            // Rebuild box

            var measurement = Settings.Font.MeasureString(Text);

            Box = new Rectangle(
                (int)-2, 
                (int)-2, 
                (int)measurement.X, 
                (int)measurement.Y);

            Box.Inflate(2, 2);

            // Calculate origin based on parameters

            var dimensions = Settings.Font.MeasureString(Text);

            switch (Settings.Alignment)
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
