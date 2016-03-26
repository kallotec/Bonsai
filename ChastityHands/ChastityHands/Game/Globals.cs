using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChastityHands.Game
{
    public static class Globals
    {
        public static SpriteFont GeneralFont { get; set; }
        
        public static Rectangle Viewport
        {
            get 
            {
                var vp = GameManager.Instance.GraphicsDevice.Viewport;
                return new Rectangle(vp.X, vp.Y, vp.Width, vp.Height);
            }
        }

        public static Vector2 Viewport_Centerpoints
        {
            get
            {
                return new Vector2(Viewport.X + (Viewport.Width / 2),
                                   Viewport.Y + (Viewport.Height / 2));
            }
        }
        
    }
}
