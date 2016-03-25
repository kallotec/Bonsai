using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.UI.Widgets;

namespace Bonsai.Framework.UI.Messages
{
    public class TextPopup : TextWidget<string>
    {
        public TextPopup(string text, PopupSettings popupSettings)
            : base(text, popupSettings)
        {
            this.popupSettings = popupSettings;
        }

        int fadeOutCounter;
        Vector2 offset;

        PopupSettings popupSettings;


        public void Update(GameTime gameTime)
        {
            if (DeleteMe || popupSettings.IsStatic)
                return;

            // Fadeout
            fadeOutCounter += gameTime.ElapsedGameTime.Milliseconds;

            // Move
            offset += popupSettings.Velocity * new Vector2((float)gameTime.ElapsedGameTime.TotalSeconds,
                                                           (float)gameTime.ElapsedGameTime.TotalSeconds);
            // Die
            if (fadeOutCounter >= popupSettings.LifeInMillisecs)
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
