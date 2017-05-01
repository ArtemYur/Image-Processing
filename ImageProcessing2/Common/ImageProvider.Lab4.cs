using System.Collections.Generic;

namespace ImageProcessing2.Common
{
    public static partial class ImageProvider
    {
        public static readonly int[]
            Se =
            {
                -3, -3, -3,
                -3, 0, 5,
                -3, 5, 5
            },
            N = {
                5, 5, 5,
                -3, 0, -3,
                -3, -3, -3
            },
            S =
            {
                -3, -3, -3,
                -3, 0, -3,
                5, 5, 5
            };

        private const string SeMask = "Se";
        private const string SMask = "S";
        private const string NMask = "N";

        public static Image GetFilteredImageWithSeMask(this Dictionary<string, Image> imageTable)
        {
            return imageTable.GetFilteredImageWithMask(SeMask);
        }

        public static Image GetFilteredImageWithSMask(this Dictionary<string, Image> imageTable)
        {
            return imageTable.GetFilteredImageWithMask(SMask);
        }

        public static Image GetFilteredImageWithNMask(this Dictionary<string, Image> imageTable)
        {
            return imageTable.GetFilteredImageWithMask(NMask);
        }

        private static Image GetFilteredImageWithMask(this Dictionary<string, Image> imageTable, 
            string maskName)
        {
            if (!imageTable.ContainsKey(maskName) || imageTable[maskName] == null)
                imageTable[maskName] = imageTable.GetFilteredImage(
                    (int[])typeof(ImageProvider).GetField(maskName).GetValue(typeof(ImageProvider)));

            return imageTable[maskName];
        }
        
        public static Image GetFilteredImage(this Dictionary<string, Image> imageTable,
            int[] mask)
        {
            var originalImage = imageTable.GetOriginalImage();

            var filteredImage = GetImageWithSameDimentions(originalImage);

            for (int x = 0; x < originalImage.Height; x++)
            {
                for (int y = 0; y < originalImage.Width; y++)
                {
                    var point = y * originalImage.Height + x;

                    if (IsBoundariesCrossed(x, originalImage.Height) ||
                        IsBoundariesCrossed(y, originalImage.Width))
                    {
                        filteredImage.ImageArray[point] =
                            GetLinearFiltrationValueOnBoundry(originalImage, x, y, mask);
                    }
                    else
                    {
                        filteredImage.ImageArray[point] = 
                            GetDirectLinearFiltrationValue(originalImage, point, mask);
                    }
                }
            }

            return filteredImage;
        }

        private static byte GetLinearFiltrationValueOnBoundry(
            Image image, int x, int y, int[] mask)
        {
            int value = 0;
            var mean = CalculateMeanForBoundryPoint(image, x, y);
            var point = y * image.Height + x;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var boardValue = IsBoundariesCrossed(x - i, image.Width) ||
                                     IsBoundariesCrossed(y - j, image.Height)
                        ? mean
                        : image.ImageArray[point - i - j * image.Width];

                    value += boardValue * mask[i + 1 + (j + 1) * 3];
                }
            }
            return ConvertToByte(value);
        }

        private static byte CalculateMeanForBoundryPoint(Image image, int x, int y)
        {
            var mean = 0;
            var counter = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if(IsBoundariesCrossed(x + i, image.Width) || 
                        IsBoundariesCrossed(y + j, image.Height))
                        continue;

                    counter++;
                    mean += image.ImageArray[x + i + (y + j) * image.Width];
                }
            }
            return ConvertToByte(mean / counter);
        }

        private static byte GetDirectLinearFiltrationValue(
            Image image, int point, int[] mask)
        {
            int value = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    value += image.ImageArray[point - i - j * image.Width]
                              * mask[i + 1 + (j + 1) * 3];
                }
            }
            return ConvertToByte(value);
        }

        private static byte ConvertToByte(int value)
        {
            if (value < byte.MinValue) value = byte.MinValue;
            if (value > byte.MaxValue) value = byte.MaxValue;
            return (byte)value;
        }

        private static bool IsBoundariesCrossed(int i, int length)
        {
            return i - 1 < 0 || i + 1 >= length;
        }
    }
}
