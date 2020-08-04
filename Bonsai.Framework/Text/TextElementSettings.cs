using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Text
{
    public class TextElementSettings
    {
        public TextElementSettings(SpriteFont font)
        {
            Font = font;
            ForegroundColor = Color.White;
        }

        public bool IsAttachedToCamera { get; set; }
        public Color ForegroundColor { get; set; }
        public Color? BackgroundColor { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position;
        public TextHorizontalAlignment HorizontalAlignment { get; set; }
        public TextVerticalAlignment VerticalAlignment { get; set; }
        public TextDisplayMode DisplayMode { get; set; } = TextDisplayMode.ValueOnly;
        public string Label { get; set; }
        public string Format { get; set; }
        public Vector2 Padding { get; set; }
        public bool HasLabel => !string.IsNullOrWhiteSpace(Label);
        public bool HasFormat => !string.IsNullOrWhiteSpace(Format);

    }
}
