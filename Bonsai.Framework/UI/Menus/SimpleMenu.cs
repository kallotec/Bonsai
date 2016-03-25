//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Bonsai.Framework;
//using Microsoft.Xna.Framework.Input;
//using Bonsai.Framework.Input;

//namespace Bonsai.Framework.UI.Menus
//{
//    public class SimpleMenu
//    {
//        public SimpleMenu(SpriteFont font, Vector2 position, Color color_label, Color color_values, Color color_value_selection, Color color_row_selection)
//        {
//            this.font = font;
//            this.position = position;
//            this.color_label = color_label;
//            this.color_values = color_values;
//            this.color_selection = color_value_selection;

//            region_menuselection = new ImageRegion();
//            region_menuselection.Color = color_row_selection;

//            configureKeyListeners();
//        }

//        SpriteFont font;
//        Color color_label;
//        Color color_values;
//        Color color_selection;
//        List<ISimpleMenuItem> items = new List<ISimpleMenuItem>();
//        List<KeyPressListener> keys = new List<KeyPressListener>();
//        int selected_row = 0;
//        Vector2 position;
//        int gap_row = 4;
//        ImageRegion region_menuselection;

//        public int Selected_Line
//        {
//            get
//            {
//                return selected_row;
//            }
//        }
//        public int Selected_Cell
//        {
//            get
//            {
//                return items[selected_row].Selected_Index;
//            }
//        }
//        public int SelectionBox_Width
//        {
//            get
//            {
//                return region_menuselection.Width;
//            }
//            set
//            {
//                region_menuselection.Width = value;
//            }
//        }
//        public int Menu_Bottom
//        {
//            get
//            {
//                ISimpleMenuItem i = items[items.Count - 1];
//                return (int)i.Position.Y + i.Rectangle.Height;
//            }
//        }
        
//        public void Add(ISimpleMenuItem item)
//        {
//            item.Color_Label = color_label;
//            item.Color_Selection = color_selection;
//            item.Color_Values = color_values;

//            if (items.Count == 0)
//            {
//                item.Position = position;
//            }
//            else
//            {
//                ISimpleMenuItem last = items[items.Count - 1];
//                item.Position = new Vector2(position.X, 
//                                            last.Position.Y + last.Rectangle.Height + gap_row);
//            }

//            items.Add(item);

//            adjust_selection_box();
//        }

//        void configureKeyListeners()
//        {
//            keys.Add(new KeyPressListener(Keys.Up, new KeyPressListener.delKeyPressed(ShiftUp)));
//            keys.Add(new KeyPressListener(Keys.Down, new KeyPressListener.delKeyPressed(ShiftDown)));
//            keys.Add(new KeyPressListener(Keys.Left, new KeyPressListener.delKeyPressed(ShiftLeft)));
//            keys.Add(new KeyPressListener(Keys.Right, new KeyPressListener.delKeyPressed(ShiftRight)));
//        }

//        void adjust_selection_box()
//        {
//            if (items.Count > 0)
//            {
//                int max_left = 9999;
//                int max_right = 0;

//                foreach (ISimpleMenuItem item in items)
//                {
//                    if (item.Rectangle.Left < max_left)
//                        max_left = item.Rectangle.Left;

//                    if (item.Rectangle.Right > max_right)
//                        max_right = item.Rectangle.Right;
//                }

//                region_menuselection.X = max_left - 20;
//                region_menuselection.Y = items[0].Rectangle.Y - (gap_row / 2);

//                region_menuselection.Width = (max_right - max_left) + 40;
//                region_menuselection.Height = items[0].Rectangle.Height + (gap_row);

//            }
//        }


//        public void ShiftUp()
//        {
//            //only shift if physically possible
//            if (selected_row > 0)
//            {
//                //track selected row change
//                selected_row--;

//                //move selection box
//                region_menuselection.Y = (int)items[selected_row].Position.Y - (gap_row / 2);
//            }
//        }

//        public void ShiftDown()
//        {
//            //only shift if physically possible
//            if (selected_row < items.Count - 1)
//            {
//                //track selected row change
//                selected_row++;

//                //move selection box
//                region_menuselection.Y = (int)items[selected_row].Position.Y - (gap_row / 2);
//            }
//        }

//        public void ShiftLeft()
//        {
//            items[selected_row].ShiftLeft();
//        }

//        public void ShiftRight()
//        {
//            items[selected_row].ShiftRight();
//        }


//        public void Update(GameTime time)
//        {
//            foreach (KeyPressListener key in keys)
//                key.Update(time);
//        }

//        public void Draw(SpriteBatch batch)
//        {
//            //selection box
//            region_menuselection.Draw(batch);

//            //menu options
//            foreach (ISimpleMenuItem i in items)
//                i.Draw(batch);
//        }

//    }
//}
