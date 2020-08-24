using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public interface IPhysicsObject
    {
        bool IsOverlappingEnabled { get; }
        bool IsCollisionEnabled { get; }

        RectangleF CollisionBox { get; }
        RectangleF OverlapBox { get; }

        void OnCollision(object actor);
        void OnOverlapping(object actor);
    }
}
