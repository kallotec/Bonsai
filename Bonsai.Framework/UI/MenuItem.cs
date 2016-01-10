using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Bonsai.Framework;
using Bonsai.Framework.Common;

namespace Bonsai.Framework.UI
{
    public class MenuItem : IMenuItem
    {
        public MenuItem(StringBuilder label, Vector2 position, SpriteFont spriteFont)
        {
            font = spriteFont;
            configureKeyListeners();

            Label = label; 
            Position = position;

            leftArrowLength = (int)font.MeasureString(leftArrow).X;
            rightArrowLength = (int)font.MeasureString(rightArrow).X;
        }

        private void configureKeyListeners()
        {
            this.keyListener_Left = new KeyPressListener(Keys.Left, new KeyPressListener.delKeyPressed(onKeyPress_Left));
            this.keyListener_Right = new KeyPressListener(Keys.Right, new KeyPressListener.delKeyPressed(onKeyPress_Right));
            this.keyListener_Enter = new KeyPressListener(Keys.Enter, new KeyPressListener.delKeyPressed(onKeyPress_Enter));
        }

        //local
        int pad = 2;
        Vector2 pos_label;
        Vector2 pos_value;
        Rectangle box_label_text;
        Rectangle box_value_text;
        Rectangle box_label;
        Rectangle box_value;

        StringBuilder label;
        SpriteFont font;
        Vector2 position;
        object value;
        bool isHidden;
        string leftArrow = "<";
        int leftArrowLength;
        string rightArrow = ">";
        int rightArrowLength;
        Color indicatorColor = Color.GreenYellow;
        Vector2 indicatorOffset = new Vector2(-12, 0);
        Vector2 valueOffset = new Vector2(100, 0);

        KeyPressListener keyListener_Left;
        KeyPressListener keyListener_Right;
        KeyPressListener keyListener_Enter;
        
        //publics
        public StringBuilder Label 
        {
            get { return label; }
            set
            {
                label = value;

                box_label_text.X = (int)position.X + pad;
                box_label_text.Y = (int)position.Y + pad;
                box_label_text.Width = (int)font.MeasureString(label).X;
                box_label_text.Height = (int)font.MeasureString(label).Y;

                box_label = box_label_text;
                box_label.Inflate(pad, pad);
            }
        }
        public Vector2 Position 
        {
            get { return position; }
            set 
            { 
                position = value;
                
                //position
                pos_label.X = position.X + pad;
                pos_label.Y = position.Y + pad;

                box_label_text.X = (int)pos_label.X;
                box_label_text.Y = (int)pos_label.Y;

                box_label = box_label_text;
                box_label.Inflate(pad, pad);

                if (IsValueAttached)
                {
                    //value
                    pos_value.X = position.X + valueOffset.X;
                    pos_value.Y = position.Y + valueOffset.Y;

                    box_value_text.X = (int)pos_value.X;
                    box_value_text.Y = (int)pos_value.Y;
                    box_value_text.Width = (int)font.MeasureString(this.value.ToString()).X;
                    box_value_text.Height = (int)font.MeasureString(this.value.ToString()).Y;

                    box_value = box_value_text;
                    box_value.Inflate(pad, pad);
                }
            }
        }
        public object Value
        {
            get { return value; }
            set
            {
                this.value = value;

                if (this.value is ValueType)
                {
                    pos_value.X = position.X + valueOffset.X;
                    pos_value.Y = position.Y + valueOffset.Y;

                    box_value_text.X = (int)pos_value.X;
                    box_value_text.Y = (int)pos_value.Y;
                    box_value_text.Width = (int)font.MeasureString(this.value.ToString()).X;
                    box_value_text.Height = (int)font.MeasureString(this.value.ToString()).Y;

                    box_value = box_value_text;
                    box_value.Inflate(pad, pad);
                }
                else
                {
                    throw new NotImplementedException("Does not work with non-value types");
                }
            }
        }
        public List<object> ValueList { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsOpen { get; set; }
        public bool IsValueAttached
        {
            get
            {
                return (Value != null);
            }
        }
        public bool IsValueChangeable
        {
            get
            {
                return (Value != null && ValueList != null);
            }
        }
        public delMenuItemClick Clicked { get; set; }
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
        }
        public Rectangle Bounds
        {
            get 
            {
                if (IsValueAttached)
                {
                    Rectangle r = Rectangle.Union(box_label, box_value);
                    r.X = (int)position.X;
                    r.Y = (int)position.Y;
                    return r;
                }
                else
                {
                    return box_label;
                }
            }
        }

        private void onKeyPress_Left()
        {
            if (IsHighlighted && IsValueAttached && IsValueChangeable)
                this.bumpValueDown();
        }
        private void onKeyPress_Right()
        {
            if (IsHighlighted && IsValueAttached && IsValueChangeable)
                this.bumpValueUp();
        }
        private void onKeyPress_Enter()
        {
            if (IsHighlighted && IsValueAttached == false)
                if (Clicked != null)
                    Clicked();
        }

        private void bumpValueUp()
        {
            for (int i = 0; i < ValueList.Count; i++)
            {
                if (ValueList[i].Equals(Value))
                {
                    if (i < ValueList.Count - 1)
                    {
                        Value = ValueList[i + 1];
                        break;
                    }
                }
            }
        }

        private void bumpValueDown()
        {
            for (int i = 0; i < ValueList.Count; i++)
            {
                if (ValueList[i].Equals(Value))
                {
                    if (i > 0)
                    {
                        Value = ValueList[i - 1];
                        break;
                    }
                }
            }
        }


        public void Update(GameTime time)
        {
            //update key listeners
            if (IsValueAttached)
            {
                keyListener_Left.Update(time);
                keyListener_Right.Update(time);
            }
            else
            {
                keyListener_Enter.Update(time);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsHidden)
                return;

            //box
            spriteBatch.Draw(Globals.Pixel_HalfTransparent, Bounds, Color.Black);

            //text
            spriteBatch.DrawString(font, Label, pos_label, (IsHighlighted ? Color.White : Color.Gray));

            //draw indicator
            if (IsHighlighted && !IsValueAttached)
                spriteBatch.DrawString(font, rightArrow, Position + indicatorOffset, indicatorColor);

            //draw value if attached
            if (IsValueAttached)
            {
                //left arrow
                if (IsHighlighted)
                    spriteBatch.DrawString(font, leftArrow,
                                           pos_value + new Vector2(-leftArrowLength, 0),
                                           indicatorColor);

                //text
                spriteBatch.DrawString(font, Value.ToString(), pos_value, (IsHighlighted ? Color.White : Color.Gray));

                //right arrow
                if (IsHighlighted)
                    spriteBatch.DrawString(font, rightArrow, 
                                           pos_value + new Vector2(box_value_text.Width, 0), 
                                           indicatorColor);

            }
        }
    }
}
