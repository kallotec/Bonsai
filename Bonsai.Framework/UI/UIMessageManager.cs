using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Bonsai.Framework.Actors;

namespace Bonsai.Framework.UI
{
    public enum eMessageType { FadingText_Slow, FadingText_Fast, StaticText } //SpeechBubble, }

    public class UIMessageManager
    {
        SpriteFont font;
        List<TextElement> msgs = new List<TextElement>();
        TextElement update_currentFadingTextMessage;


        public void LoadContent(SpriteFont font)
        {
            //load up the font file
            this.font = font;
        }
        
        public void AddMessage(StringBuilder msg, Vector2 position, eMessageType type)
        {
            switch (type)
            {
                case eMessageType.FadingText_Fast:
                    msgs.Add(new TextElement(msg, font, position, Color.Orange, true, true));
                    break;

                case eMessageType.FadingText_Slow:
                    msgs.Add(new TextElement(msg, font, position, Color.Orange, true, false));
                    break;

                case eMessageType.StaticText:
                    msgs.Add(new TextElement(msg, font, position, Color.GreenYellow, false, false));
                    break;

                //case eMessageType.SpeechBubble:
                //    throw new NotImplementedException();
            }   
        }

        public void Clear()
        {
            this.msgs.Clear();
        }


        public void Update(GameTime gameTime)
        {
            //update fading messages and remove any expired
            for (int m = 0; m < this.msgs.Count; m++)
            {
                update_currentFadingTextMessage = this.msgs[m];
                update_currentFadingTextMessage.Update(gameTime);

                if (update_currentFadingTextMessage.DeleteMe)
                {
                    this.msgs.RemoveAt(m);
                    m--;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //todo: draw only messages that are within the camera's viewport

            //draw out fading messages
            for (int m = 0; m < this.msgs.Count; m++)
                this.msgs[m].Draw(spriteBatch);

        }

        public void Draw(SpriteBatch spriteBatch, Color force_to_color)
        {
            //todo: draw only messages that are within the camera's viewport

            //draw out fading messages
            for (int m = 0; m < this.msgs.Count; m++)
                this.msgs[m].Draw(spriteBatch, force_to_color);

        }

        
    }
}
