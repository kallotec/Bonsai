using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Text;

namespace Bonsai.Framework.Text.Managers
{
    public class TextPopupManager : DrawableBase, ILoadable, IUpdateable
    {
        public TextPopupManager(string fontContentPath, StackingMethod stackingMethod)
        {
            this.fontContentPath = fontContentPath;
            this.stackingMethod = stackingMethod;

            messages = new List<PopupTextElement>();
        }

        StackingMethod stackingMethod;
        string fontContentPath;
        SpriteFont font;
        List<PopupTextElement> messages;
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
                message.TextElement.Unload();

            messages.Clear();
        }

        public void AddMessage(string msg, Vector2 position, MessageType type)
        {
            PopupTextElement newPopup = null;

            switch (type)
            {
                case MessageType.FadingText_Fast:
                    newPopup = new PopupTextElement(
                        new TextElement<string>(
                            msg,
                            font,
                            new TextElementSettings { ForegroundColor = Color.Orange }
                            )
                        {
                            Position = position
                        })
                    {
                        FadeTimeTotalMs = 500,
                        FadeDirection = FadeDirection.Up,
                    };
                    break;

                case MessageType.FadingText_Slow:
                    newPopup = new PopupTextElement(
                        new TextElement<string>(
                            msg,
                            font,
                            new TextElementSettings { ForegroundColor = Color.Orange }
                            )
                        {
                            Position = position
                        })
                    {
                        FadeTimeTotalMs = 1000,
                        FadeDirection = FadeDirection.Up,
                    };
                    break;

                case MessageType.StaticText:
                    newPopup = new PopupTextElement(
                        new TextElement<string>(
                            msg,
                            font,
                            new TextElementSettings { ForegroundColor = Color.GreenYellow  }
                        )
                        {
                            Position = position
                        });
                    break;

            }

            if (newPopup != null)
            {
                newPopup.TextElement.Load(loader);
                messages.Add(newPopup);
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
                messages[0].TextElement.Draw(gameTime, spriteBatch);
            }
            else if (stackingMethod == StackingMethod.Parallel)
            {
                foreach (var msg in messages)
                    msg.TextElement.Draw(gameTime, spriteBatch);
            }

        }

        void updateQueue(GameTime gameTime)
        {
            if (messages.Count == 0)
                return;

            var first = messages[0];

            updateElement(gameTime, first);

            if (first.DeleteMe)
                messages.RemoveAt(0);
        }

        void updateParallel(GameTime gameTime)
        {
            for (var x = 0; x < messages.Count; x++)
            {
                var current = messages[x];
                updateElement(gameTime, current);

                if (current.DeleteMe)
                {
                    messages.RemoveAt(x);
                    x--;
                }

            }
        }

        void updateElement(GameTime gameTime, PopupTextElement element)
        {
            if (element.FadeDirection == FadeDirection.Static)
                return;

            if (element.FadeOutCounter.Completed)
            {
                element.DeleteMe = true;
                return;
            }

            // move element (up by default)
            element.TextElement.Position -= new Vector2(0, (float)(element.MovementSpeed * gameTime.ElapsedGameTime.TotalSeconds));
            element.FadeOutCounter.Update(gameTime.ElapsedGameTime.Milliseconds);

        }

    }
}
