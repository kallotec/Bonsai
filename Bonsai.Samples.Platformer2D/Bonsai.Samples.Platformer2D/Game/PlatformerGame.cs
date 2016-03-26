using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using System.Diagnostics;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Bonsai.Samples.Platformer2D.Game;
using Bonsai.Framework.Content;

namespace Bonsai.Samples.Platformer2D
{
    public class PlatformerGame : BonsaiGame
    {
        public PlatformerGame()
        {
            Content.RootDirectory = "Content";
        }

        Level level;
        HUD hud;


        protected override void Init()
        {
            // Set up the game window
            base.SetWindow("Platformer Demo", width: 800, height: 600, showMouse: true);

            // Create level
            level = new Level();
            level.Camera = base.Camera;

            // Create HUD
            hud = new HUD(level);
            hud.ScreenBounds = base.GraphicsDevice.Viewport.Bounds;
            hud.Exit += () => { this.Exit(); };

            // Add objects to pipeline
            base.GameObjects.Add(level);
            base.GameObjects.Add(hud);

        }

        protected override void Load(IContentLoader loader)
        {
            // Enforce load sequence
            level.Load(loader);
            hud.Load(loader);
        }

        protected override void Unload()
        {
            // Enforce unload sequence
            hud.Unload();
            level.Unload();
        }

    }
}
