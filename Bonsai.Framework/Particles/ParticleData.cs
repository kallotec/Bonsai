using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bonsai.Framework.Particles
{
    public struct ParticleData
    {
        public float BirthTime;
        public float MaxAge;
        public Vector2 OriginalPosition;
        public Vector2 Accelaration;
        public Vector2 Direction;
        public Vector2 Position;
        public float Scaling;
        public Color ModColor;
    }
}
