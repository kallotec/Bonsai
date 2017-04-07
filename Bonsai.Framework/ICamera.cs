﻿using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface ICamera : Bonsai.Framework.IUpdateable
    {
        Matrix Transform { get; }
        Vector2 Focus { get; }

        void SetFocus(GameEntity focusedActor);
        void SetFocus(Vector2 focusedPoint);

    }
}