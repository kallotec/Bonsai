using Bonsai.Framework;
using Bonsai.Framework.Actors;
using Bonsai.Framework.ContentLoading;
using Bonsai.Framework.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Skavenger.Game
{
    public class Projectile : Actor, ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, IDeletable, IPhysicsObject
    {
        public Projectile(Vector2 position, float angle, int power, Texture2D bulletTexture)
        {
            base.Props.Position = position;
            this.texture = bulletTexture;
            originalPosition = position;
            base.Props.TopSpeed = 10000f;
            base.Props.HasGravity = false;
            base.Props.Rotation = angle;

            // calc velocity
            var velocity = Bonsai.Framework.Maths.GameMathHelper.PlotVector(angle, power, position);
            base.Props.AddForce(velocity);

        }

        RectangleF projectileSize = new RectangleF(0, 0, 1, 1);
        Texture2D texture;
        Vector2 originalPosition;
        Vector2 destructionPosition;
        public bool IsDisabled => false;
        public bool IsHidden { get; set; } = false;
        public DrawOrderPosition DrawOrder => DrawOrderPosition.Foreground;
        public bool IsAttachedToCamera => false;
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => false;
        public RectangleF CollisionBox => new RectangleF(base.Position.X, base.Position.Y, projectileSize.Width, projectileSize.Height);
        public RectangleF OverlapBox => CollisionBox;


        public void Load(IContentLoader loader)
        {
        }

        public void Unload()
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(texture, Position, null, Color.WhiteSmoke, Props.Rotation, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
        }

        public void Update(GameTime time)
        {
        }

        public void OnOverlapping(object actor)
        {
            Debug.WriteLine($"Projectile.OnOverlapping = {actor.GetType().Name}");

            if (actor is Wall)
                destroyProjectile();
        }

        public void OnCollision(object actor)
        {
        }

        void destroyProjectile()
        {
            destructionPosition = base.Position;
            var distance = Vector2.Distance(originalPosition, destructionPosition);
            Debug.WriteLine($"Projectile traveled: {distance}");

            base.DeleteMe = true;
        }

    }
}
