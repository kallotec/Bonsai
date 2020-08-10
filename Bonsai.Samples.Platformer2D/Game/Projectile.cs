using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Projectile : Actor, ILoadable, Framework.IUpdateable, Framework.IDrawable, IDeletable, ICollidable
    {
        public Projectile(Vector2 position, Vector2 power)
        {
            base.Props.TopSpeed = 1000f;
            base.Props.HasGravity = true;
            base.Props.Weight = 0.2f;
            base.Props.Position = position;
            base.Props.AddForce(power);
        }

        RectangleF projectileSize = new RectangleF(0, 0, 1, 1);
        Texture2D texture;
        public bool IsDisabled => false;
        public bool IsHidden { get; set; } = false;
        public DrawOrderPosition DrawOrder => DrawOrderPosition.Foreground;
        public bool IsAttachedToCamera => false;
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => false;
        public RectangleF CollisionBox => new RectangleF(base.Position.X, base.Position.Y, projectileSize.Width, projectileSize.Height);


        public void Load(IContentLoader loader)
        {
            texture = FrameworkGlobals.Pixel;
        }

        public void Unload()
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(texture, (Rectangle)CollisionBox, Color.WhiteSmoke);
        }

        public void Update(GameTime time)
        {
        }

        public void OnOverlapping(object actor)
        {
            if (actor is Platform)
                base.DeleteMe = true;
        }

    }
}
