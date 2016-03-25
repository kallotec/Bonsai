using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Widgets
{
    public abstract class WidgetBase : DrawableBase
    {
        public WidgetBase(WidgetSettings settings)
        {
            this.Settings = settings;
        }

        public WidgetSettings Settings;
        public Vector2 Origin;
        public string Text;

    }
}
