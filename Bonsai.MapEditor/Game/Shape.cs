using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonsai.MapEditor.Game
{
    public enum ShapeType { Impassable, Passable, Death }

    public class Shape
    {
        public string StrokeColor { get; set; }
        public string FillColor { get; set; }
        public ShapeType ShapeType { get; set; } = ShapeType.Impassable;
        public List<Vector2> Indices { get; set; } = new List<Vector2>();
    }

}
