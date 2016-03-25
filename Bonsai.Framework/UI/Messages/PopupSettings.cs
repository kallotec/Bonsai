using Bonsai.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Messages
{
    public class PopupSettings : WidgetSettings
    {
        public bool IsStatic { get; set; }
        public int LifeInMillisecs { get; set; }
        public Vector2 Velocity { get; set; }
    }
}
