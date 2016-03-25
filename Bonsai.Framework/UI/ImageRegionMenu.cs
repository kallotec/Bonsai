//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;

//namespace Bonsai.Framework.UI
//{
//    public class ImageRegionMenu
//    {
//        public ImageRegionMenu(Vector2 pos, int item_gap)
//        {
//            color_unselected = Color.FromNonPremultiplied(60, 60, 60, 255);
//            position = pos;
//            gap = item_gap;
//        }

//        Vector2 position;
//        List<ImageRegion> regions = new List<ImageRegion>();
//        int selected_index = 0;
//        int gap = 10;
//        Color color_selected = Color.White;
//        Color color_unselected;

//        public Vector2 Position
//        {
//            get { return position; }
//        }

//        public int Height
//        {
//            get
//            {
//                if (regions.Count > 0)
//                    return regions[0].Rectangle.Height;
//                else
//                    return 1;
//            }
//        }

//        public int ItemWidth
//        {
//            get
//            {
//                if (regions.Count > 0)
//                    return regions[0].Rectangle.Width;
//                else
//                    return 1;
//            }
//        }

//        public int SelectedIndex
//        {
//            get { return selected_index; }
//            set
//            {
//                selected_index = value;
//            }
//        }


//        /// <summary>
//        /// All regions should be equal in dimensions or unexpected behaviour will occur
//        /// </summary>
//        public void Add(ImageRegion region)
//        {
//            if (regions.Count > 0)
//            {
//                region.X = regions[regions.Count - 1].X + region.Rectangle.Width + gap;
//                region.Y = (int)position.Y;
//            }
//            else
//            {
//                region.X = (int)position.X;
//                region.Y = (int)position.Y;
//            }

//            regions.Add(region);
//        }

//        public void ShuffleLeft()
//        {
//            if (regions.Count == 0 || selected_index == 0)
//                return;

//            int movement = regions[0].Rectangle.Width + gap;

//            foreach (ImageRegion r in regions)
//                r.X += movement;

//            selected_index -= 1;
//        }

//        public void ShuffleRight()
//        {
//            if (regions.Count == 0 || selected_index == regions.Count-1)
//                return;

//            int movement = regions[0].Rectangle.Width + gap;

//            foreach (ImageRegion r in regions)
//                r.X -= movement;

//            selected_index += 1;
//        }


//        public void Draw(SpriteBatch batch)
//        {
//            int i = 0;
//            foreach(ImageRegion r in regions)
//            {
//                batch.Draw(r.Texture, r.Rectangle, (selected_index == i ? color_selected : color_unselected));
//                i += 1;
//            }
//        }

//    }
//}
