using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bonsai.Framework.Common
{
    public enum TextureFormatterOption { no_shine, low_shine, high_shine, shiny_top_border }

    public static class TextureFormatter
    {
        /// <summary>
        /// WHAT THE FUCK DOES THIS DO???? SRS
        /// </summary>
        public static Texture2D Format(GraphicsDevice device, int width, int height, TextureFormatterOption format, Color color_bg, Color color_border, Color color_shine)
        {
            Texture2D tex = new Texture2D(device, width, height);
            Color[] array = new Color[width * height];
            
            //bg
            for (int x = 0; x < array.Length; x++)
                array[x] = color_bg;

            //border
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    if (y == 0 || y == (height - 1) || x == 0 || x == (width - 1))
                        array[(width * y) + x] = color_border;

            //shine
            switch (format)
            {
                case TextureFormatterOption.low_shine:
                    for (int x = 0; x < width; x++)
                        if (x != 0 && x != (width - 1))
                            for (int y = 1; y < 2; y++)
                                array[(width * y) + x] = color_shine;
                    break;

                case TextureFormatterOption.high_shine:
                    for (int x = 0; x < width; x++)
                        if (x != 0 && x != (width - 1))
                            for (int y = 1; y < height / 2; y++)
                                array[(width * y) + x] = color_shine;
                    break;

                case TextureFormatterOption.shiny_top_border:
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < 1; y++)
                            array[(width * y) + x] = color_shine;
                    break;

                case TextureFormatterOption.no_shine:
                    //do nothing
                    break;
            }

            ////clip the corner
            //array[0] = Color.Transparent;
            //array[width - 1] = Color.Transparent;
            //array[((width * height) - width)] = Color.Transparent;
            array[(width * height) - 1] = Color.Transparent;

            tex.SetData<Color>(array);
            return tex;
        }
    }
}
