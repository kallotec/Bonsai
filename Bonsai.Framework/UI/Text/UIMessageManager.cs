using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Content;
using Bonsai.Framework.UI.Widgets;

namespace Bonsai.Framework.UI.Text
{
    public class UIMessageManager : DrawableBase, ILoadable, IUpdateable, IDrawable
    {
        public UIMessageManager(string fontContentPath, StackingMethod stackingMethod)
        {
            this.fontContentPath = fontContentPath;
            this.stackingMethod = stackingMethod;

            messages = new List<IWidget>();
        }

        StackingMethod stackingMethod;
        string fontContentPath;
        SpriteFont font;
        List<IWidget> messages;
        IContentLoader loader;
        public bool IsDisabled => false;


        public void Load(IContentLoader loader)
        {
            this.loader = loader;
            font = loader.Load<SpriteFont>(fontContentPath);
        }

        public void Unload()
        {
            foreach (var message in messages)
                message.Unload();

            messages.Clear();
        }

        public void AddMessage(string msg, Vector2 position, MessageType type)
        {
            TextElement<string> newText = null;

            switch (type)
            {
                case MessageType.FadingText_Fast:
                    newText = new TextElement<string>(msg,
                        new WidgetSettings
                        {
                            Font = font,
                            Position = position,
                            ForegroundColor = Color.Orange,
                            FadesInMillisecs = 500,
                            FadeDirection = FadeDirection.Up,
                        });
                    break;

                case MessageType.FadingText_Slow:
                    newText = new TextElement<string>(msg,
                        new WidgetSettings
                        {
                            Font = font,
                            Position = position,
                            ForegroundColor = Color.Orange,
                            FadesInMillisecs = 1000,
                            FadeDirection = FadeDirection.Up,
                        });
                    break;

                case MessageType.StaticText:
                    newText = new TextElement<string>(msg,
                        new WidgetSettings
                        {
                            Font = font,
                            Position = position,
                            ForegroundColor = Color.GreenYellow,
                        });
                    break;

                //case eMessageType.SpeechBubble:
                //    throw new NotImplementedException();
            }

            if (newText != null)
            {
                newText.Load(loader);
                messages.Add(newText);
            }

        }

        public void Update(GameTime gameTime)
        {
            switch (stackingMethod)
            {
                case StackingMethod.Parallel:
                    updateParallel(gameTime);
                    break;

                case StackingMethod.Queue:
                    updateQueue(gameTime);
                    break;
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (messages.Count == 0)
                return;

            if (stackingMethod == StackingMethod.Queue)
            {
                messages[0].Draw(gameTime, spriteBatch);
            }
            else if (stackingMethod == StackingMethod.Parallel)
            {
                foreach (var msg in messages)
                    msg.Draw(gameTime, spriteBatch);
            }

        }


        void updateQueue(GameTime gameTime)
        {
            if (messages.Count == 0)
                return;

            var first = messages[0];

            first.Update(gameTime);

            if (first.DeleteMe)
                messages.RemoveAt(0);
        }

        void updateParallel(GameTime gameTime)
        {
            for (var x = 0; x < messages.Count; x++)
            {
                var current = messages[x];
                current.Update(gameTime);

                if (current.DeleteMe)
                {
                    messages.RemoveAt(x);
                    x--;
                }

            }
        }

    }
}
