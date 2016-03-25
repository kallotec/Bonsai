//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Bonsai.Framework.UI
//{
//    public class Field<T> : IField
//    {
//        public Field(SpriteFont font, string label, T value, FieldDisplayMode display_mode)
//        {
//            this.font = font;
//            this.color = Color.White;
//            this.display_mode = display_mode;
//            this.Value = value;
//            this.Label = label;
//        }

//        string label = string.Empty;
//        T value;
//        string label_format = string.Empty;
//        FieldDisplayMode display_mode = FieldDisplayMode.LabelOnly;
//        string label_generated = string.Empty;
//        Vector2 position_origional;
//        Vector2 position_actual;
//        Rectangle rectangle;
//        Vector2 dimensions;
//        SpriteFont font;
//        FieldAlignmentMode alignment;
//        Color color;
//        Color color_icon = Color.White;
//        //Texture2D icon;
//        //Vector2 icon_pos;
//        //byte pulse_threshold_low = (int)ePulseDepth.Moderate;
//        //byte pulse_threshold_high = 255;
//        //int pulse_movement = (int)ePulseSpeed.Medium;
//        //ePulseSpeed pulse_speed = ePulseSpeed.Medium;
//        //ePulseDepth pulse_depth = ePulseDepth.Moderate;

//        public string Label
//        {
//            get
//            {
//                return this.label;
//            }
//            set
//            {
//                this.label = value;
//                process();
//            }
//        }

//        public T Value
//        {
//            get { return value; }
//            set
//            {
//                this.value = value;
//                process();
//            }
//        }

//        public string Format
//        {
//            get { return label_format; }
//            set
//            {
//                label_format = value;
//                process();
//            }
//        }

//        public Vector2 Position
//        {
//            get { return position_origional; }
//            set
//            {
//                position_origional = value;
//                process();
//            }
//        }

//        public Rectangle Rectangle
//        {
//            get { return rectangle; }
//        }

//        public FieldAlignmentMode Alignment
//        {
//            get { return alignment; }
//            set
//            {
//                alignment = value;
//                process();
//            }
//        }

//        public Color Color
//        {
//            get { return color; }
//            set { color = value; }
//        }

//        //public Texture2D Icon
//        //{
//        //    get { return icon; }
//        //    set 
//        //    { 
//        //        icon = value;
//        //        process();
//        //    }
//        //}

//        //public Color Icon_Color
//        //{
//        //    get 
//        //    { 
//        //        if (color_icon == null) 
//        //            return color; 
//        //        else 
//        //            return color_icon; 
//        //    }
//        //    set
//        //    {
//        //        color_icon = value;
//        //    }
//        //}

//        //public bool IsPulsing { get; set; }

//        //public ePulseSpeed PulseSpeed
//        //{
//        //    get
//        //    {
//        //        return pulse_speed;
//        //    }
//        //    set
//        //    {
//        //        pulse_speed = value;
//        //        pulse_movement = (int)pulse_speed;
//        //    }
//        //}

//        //public ePulseDepth PulseDepth
//        //{
//        //    get
//        //    {
//        //        return pulse_depth;
//        //    }
//        //    set
//        //    {
//        //        pulse_depth = value;
//        //        pulse_threshold_low = (byte)(int)pulse_depth;
//        //    }
//        //}

//        //public ePulseChannelSelection PulseChannel { get; set; }


//        void process()
//        {
//            //switch (display_mode)
//            //{
//            //    case DisplayMode.LabelOnly:
//            //        process_labelonly();
//            //        break;

//            //    case DisplayMode.ValueOnly:
//            process_valueonly();
//            //        break;

//            //    case DisplayMode.LabelAndValue:
//            //        process_labelandvalue();
//            //        break;
//            //}

//            dimensions = font.MeasureString(label_generated);

//            switch (alignment)
//            {
//                case FieldTextAlignment.Left:
//                    position_actual = position_origional;
//                    break;

//                case FieldTextAlignment.Right:
//                    position_actual = position_origional - new Vector2(dimensions.X, 0);
//                    break;

//                case FieldTextAlignment.Center:
//                    position_actual = position_origional - new Vector2(dimensions.X / 2, 0);
//                    break;
//            }

