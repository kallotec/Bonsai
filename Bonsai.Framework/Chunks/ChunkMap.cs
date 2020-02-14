using Bonsai.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Chunks
{
    public class ChunkMap
    {
        public ChunkMap(int chunkWidth, int chunkHeight, double mapWidth, double mapHeight)
        {
            index = new Dictionary<ICollidable, Point>();

            this.chunkWidth = chunkWidth;
            this.chunkHeight = chunkHeight;
            MapWidth = mapWidth;
            MapHeight = mapHeight;

            initChunkMap();
        }

        int chunkWidth;
        int chunkHeight;
        public double MapWidth { get; private set; }
        public double MapHeight { get; private set; }
        Chunk[,] grid { get; set; }
        Dictionary<ICollidable, Point> index;


        void initChunkMap()
        {
            var chunksX = (int)(MapWidth / chunkWidth);
            var chunksY = (int)(MapHeight / chunkHeight);

            grid = new Chunk[chunksX + 1, chunksY + 1]; // +1 for safety incase objects are right up against the edge of map 

            // Create chunk data
            for (var y = 0; y < grid.GetLength(1); ++y)
                for (var x = 0; x < grid.GetLength(0); ++x)
                    grid[x, y] = new Chunk(this, x, y);
        }

        public ICollidable[] GetNearbyCollidables(ICollidable actor)
        {
            int chunkIndexX = (actor.CollisionBox.X / chunkWidth);
            int chunkIndexY = (actor.CollisionBox.Y / chunkHeight);

            var chunks = new Chunk[]
            {
                // Get own chunk
                tryGetChunk(chunkIndexX, chunkIndexY),

                // Get surrounding chunks
                tryGetChunk(chunkIndexX - 1, chunkIndexY),
                tryGetChunk(chunkIndexX - 1, chunkIndexY - 1),
                tryGetChunk(chunkIndexX,     chunkIndexY - 1),
                tryGetChunk(chunkIndexX + 1, chunkIndexY - 1),
                tryGetChunk(chunkIndexX + 1, chunkIndexY),
                tryGetChunk(chunkIndexX + 1, chunkIndexY + 1),
                tryGetChunk(chunkIndexX,     chunkIndexY + 1),
                tryGetChunk(chunkIndexX - 1, chunkIndexY + 1),
            };

            var collidables = chunks.Where(t => t != null)      // Ensure chunk was found
                                   .SelectMany(t => t.Entities) // Flatten
                                   .Distinct()                  // Remove dupes
                                   .Where(e => e != actor && e.IsCollisionEnabled) // Ignore self, and only look at enableds
                                   .ToArray();

            return collidables;
        }

        public bool UpdateEntity(ICollidable entity)
        {
            var center = entity.CollisionBox.Center;
            var xIndex = (int)(center.X / chunkWidth);
            var yIndex = (int)(center.Y / chunkHeight);

            if (index.ContainsKey(entity))
            {
                var oldIndexes = index[entity];

                var changed = (oldIndexes.X != xIndex || oldIndexes.Y != yIndex);
                if (changed)
                {
                    // Remove from old chunk
                    var oldChunk = grid[oldIndexes.X, oldIndexes.Y];
                    oldChunk.Entities.Remove(entity);

                    // Add to new chunk
                    var newChunk = tryGetChunk(xIndex, yIndex);
                    if (newChunk != null)
                    {
                        newChunk.Entities.Add(entity);
                        index[entity] = new Point(xIndex, yIndex); // update index
                        return true;
                    }
                }
                else
                {
                    // If not changed then still a success
                    return true;
                }
            }
            else
            {
                var chunk = tryGetChunk(xIndex, yIndex);
                if (chunk != null)
                {
                    chunk.Entities.Add(entity);
                    index.Add(entity, new Point(xIndex, yIndex));
                    return true;
                }
            }

            return false;
        }

        public void RemoveFromMap(ICollidable entity)
        {
            if (!index.ContainsKey(entity))
                return;

            // Find chunk
            var indexes = index[entity];
            var chunk = grid[indexes.X, indexes.Y];

            chunk.Entities.Remove(entity);

        }

        Chunk tryGetChunk(int xIndex, int yIndex)
        {
            if (xIndex >= 0 && xIndex < grid.GetLength(0) &&
                yIndex >= 0 && yIndex < grid.GetLength(1))
            {
                return grid[xIndex, yIndex];
            }
            else
            {
                return null;
            }
        }

    }

}

//public struct TileEdges
//{
//    public TileEdges(Vector2 position, int objWidth, int objHeight, int tileWidth, int tileHeight)
//    {
//        this.position = position;
//        this.objWidth = objWidth;
//        this.objHeight = objHeight;
//        this.tileWidth = tileWidth;
//        this.tileHeight = tileHeight;
//    }

//    Vector2 position;
//    int objWidth;
//    int objHeight;
//    int tileWidth;
//    int tileHeight;

//    public int LeftIndex { get { return (int)Math.Round(position.X) / tileWidth; } }
//    public int RightIndex { get { return (int)(Math.Round(position.X) + objWidth) / tileWidth; } }
//    public int TopIndex { get { return (int)Math.Round(position.Y) / tileHeight; } }
//    public int BottomIndex { get { return (int)(Math.Round(position.Y) + objHeight) / tileHeight; } }

//}