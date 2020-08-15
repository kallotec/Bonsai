using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skavenger.Game
{
    public class Wall : DrawableBase, Bonsai.Framework.ILoadable, Bonsai.Framework.IDrawable, Bonsai.Framework.IUpdateable, Bonsai.Framework.ICollidable
    {
        public Wall(RectangleF rect, Color color)
        {
            this.CollisionBox = rect;
            this.color = color;
        }

        Color color;
        Texture2D texture;
        public Vector2 Position { get; set; }
        public RectangleF CollisionBox { get; private set; }
        public bool IsCollisionEnabled => true;
        public bool IsOverlappingEnabled => true;
        public bool IsDisabled => false;


        public void Load(IContentLoader loader)
        {
            if (texture == null)
                texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(texture, (Rectangle)CollisionBox, color);
        }

        public void OnOverlapping(object actor)
        {
        }


    }
}
