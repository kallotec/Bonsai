using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Framework.ContentLoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework;
using Bonsai.Framework.Actors;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Skavenger.Game.Loot;

namespace Skavenger.Game.Loot
{
    public class LootBox : Actor, Bonsai.Framework.ILoadable, Bonsai.Framework.IUpdateable, Bonsai.Framework.IDrawable, Bonsai.Framework.Physics.IPhysicsObject, Bonsai.Framework.IDeletable
    {
        public LootBox()
        {
            base.Props.OverlapRect = new Rectangle(0, 0, 30, 30);
            base.Props.PhysicalRect = new Rectangle(10, 10, 10, 10);
        }

        List<LootItem> lootItems;
        SoundEffect sfxOpen;

        public DrawOrderPosition DrawOrder { get; set; }
        public bool IsAttachedToCamera => false;
        public bool IsDisabled => false;
        public bool IsHidden { get; set; }
        public RectangleF CollisionBox => new RectangleF(Props.Position.X + Props.PhysicalRect.X, Props.Position.Y + Props.PhysicalRect.Y, Props.PhysicalRect.Width, Props.PhysicalRect.Height);
        public bool IsOverlappingEnabled => true;
        public bool IsCollisionEnabled => true;
        public RectangleF OverlapBox => new RectangleF(Props.Position.X + Props.OverlapRect.X, Props.Position.Y + Props.OverlapRect.Y, Props.OverlapRect.Width, Props.OverlapRect.Height);

        public void Load(IContentLoader loader)
        {
            base.Props.Texture = FrameworkGlobals.Pixel;
            sfxOpen = loader.Load<SoundEffect>(ContentPaths.SFX_BOX_OPEN);

            lootItems = new List<LootItem>
            {
                new LootItem
                {
                    Name = "Ammo",
                    Quantity = 20
                }
            };
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
        }

        public void Draw(GameTime time, SpriteBatch batch, Vector2 parentPosition)
        {
            batch.Draw(Props.Texture, Props.Position, (Rectangle)CollisionBox, Props.Tint);
        }

        public void OnOverlapping(object actor)
        {
        }

        public List<LootItem> OpenBox()
        {
            sfxOpen.Play(0.5f, 0f, 0f);
            return lootItems;
        }

        public void OnCollision(object actor)
        {
        }
    }
}
