using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Actors
{
    public abstract class MoveableActor : StaticActor
    {
        public MoveableActor() {}

        protected float Acceleration;
        protected int Speed;
        protected Vector2 Velocity;

    }
}
