using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public abstract class DrawableBase : GameComponentBase
    {
        public bool IsHidden { get; set; }
        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
    }
}
