using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.Framework.Text
{
    public interface ITextElement : IDrawable, IUpdateable, ILoadable
    {
        Vector2 Position { get; set; }
        Color ForegroundColor { get; set; }
        Color? BackgroundColor { get; set; }
    }
}
