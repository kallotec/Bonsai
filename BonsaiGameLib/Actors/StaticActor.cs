using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework;
using System.Diagnostics;

namespace Bonsai.Framework.Actors
{
    public abstract class StaticActor : DrawableBase
    {
        public StaticActor() { }

        public Color DrawingTint = Color.White;
        public Vector2 Position;
        public Texture2D Texture;

    }
}
