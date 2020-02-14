using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bonsai.Framework.UI.Menus
{
    public delegate void delMenuItemClick();

    public interface IMenuItem
    {
        StringBuilder Label { get; set; }
        object Value { get; set; }
        Vector2 Position { get; set; }
        bool IsHighlighted { get; set; }

        delMenuItemClick Clicked { get; }

        void Update(GameTime time);
        void Draw(SpriteBatch batch);

    }
}
