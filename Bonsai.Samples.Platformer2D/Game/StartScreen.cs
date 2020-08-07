using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Images;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Menus;
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

        ImageElement logo;
        SimpleMenu menu;

        public event EventHandler StartGame;
        public event EventHandler ExitGame;
        List<KeyPressListener> listeners;

        public override void Load(IContentLoader loader)
        {
            var logoTexture = loader.Load<Texture2D>(ContentPaths.LOGO_START_SCREEN);

            logo = new ImageElement(logoTexture, new ImageElementSettings
            {
                HorizontalAlignment = ImageHorizontalAlignment.Center,
                VerticalAlignment = ImageVerticalAlignment.Center,
            })
            {
                Position = base.Game.ScreenCenter + new Vector2(0, -100)
            };

            menu = new SimpleMenu(ContentPaths.FONT_UI_GENERAL, new Dictionary<string, Action>()
            {
                { "Start Game", () => StartGame?.Invoke(this, null) },
                { "Exit Game", () => ExitGame?.Invoke(this, null) }
            })
            {
                Position = base.Game.ScreenCenter
            };

            menu.Load(loader);

            listeners = new List<KeyPressListener>
            {
                new KeyPressListener(Keys.Escape, () => this.ExitGame?.Invoke(this, null))
            };
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);

            using (var drawer = base.StartDrawing())
            {
                logo.Draw(time, drawer.Value, new Vector2());
                menu.Draw(time, drawer.Value, new Vector2());
            }

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            logo.Update(time);
            menu.Update(time);
            listeners.ForEach(l => l.Update(time));
        }

    }
}
