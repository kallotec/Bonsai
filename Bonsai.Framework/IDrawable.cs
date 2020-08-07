using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IDrawable
    {
        Vector2 Position { get; set; }
        bool IsHidden { get; set; }
        DrawOrderPosition DrawOrder { get; }
        bool IsAttachedToCamera { get; }

        void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition);
    }
}
