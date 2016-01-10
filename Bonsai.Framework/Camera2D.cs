using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework.Common;

namespace Bonsai.Framework
{
    public class Camera2D
    {
        public Camera2D()
        {
            viewbox.Width = Globals.Device.Viewport.Width;
            viewbox.Height = Globals.Device.Viewport.Height;
            screenCenter = new Vector2(viewbox.Width / 2, viewbox.Height / 2);
            Scale = 1;
            Origin = screenCenter;
            Position = new Vector2(0, 0);
            MoveSpeed = 1.25f;
        }

        Vector2 position;
        Rectangle viewbox;
        Vector2 origin;
        float scale;
        Vector2 screenCenter;
        StaticActor focus;

        public Rectangle Box
        {
            get { return viewbox; }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                viewbox.X = (int)(position.X - (viewbox.Width / 2));
                viewbox.Y = (int)(position.Y - (viewbox.Height / 2));
                updateTransform();
            }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                updateTransform();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                Box.Inflate((int)(Box.Width / scale), 
                            (int)(Box.Height / scale));
                updateTransform();
            }
        }

        public Matrix Transform
        {
            get; set;
        }

        public StaticActor Focus
        {
            get { return focus; }
            set
            {
                focus = value;
                Position = focus.Position;
                updateTransform();
            }
        }

        public float MoveSpeed
        {
            get;
            set;
        }


        private void updateTransform()
        {
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0)
                     * Matrix.CreateTranslation(Origin.X, Origin.Y, 0)
                     * Matrix.CreateScale(new Vector3(Scale, Scale, Scale));
        }


        public bool IsInView(Rectangle box)
        {
            return box.Intersects(viewbox);
        }

        public void Update(GameFrame frame)
        {
            ////Note: Do not delete!
            ////note: Currently redundant as the player never moves
            ////      but keep for movement iteration

            //var delta = (float)frame.GameTime.ElapsedGameTime.TotalSeconds;

            //position.X += (Focus.Position.X - Position.X) * MoveSpeed * delta;
            //position.Y += (Focus.Position.Y - Position.Y) * MoveSpeed * delta;
        }

    }
}
