using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Processing.Lab_3
{
    static class Converter
    {
        public static Bitmap ToBitmap(this List<byte> pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height); // экземпляр точечного рисунка
            int index = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(pixels[index], pixels[index], pixels[index]));
                    index++;
                }
            }
            return bitmap;
        }

        public static Bitmap Bckg(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    if (j < height / 2)
                        bitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255)); // чёрный 
                    else
                        bitmap.SetPixel(i, j, Color.FromArgb(0, 0, 0)); // белый
                }
            return bitmap;
        }

        public static Bitmap bckgAdd(this List<byte> pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Bitmap image = ToBitmap(pixels, width, height);
            Bitmap bckg = Bckg(width, height);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)

                    bitmap.SetPixel(i, j, Color.FromArgb(
                        bckg.GetPixel(i, j).R & image.GetPixel(i, j).R,
                        bckg.GetPixel(i, j).G & image.GetPixel(i, j).G,
                        bckg.GetPixel(i, j).B & image.GetPixel(i, j).B));
            return bitmap;
        }

        public static Bitmap ColorChange(this List<byte> pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            int index = 0;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(pixels[index], 0, 0)); // изменение на красный
                    index++;
                }
            return bitmap;
        }
    }
}

