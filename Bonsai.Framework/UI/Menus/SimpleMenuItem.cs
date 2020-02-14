//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Bonsai.Framework.UI.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Bonsai.Framework.UI.Menus
//{
//    public interface ISimpleMenuItem
//    {
//        Rectangle Rectangle { get; }
//        Vector2 Position { get; set; }
//        int IsHighlighted { get; set; }
//        int Selected_Index { get; }
//        Color Color_Label { get; set; }
//        Color Color_Values { get; set; }
//        Color Color_Selection { get; set; }

//        void ShiftLeft();
//        void ShiftRight();
//        void Draw(SpriteBatch batch);
//    }

//    public class SimpleMenuItem<T> : ISimpleMenuItem
//    {
//        public SimpleMenuItem(SpriteFont font, string label)
//        {
//            this.font = font;
//            this.label = new TextElement<string>(font, label, label, eDisplayMode.LabelOnly);
//            this.label.Alignment = FieldAlignmentMode.Right;
//            this.rectangle = new Rectangle(0, 0, (int)font.MeasureString(label).X, (int)font.MeasureString(label).Y);
//        }

//        SpriteFont font;
//        TextElement<string> label;
//        List<TextElement<T>> items = new List<TextElement<T>>();
//        Rectangle rec_selection = new Rectangle();
//        int selected_index;
//        Rectangle rectangle;
//        Vector2 position;
//        int gap_labelandvalue = 20;
//        int gap_interitem = 4;

//        public Rectangle Rectangle
//        {
//            get { return rectangle; }
//        }
//        public Vector2 Position
//        {
//            get { return position; }
//            set
//            {
//                position = value;

//                //we want to make sure the menu items are aligned right in the middle of the gap_labelandvalue
//                label.Position = new Vector2(position.X - (gap_labelandvalue / 2), position.Y);

//                //reposition items
//                if (items.Count > 0)
//                {

//                    for (int i = 0; i < items.Count; i++)
//                    {
//                        if (i == 0)
//                        {
//                            items[i].Position = new Vector2(position.X + (gap_labelandvalue / 2), position.Y);
//                        }
//                        else
//                        {
//                            IField last = items[i - 1];
//                            items[i].Position = new Vector2(last.Position.X + last.Rectangle.Width + (gap_labelandvalue / 2),
//                                                             last.Position.Y);
//                        }
//                    }

//                    //value selection box
//                    rec_selection = items[selected_index].Rectangle;
//                    rec_selection.Inflate(2, 0);

//                    //bounding box
//                    rectangle = Rectangle.Union(label.Rectangle, items[items.Count - 1].Rectangle);
//                }
//            }
//        }
//        public int IsHighlighted { get; set; }
//        public int Selected_Index
//        {
//            get { return selected_index; }
//        }
//        public Color Color_Label { get; set; }
//        public Color Color_Values { get; set; }
//        public Color Color_Selection { get; set; }

//        public T Selected_Item
//        {
//            get
//            {
//                return items[selected_index].Value;
//            }
//        }

//        public delegate void delSelectionChanged(T value);
//        public event delSelectionChanged SelectionChanged;


//        public void Add(string label, T value)
//        {
//            if (items.Count == 0)
//            {
//                Field<T> f = new Field<T>(font, label, value, eDisplayMode.LabelOnly);
//                f.Position = new Vector2(position.X + (gap_labelandvalue / 2), position.Y);
//                items.Add(f);

//                rec_selection = f.Rectangle;
//                rec_selection.Inflate(2, 0);
//            }
//            else
//            {
//                Field<T> f = new Field<T>(font, label, value, eDisplayMode.LabelOnly);
//                Field<T> last = items[items.Count - 1];
//                f.Position = new Vector2(last.Position.X + last.Rectangle.Width + gap_interitem,
//                                         last.Position.Y);
//                items.Add(f);
//            }

//            //adjust the overall bounding box
//            rectangle = Rectangle.Union(this.label.Rectangle, items[items.Count - 1].Rectangle);

//        }

//        public void ShiftLeft()
//        {
//            if (selected_index > 0)
//            {
//                selected_index--;

//                rec_selection = items[selected_index].Rectangle;
//                rec_selection.Inflate(2, 0);

//                //raise selection changed event
//                if (SelectionChanged != null)
//                    SelectionChanged(items[selected_index].Value);
//            }
//        }

//        public void ShiftRight()
//        {
//            if (selected_index < items.Count - 1)
//            {
//                selected_index++;

//                rec_selection = items[selected_index].Rectangle;
//                rec_selection.Inflate(2, 0);

//                //raise selection changed event
//                if (SelectionChanged != null)
//                    SelectionChanged(items[selected_index].Value);
//            }
//        }

//        public void Draw(SpriteBatch batch)
//        {
//            ////draw bounding box
//            //batch.Draw(Globals.Pixel, rectangle, Color.ForestGreen);

//            //label
//            label.Draw(batch, Color_Label, false);

//            //selection box
//            batch.Draw(Globals.Pixel, rec_selection, Color_Selection);

//            //values
//            for (int i = 0; i < items.Count; i++)
//            {
//                if (i == selected_index)
//                    items[i].Draw(batch, Color.Black, false);
//                else
//                    items[i].Draw(batch, Color_Values, false);
//            }
//        }

//    }
//}
