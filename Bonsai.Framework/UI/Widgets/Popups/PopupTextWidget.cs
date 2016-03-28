using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.Utility;

namespace Bonsai.Framework.UI.Widgets.Popups
{
    public class PopupTextWidget<T> : TextWidget<T>
    {
        public PopupTextWidget(T value, PopupSettings popupSettings) : base(value, popupSettings)
        {
            this.popupSettings = popupSettings;
            lifetime = new MillisecCounter(popupSettings.LifeInMillisecs);
        }

        MillisecCounter lifetime;
        Vector2 offset;
        PopupSettings popupSettings;


        public void Update(GameTime gameTime)
        {
            if (DeleteMe || popupSettings.IsStatic)
                return;

            // Fadeout
            lifetime.Update(gameTime.ElapsedGameTime.Milliseconds);

            // Move
            offset += popupSettings.Velocity * new Vector2((float)gameTime.ElapsedGameTime.TotalSeconds,
                                                           (float)gameTime.ElapsedGameTime.TotalSeconds);
            // Die
            if (lifetime.Completed)
                this.DeleteMe = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (DeleteMe)
                return;

            //draw display text to screen
            spriteBatch.DrawString(base.Settings.Font, 
                                   base.Text, 
                                   base.Settings.Position + offset, 
                                   base.Settings.ForegroundColor);
        }

    }
}
