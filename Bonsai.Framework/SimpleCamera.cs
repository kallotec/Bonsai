using Bonsai.Framework.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public enum CameraElasticMode { Instant, Lerp, Cubic }

    public class SimpleCamera : Bonsai.Framework.ICamera
    {
        public SimpleCamera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        Matrix transform;
        Viewport viewport;
        Actor focusedActor;
        Vector2? focusedPoint;

        Vector2 current;
        CameraElasticMode elasticMode = CameraElasticMode.Cubic;

        public Matrix Transform
        {
            get { return transform; }
        }
        public Vector2 CurrentFocus => current;
        public Vector2 TargetFocus
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
            if (current == TargetFocus)
                return;

            // move current focus based on target and mode
            switch (elasticMode)
            {
                case CameraElasticMode.Instant:
                    current = TargetFocus;
                    break;

                case CameraElasticMode.Lerp:
                    current.X = MathHelper.Lerp(current.X, TargetFocus.X, 0.3f);
                    current.Y = MathHelper.Lerp(current.Y, TargetFocus.Y, 0.3f);
                    break;

                case CameraElasticMode.Cubic:
                    current.X = MathHelper.SmoothStep(current.X, TargetFocus.X, 0.3f);
                    current.Y = MathHelper.SmoothStep(current.Y, TargetFocus.Y, 0.3f);
                    break;
            }

            // offset by viewport size
            var center = new Vector2(current.X - viewport.Width / 2, current.Y - viewport.Height / 2);

            // stop camera going outside map left
            if (center.X < current.X - viewport.Width / 2)
                center.X = current.X - viewport.Width / 2;

            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * 
                        Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0));

        }

        public void SetFocus(Actor focusedActor, bool immediateFocus)
        {
            this.focusedPoint = null;
            this.focusedActor = focusedActor;

            if (immediateFocus)
                current = TargetFocus;
        }

        public void SetFocus(Vector2 focusedPoint, bool immediateFocus)
        {
            this.focusedActor = null;
            this.focusedPoint = focusedPoint;

            if (immediateFocus)
                current = TargetFocus;
        }

    }
}
