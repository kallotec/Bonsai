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
        public Chunk(ChunkMap chunkMap, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.chunkMap = chunkMap;
            Entities = new List<ICollidable>();
        }

        int x, y;
        ChunkMap chunkMap;

        public List<ICollidable> Entities { get; private set; }

    }
}
