using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.UI.Text
{
    public class WidgetSettings
    {
        public WidgetSettings()
        {
            ForegroundColor = Color.White;
        }

        public bool IsAttachedToCamera { get; set; }
        public Color ForegroundColor { get; set; }
        public Color? BackgroundColor { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position;
        public FieldAlignmentMode Alignment { get; set; }
        public FieldDisplayMode DisplayMode { get; set; }
        public string Label { get; set; }
        public string Format { get; set; }
        public Vector2 Padding { get; set; }

        public int? FadesInMillisecs { get; set; }
        public FadeDirection? FadeDirection { get; set; }

        public bool HasLabel => !string.IsNullOrWhiteSpace(Label);
        public bool HasFormat => !string.IsNullOrWhiteSpace(Format);

    }
}
