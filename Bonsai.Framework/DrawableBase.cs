using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public abstract class DrawableBase : BonsaiGameObject
    {
        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
    }
}
