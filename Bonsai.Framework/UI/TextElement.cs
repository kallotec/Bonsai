using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bonsai.Framework.UI
{
    public class TextElement
    {
        public TextElement(StringBuilder displayText, SpriteFont spriteFont, Vector2 position, Color color, bool fadesOut, bool fastFade)
        {
            this.DisplayText = displayText;
            this.SpriteFont = spriteFont;
            this.Position = position;
            this.Color = color;
            this.fadesOut = fadesOut;
            Vector2 measurement = spriteFont.MeasureString(displayText);
            Box = new Rectangle((int)position.X, (int)position.Y, (int)measurement.X, (int)measurement.Y);
            Box.Inflate(2, 2);

            if (fadesOut)
            {
                if (fastFade)
                {
                    fadeOutCounterMax = 0400;
                    yMovementSpeed = 20f;
                }
                else
                {
                    fadeOutCounterMax = 1000;
                    yMovementSpeed = 10f;
                }
            }
        }

        public StringBuilder DisplayText { get; set; }
        public SpriteFont SpriteFont { get; set; }
        public Vector2 Position;
        public Rectangle Box { get; set; }
        public Color Color { get; set; }
        public bool DeleteMe { get { return deleteMe; } }
        int fadeOutCounter;
        int fadeOutCounterMax = 0400; //milliseconds
        float yMovementSpeed = 20f;
        bool deleteMe;
        bool fadesOut;

        public void Update(GameTime gameTime)
        {
            if (fadesOut)
            {
                fadeOutCounter += gameTime.ElapsedGameTime.Milliseconds;
                Position.Y -= (float)(yMovementSpeed * gameTime.ElapsedGameTime.TotalSeconds);

                if (fadeOutCounter >= fadeOutCounterMax)
                    deleteMe = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draw display text to screen
            spriteBatch.DrawString(SpriteFont, DisplayText, Position, Color);
        }

        public void Draw(SpriteBatch spriteBatch, Color force_to_color)
        {
            //draw display text to screen
            spriteBatch.DrawString(SpriteFont, DisplayText, Position, force_to_color);
        }

    }
}
