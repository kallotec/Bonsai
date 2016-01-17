using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bonsai.Framework.Collision
{
    public enum AABBSide { Left, Top, Right, Bottom }

    public static class AABBNormals
    {
        public static Vector2 Left = new Vector2(-1, 0);
        public static Vector2 Top = new Vector2(0, -1);
        public static Vector2 Right = new Vector2(1, 0);
        public static Vector2 Bottom = new Vector2(0, 1);
    }

}
