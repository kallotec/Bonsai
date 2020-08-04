using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Images
{
    public class ImageElementSettings
    {
        public ImageElementSettings()
        {
        }

        public bool IsAttachedToCamera { get; set; }
        public Vector2 Position;
        public ImageHorizontalAlignMode Alignment { get; set; }
        public Vector2 Padding { get; set; }
    }
}
