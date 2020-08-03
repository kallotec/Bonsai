using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.Chunks;
using Bonsai.Framework.Content;
using Bonsai.Framework.Input;
using Bonsai.Framework.Particles;
using Bonsai.Framework.Physics;
using Bonsai.Framework.UI;
using Bonsai.Framework.UI.Menus;
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

        SimpleMenu menu;

        public event EventHandler StartGame;
        public event EventHandler ExitGame;


        public override void Load(IContentLoader loader)
        {
            var menuStart = base.Game.ScreenCenter;

            menu = new SimpleMenu(ContentPaths.FONT_UI_GENERAL, new Dictionary<string, Action>()
            {
                { "Start Game", () => StartGame?.Invoke(this, null) },
                { "Exit Game", () => ExitGame?.Invoke(this, null) }
            });

            menu.Load(loader);
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);

            using (var drawer = base.StartDrawing())
            {
                menu.Draw(time, drawer.Value);
            }

        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            menu.Update(time);
        }
    }
}
