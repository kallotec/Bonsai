//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework;

//namespace Bonsai.Framework.UI.Widgets.Popups
//{
//    public class UIMessageQueue
//    {
//        public UIMessageQueue()
//        {
//            msgs = new Queue<TextMessage>();
//        }

//        SpriteFont font;
//        Queue<TextMessage> msgs;
//        TextMessage update_currentMessage;


//        public void LoadContent(ContentManager content)
//        {
//            ////load up the font file
//            //font = content.Load<SpriteFont>(Globals.FONT_MAPTEXT);
//        }

//        public void AddMessage(StringBuilder msg, Vector2 position, eMessageType type)
//        {
//            switch (type)
//            {
//                case eMessageType.FadingText_Fast:
//                    msgs.Enqueue(new TextMessage(msg, font, position, Color.Orange, true, true));
//                    break;

//                case eMessageType.FadingText_Slow:
//                    msgs.Enqueue(new TextMessage(msg, font, position, Color.Orange, true, false));
//                    break;

//                case eMessageType.StaticText:
//                    msgs.Enqueue(new TextMessage(msg, font, position, Color.GreenYellow, false, false));
//                    break;

//                //case eMessageType.SpeechBubble:
//                //    throw new NotImplementedException();
//            }
//        }

//        public void Clear()
//        {
//            this.msgs.Clear();
//        }


//        public void Update(GameTime gameTime)
//        {
//            if (update_currentMessage == null)
//            {
//                //all out of messages
//                if (msgs.Count == 0)
//                    return;

//                //dequeue message to be updated and displayed
//                update_currentMessage = msgs.Dequeue();
//            }
//            else
//            {
//                //update
//                update_currentMessage.Update(gameTime);

//                //remove if completed
//                if (update_currentMessage.DeleteMe)
//                    update_currentMessage = null;
//            }
//        }

//        public void Draw(SpriteBatch spriteBatch)
//        {
//            //if a msg has been selected, then display it
//            if (update_currentMessage != null)
//                update_currentMessage.Draw(spriteBatch);
//        }


//    }
//}
