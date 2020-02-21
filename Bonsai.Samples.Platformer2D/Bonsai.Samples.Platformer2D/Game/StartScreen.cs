using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Text;
using Bonsai.Framework.UI.Widgets;
using Bonsai.Framework.Variables;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class StartScreen : Screen
    {
        public StartScreen(BonsaiGame game) : base(game)
        {
        }

        List<KeyPressListener> keyListeners;
        SpriteFont font;
        TextElement<string> lblStartMessage;

        public event EventHandler StartGame;


        public override void Load(IContentLoader loader)
        {
            base.Load(loader);

            // Content
            font = loader.Load<SpriteFont>(ContentPaths.FONT_UI_GENERAL);

            lblStartMessage = new TextElement<string>("Press <enter> to start!", new WidgetSettings
            {
                IsAttachedToCamera = true,
                Alignment = FieldAlignmentMode.Center,
                DisplayMode = FieldDisplayMode.ValueOnly,
                Font = font,
                ForegroundColor = Color.White,
                Position = base.ScreenCenter
            });
            lblStartMessage.Load(loader);
            base.GameObjects.Add(lblStartMessage);

            // Key listeners
            keyListeners = new List<KeyPressListener>
            {
                // [Enter] Starts game
                new KeyPressListener(Keys.Enter, () => StartGame?.Invoke(this, null)),

                // [ESC] Exit
                new KeyPressListener(Keys.Escape, () => this.Game.Exit())
            };
            base.GameObjects.AddRange(keyListeners);

            base.Camera.SetFocus(base.ScreenCenter);
            base.GameObjects.Add(Camera);
        }

    }
}
