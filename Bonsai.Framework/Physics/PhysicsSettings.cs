using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework.Physics
{
    public enum PhysicsType { Platformer, Topdown }

    public class PhysicsSettings
    {
        /// <summary>
        /// Intensity of gravity affecting player while not grounded
        /// </summary>
        public float Gravity { get; set; }
        public bool HasGravity => Gravity != 0f;

        public PhysicsType PhysicsType { get; set; }

        /// <summary>
        /// Max speed of any object
        /// </summary>
        public float TerminalVelocity { get; set; }
        public bool HasTerminalVelocity => TerminalVelocity != 0f;

        /// <summary>
        /// 0f to 1f to represent intensity of friction when entity grounded
        /// </summary>
        public float Friction { get; set; }
        public bool HasFriction => Friction != 0f;

    }
}
