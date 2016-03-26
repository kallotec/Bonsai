﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface IDrawable
    {
        bool IsHidden { get; set; }
        int DrawOrder { get; }
        bool IsAttachedToCamera { get; }

        void Draw(GameTime time, SpriteBatch batch);
    }
}