//            //if (icon != null)
//            //{
//            //    switch (alignment)
//            //    {
//            //        case FieldTextAlignment.Left:
//            //            icon_pos = position_actual - new Vector2(0, (icon.Height - dimensions.Y) * 0.5f);
//            //            position_actual += new Vector2(icon.Width + 5, 0);
//            //            break;

//            //        case FieldTextAlignment.Right:
//            //            icon_pos = position_actual - new Vector2(icon.Width + 5, (icon.Height - dimensions.Y) * 0.5f);
//            //            break;

//            //        case FieldTextAlignment.Center:
//            //            icon_pos = position_actual - new Vector2(icon.Width + 5, (icon.Height - dimensions.Y) * 0.5f);;
//            //            break;
//            //    }
//            //}

//            rectangle.X = (int)position_actual.X;
//            rectangle.Y = (int)position_actual.Y;
//            rectangle.Width = (int)dimensions.X;
//            rectangle.Height = (int)dimensions.Y;

//        }

//        //void process_labelonly()
//        //{
//        //    this.label_generated = this.label;
//        //}

//        void process_valueonly()
//        {
//            if (typeof(T) == typeof(TimeSpan))
//            {
//                //for datetimes, use the tostring overload for better formatting flexibility
//                this.label_generated = ((TimeSpan)((object)value)).ToString(this.label_format);
//            }
//            else
//            {
//                if (this.label_format.Contains("{0}"))
//                    this.label_generated = string.Format(this.label_format, this.value.ToString());
//                else
//                    this.label_generated = this.value.ToString();
//            }

//        }

//        //void process_labelandvalue()
//        //{
//        //    process_labelonly();

//        //    //assume label now == "{label}";

//        //    if (typeof(T) == typeof(TimeSpan))
//        //    {
//        //        //for datetimes, use the tostring() overload for better formatting flexibility
//        //        label_generated = string.Concat(label_generated, ((TimeSpan)((object)value)).ToString(label_format));
//        //    }
//        //    else
//        //    {
//        //        if (label_format.Contains("{0}"))
//        //            label_generated = string.Concat(label_generated, string.Format(label_format, this.value.ToString()));
//        //        else
//        //            label_generated = string.Concat(label_generated, this.value.ToString());
//        //    }

//        //}


//        public void Update(GameTime gameTime)
//        {
//            //if (IsPulsing)
//            //    update_pulsing(gameTime);
//        }

//        //void update_pulsing(GameTime gameTime)
//        //{
//        //    //byte channel = 0;

//        //    //switch (PulseChannel)
//        //    //{
//        //    //    case ePulseChannelSelection.R:
//        //    //        channel = color.R;
//        //    //        break;
//        //    //    case ePulseChannelSelection.G:
//        //    //        channel = color.G;
//        //    //        break;
//        //    //    case ePulseChannelSelection.B:
//        //    //        channel = color.B;
//        //    //        break;
//        //    //}

//        //    //if (channel >= pulse_threshold_high)
//        //    //    pulse_movement = (int)pulse_speed * -1;

//        //    //if (channel <= pulse_threshold_low)
//        //    //    pulse_movement = (int)pulse_speed * 1;

//        //    //switch (PulseChannel)
//        //    //{
//        //    //    case ePulseChannelSelection.R:
//        //    //        color.R += (byte)pulse_movement;
//        //    //        break;
//        //    //    case ePulseChannelSelection.G:
//        //    //        color.G += (byte)pulse_movement;
//        //    //        break;
//        //    //    case ePulseChannelSelection.B:
//        //    //        color.B += (byte)pulse_movement;
//        //    //        break;
//        //    //}

//        //}


//        public void Draw(SpriteBatch batch)
//        {
//            batch.DrawString(font, label_generated, position_actual, color);

//            //if (icon != null)
//            //    batch.Draw(icon, icon_pos, Icon_Color);
//        }

//        public void Draw(SpriteBatch batch, Color force_to_color, bool recolor_icon_aswell)
//        {
//            batch.DrawString(font, label_generated, position_actual, force_to_color);

//            //if (icon != null)
//            //    batch.Draw(icon, icon_pos, (recolor_icon_aswell ? force_to_color : Icon_Color));
//        }


//        public override string ToString()
//        {
//            if (string.IsNullOrEmpty(label_generated))
//                return base.ToString();
//            else
//                return label_generated;
//        }
//    }
//}
