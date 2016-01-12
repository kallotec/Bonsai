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
using Bonsai.Framework.Common;
using Bonsai.Framework.Screens;
using System.Diagnostics;
using BonsaiSandbox1.Game.Actors;
using BonsaiSandbox1.Game;

namespace BonsaiSandbox1
{
    public class SandboxGame : BonsaiGame
    {
        public SandboxGame()
        {
            Content.RootDirectory = "Content";
        }


        protected override void Init()
        {
            // Setup game window
            base.SetWindow(width: 800, height: 600, showMouse: true);

            // Create level
            var level = new Level();
            level.Exit += () => { this.Exit(); };

            base.GameObjects.Add(level);
        }

    }
}
