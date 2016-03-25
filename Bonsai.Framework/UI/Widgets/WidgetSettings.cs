using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Widgets
{
    public class WidgetSettings
    {
        public WidgetSettings()
        {
            ForegroundColor = Color.White;
            BackgroundColor = Color.Black;
        }

        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }
        public FieldAlignmentMode Alignment { get; set; }
        public FieldDisplayMode DisplayMode { get; set; }
        public string Label { get; set; }
        public string Format { get; set; }
        //public bool Pulses { get; set; }

        public bool HasLabel { get { return !string.IsNullOrWhiteSpace(Label); } }
        public bool HasFormat { get { return !string.IsNullOrWhiteSpace(Format); } }

    }
}
