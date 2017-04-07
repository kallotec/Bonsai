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
using Bonsai.Framework.Content;

namespace Bonsai.Sandbox.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SandboxGame : BonsaiGame
    {
        public SandboxGame()
        {
            Content.RootDirectory = "Content";
        }

        protected override void Init()
        {
            // Set up the game window
            base.SetWindow("Platformer Demo", width: 800, height: 600, showMouse: true);

            var gnat = new Gnat();

            base.GameObjects.Add(gnat);
        }


    }
}
