using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Widgets
{
    public interface IWidget : IDrawable, ILoadable, IUpdateable
    {
        bool DeleteMe { get; }
    }

}
