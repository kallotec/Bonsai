using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class StandardCamera : BonsaiGameObject, Bonsai.Framework.ICamera
    {
        public StandardCamera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        Matrix transform;
        Vector2 center;
        Viewport viewport;
        StaticActor focusedActor;
        Vector2? focusedPoint;

        public Matrix Transform
        {
            get { return transform; }
        }

        public bool IsDisabled { get; private set; }


        public void Update(GameFrame frame)
        {
            // Decide what to focus on
            var focus = (focusedActor != null 
                            ? focusedActor.Position 
                            : focusedPoint ?? new Vector2(0));

            // TODO: offset by viewport size, I think?
            center = new Vector2(focus.X - viewport.Width / 2, focus.Y - viewport.Height / 2);

            // Stop camera going outside map left
            if (center.X < focus.X - viewport.Width / 2)
                center.X = focus.X - viewport.Width / 2;

            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * 
                        Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0));

        }

        public void SetFocus(StaticActor focusedActor)
        {
            this.focusedPoint = null;
            this.focusedActor = focusedActor;
        }

        public void SetFocus(Vector2 focusedPoint)
        {
            this.focusedActor = null;
            this.focusedPoint = focusedPoint;
        }


    }
}
