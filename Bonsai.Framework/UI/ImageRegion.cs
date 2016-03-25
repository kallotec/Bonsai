//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Bonsai.Framework.UI
//{
    
//    public class ImageRegion
//    {
//        /// <summary>
//        /// Defaults Color to Color.White for drawing
//        /// </summary>
//        public ImageRegion()
//        {
//            Color = Color.White;
//            PulseDepth = ePulseDepth.Moderate;
//            PulseSpeed = ePulseSpeed.Medium;
//        }

//        Rectangle r;
//        Texture2D t;
//        Color c;
//        byte pulse_threshold_low = (int)ePulseDepth.Moderate;
//        byte pulse_threshold_high = 255;
//        int pulse_movement = (int)ePulseSpeed.Medium;
//        ePulseSpeed pulse_speed = ePulseSpeed.Medium;
//        ePulseDepth pulse_depth = ePulseDepth.Moderate;

//        public Rectangle Rectangle 
//        {
//            get { return r; }
//            set
//            {
//                r = value;
//            }
//        }
//        public Texture2D Texture 
//        {
//            get { return t; }
//            set
//            {
//                t = value;
//                //why this check?!?!
//                if (r.Width == 0 && r.Height == 0)
//                {
//                    r.Width = t.Width;
//                    r.Height = t.Height;
//                }
//            }
//        }
//        public Color Color 
//        {
//            get { return c; }
//            set
//            {
//                c = value;
//            }
//        }
//        public int X
//        {
//            get { return r.X; }
//            set
//            {
//                r.X = value;
//            }
//        }
//        public int Y
//        {
//            get { return r.Y; }
//            set
//            {
//                r.Y = value;
//            }
//        }
//        public int Width
//        {
//            get { return r.Width; }
//            set { r.Width = value; }
//        }
//        public int Height
//        {
//            get { return r.Height; }
//            set { r.Height = value; }
//        }

//        public bool IsPulsing { get; set; }

//        public ePulseSpeed PulseSpeed
//        {
//            get
//            {
//                return pulse_speed;
//            }
//            set
//            {
//                pulse_speed = value;
//                pulse_movement = (int)pulse_speed;
//            }
//        }

//        public ePulseDepth PulseDepth
//        {
//            get
//            {
//                return pulse_depth;
//            }
//            set
//            {
//                pulse_depth = value;
//                pulse_threshold_low = (byte)(int)pulse_depth;
//            }
//        }

//        public ePulseChannelSelection PulseChannel { get; set; }

//        public void Update(GameTime gameTime)
//        {
//            if (IsPulsing)
//                update_pulsing(gameTime);
//        }

//        private void update_pulsing(GameTime gameTime)
//        {
//            byte channel = 0;

//            switch (PulseChannel)
//            {
//                case ePulseChannelSelection.R:
//                    channel = c.R;
//                    break;
//                case ePulseChannelSelection.G:
//                    channel = c.G;
//                    break;
//                case ePulseChannelSelection.B:
//                    channel = c.B;
//                    break;
//            }

//            if (channel >= pulse_threshold_high)
//                pulse_movement = (int)pulse_speed * -1;

//            if (channel <= pulse_threshold_low)
//                pulse_movement = (int)pulse_speed * 1;

//            switch (PulseChannel)
//            {
//                case ePulseChannelSelection.R:
//                    c.R = (byte)MathHelper.Clamp(c.R + pulse_movement, pulse_threshold_low, pulse_threshold_high);
//                    break;
//                case ePulseChannelSelection.G:
//                    c.G += (byte)MathHelper.Clamp(c.G + pulse_movement, pulse_threshold_low, pulse_threshold_high);
//                    break;
//                case ePulseChannelSelection.B:
//                    c.B += (byte)MathHelper.Clamp(c.B + pulse_movement, pulse_threshold_low, pulse_threshold_high);
//                    break;
//            }
            
//        }


//        public void Draw(SpriteBatch batch)
//        {
//            if (Texture == null)
//                batch.Draw(Globals.Pixel, Rectangle, c);
//            else
//                batch.Draw(Texture, Rectangle, c);
//        }

//        public void Draw(SpriteBatch batch, Color force_to_color)
//        {
//            if (Texture == null)
//                batch.Draw(Globals.Pixel, Rectangle, force_to_color);
//            else
//                batch.Draw(Texture, Rectangle, force_to_color);
//        }

//    }
//}
