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
    public class SandboxGame : BonsaiGame
    {
        public SandboxGame()
        {
            Content.RootDirectory = "Content";
        }

        protected override void Init()
        {
            throw new NotImplementedException();
        }

        protected override void Load(IContentLoader loader)
        {
            throw new NotImplementedException();
        }

        protected override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
