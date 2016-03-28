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

namespace Bonsai.Framework.UI.Widgets.Popups
{
    public class PopupManager : DrawableBase, IUpdateable, IDrawable
    {
        public PopupManager()
        {
            msgs = new List<PopupTextWidget<string>>();
        }

        List<PopupTextWidget<string>> msgs;

        public bool IsDisabled { get; set; }


        public void Update(GameTime time)
        {
            PopupTextWidget<string> current = null;

            // Update fading messages and remove any expired
            for (var x = 0; x < this.msgs.Count; x++)
            {
                current = this.msgs[x];
                current.Update(time);

                if (current.DeleteMe)
                {
                    current.Unload();
                    this.msgs.RemoveAt(x);
                    x--;
                }

            }
        }

        public void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            // TODO: Draw only messages that are within the camera's viewport

            // Draw out fading messages
            for (var x = 0; x < this.msgs.Count; x++)
                this.msgs[x].Draw(spriteBatch);

        }

        public void AddMessage(string msg, PopupSettings popupSettings)
        {
            // Load up a new popup
            var popup = new PopupTextWidget<string>(msg, popupSettings);
            popup.Load(loader:null);

            msgs.Add(popup);
        }

        public void Clear()
        {
            // Unload and clear all
            for (var x = 0; x < this.msgs.Count; x++)
            {
                this.msgs[x].Unload();
                this.msgs.RemoveAt(x);
                x--;
            }
        }

    }
}
