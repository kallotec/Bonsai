using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public interface ICollidable
    {
        bool IsOverlappingEnabled { get; }
        bool IsCollisionEnabled { get; }
        RectangleF CollisionBox { get; }

        void OnOverlapping(object actor);
    }
}
