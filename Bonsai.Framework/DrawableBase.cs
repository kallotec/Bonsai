using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    /// <summary>
    ///  Convenience base class that implements the properties required by IDrawable
    /// </summary>
    public abstract class DrawableBase : GameComponentBase
    {
        public bool IsHidden { get; set; }
        public int DrawOrder { get; set; }
        public bool IsAttachedToCamera { get; set; }
    }
}
