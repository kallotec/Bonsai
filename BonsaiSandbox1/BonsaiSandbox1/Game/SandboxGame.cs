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

        Level level;


        protected override void Init()
        {
            //setup
            base.SetWindow(width: 800, height: 600);
            base.SetMouse(isVisible: true);
        }

        protected override void Load(ContentManager contentManager)
        {
            level = new Level();
            level.Exit += level_Exit;
            level.Load(contentManager);
        }

        protected override void Unload()
        {
            level.Exit -= level_Exit;
            level.Unload();
            level = null;
        }

        protected override void Update(GameFrame frame)
        {
            //update level
            level.Update(frame);
        }

        protected override void Draw(GameFrame frame)
        {
            base.SpriteBatch.Begin();

            //draw level
            level.Draw(frame, base.SpriteBatch);
            
            base.SpriteBatch.End();
        }

        void level_Exit()
        {
            this.Exit();
        }


    }
}
