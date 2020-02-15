using Bonsai.Framework;
using Bonsai.Framework.Content;
using Bonsai.Samples.Platformer2D.Game.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Samples.Platformer2D.Game
{
    public class Platform : DrawableBase, Bonsai.Framework.ILoadable, Bonsai.Framework.IDrawable, Bonsai.Framework.IUpdateable, Bonsai.Framework.ICollidable
    {
        public Platform(int x, int y, int w, int h, string fillHex, string strokeHex)
        {
            this.position = new Vector2(x, y);
            this.width = w;
            this.height = h;

            this.fillHex = fillHex;
            this.fillColor = getRgba(fillHex);

            if (!string.IsNullOrWhiteSpace(strokeHex))
            {
                this.strokeColor = getRgba(strokeHex);
                this.drawingColor = strokeColor;
            }
            else
            {
                this.drawingColor = fillColor;
            }
        }

        string fillHex;
        Color drawingColor;
        Color fillColor;
        Color strokeColor;
        Vector2 position;
        int width;
        int height;
        Texture2D texture;
        public bool IsHazardTile => fillHex == "#ff0000";
        SoundEffect sfxCheckpoint;

        public Rectangle CollisionBox => new Rectangle((int)position.X, (int)position.Y, width, height);
        public bool IsCollisionEnabled => true;
        public bool IsOverlappingEnabled => true;
        public bool IsDisabled => false;


        public void Load(IContentLoader loader)
        {
            texture = loader.Load<Texture2D>(ContentPaths.TEX_PIXEL);
            sfxCheckpoint = loader.Load<SoundEffect>(ContentPaths.SFX_CHECKPOINT);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            batch.Draw(texture, CollisionBox, drawingColor);
        }


        public void OnOverlapping(object actor)
        {
            if (drawingColor != fillColor)
            {
                drawingColor = fillColor;
                sfxCheckpoint.Play(0.5f, 0f, 0f);
            }

        }

        Color getRgba(string hex)
        {
            hex = hex.TrimStart('#');
            var r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b);
        }

    }
}
