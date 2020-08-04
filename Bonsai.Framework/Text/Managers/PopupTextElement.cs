using Bonsai.Framework.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.Framework.Text.Managers
{
    public class PopupTextElement : IDeletable
    {
        public PopupTextElement(ITextElement element)
        {
            this.TextElement = element;
        }

        int? fadeTimeTotalMs;

        public ITextElement TextElement { get; private set; }
        public Vector2 Offset { get; set; }
        public FadeDirection FadeDirection { get; set; } = FadeDirection.Static;
        public MillisecCounter FadeOutCounter { get; set; } = new MillisecCounter(0);
        public float MovementSpeed { get; set; } = 20f;
        public int? FadeTimeTotalMs
        {
            get => fadeTimeTotalMs;
            set
            {
                fadeTimeTotalMs = value;

                if (fadeTimeTotalMs != null)
                    FadeOutCounter = new MillisecCounter(fadeTimeTotalMs.Value);
                else
                    FadeOutCounter = null;
            }
        }
        public bool DeleteMe { get; set; }

    }
}
