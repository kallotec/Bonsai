using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class SimpleCamera : Bonsai.Framework.ICamera
    {
        public SimpleCamera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        Matrix transform;
        Vector2 center;
        Viewport viewport;
        Actor focusedActor;
        Vector2? focusedPoint;

        public Matrix Transform
        {
            get { return transform; }
        }
        public Vector2 Focus
        {
            get
            {
                return (focusedActor != null
                          ? focusedActor.Props.Position
                          : focusedPoint ?? new Vector2(0));
            }
        }
        public bool IsDisabled { get; private set; }


        public void Update(GameTime time)
        {
            // Decide what to focus on
            var focus = Focus;

            // TODO: offset by viewport size, I think?
            center = new Vector2(focus.X - viewport.Width / 2, focus.Y - viewport.Height / 2);

            // Stop camera going outside map left
            if (center.X < focus.X - viewport.Width / 2)
                center.X = focus.X - viewport.Width / 2;

            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * 
                        Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0));

        }

        public void SetFocus(Actor focusedActor)
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
