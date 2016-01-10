using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Bonsai.Framework.UI;
using Bonsai.Framework.Actors;

namespace Bonsai.Framework.Levels
{
    public interface ILevel : ILoadable, IUpdateable, IDrawable
    {
        Rectangle Bounds { get; }
        BonsaiGameObject Player { get; }
    }
}
