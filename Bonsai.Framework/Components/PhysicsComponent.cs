using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Components
{
    public class PhysicsComponent
    {
        public void UpdateMovement(PhysicalProperties props, float millisecs)
        {
            props.Velocity.Y = MathHelper.Clamp(props.Velocity.Y + (props.Gravity * 1), -props.TerminalVelocity, props.TerminalVelocity);

            props.Position.Y = (float)Math.Round(props.Position.Y + props.Velocity.Y); // TODO: use (float)frame.GameTime.ElapsedGameTime.TotalSeconds;
        }


        public void ApplyForceY(float force, PhysicalProperties props)
        {
            props.Velocity.Y -= force;
        }

        //public void ApplyForceX(float force, PhysicalProperties props)
        //{
        //    props.Velocity.X -= force;
        //}
    }
}
