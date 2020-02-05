using Bonsai.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Chunks
{
    public class Chunk
    {
        public Chunk()
        {
            Entities = new List<ICollidable>();
        }

        public List<ICollidable> Entities { get; private set; }

    }
}
