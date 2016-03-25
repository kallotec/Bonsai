using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Content;
using Bonsai.Framework.UI.Widgets;

namespace Bonsai.Framework.UI.Messages
{
    public class PopupManager : DrawableBase, IUpdateable
    {
        public PopupManager()
        {
            msgs = new List<TextPopup>();
        }

        List<TextPopup> msgs;

        public bool IsDisabled { get; set; }


        public void Update(GameFrame frame)
        {
            TextPopup current = null;

            // Update fading messages and remove any expired
            for (int x = 0; x < this.msgs.Count; x++)
            {
                current = this.msgs[x];
                current.Update(frame.GameTime);

                if (current.DeleteMe)
                {
                    this.msgs.RemoveAt(x);
                    x--;
                }

            }
        }

        public void Draw(GameFrame frame, SpriteBatch spriteBatch)
        {
            // TODO: draw only messages that are within the camera's viewport

            // Draw out fading messages
            for (var x = 0; x < this.msgs.Count; x++)
                this.msgs[x].Draw(spriteBatch);

        }

        public void AddMessage(string msg, PopupSettings popupSettings)
        {
            msgs.Add(new TextPopup(msg, popupSettings));
        }

        public void Clear()
        {
            this.msgs.Clear();
        }

    }
}
