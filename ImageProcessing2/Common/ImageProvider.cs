using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageProcessing2.Common
{
    public static class ImageProvider
    {
        private const string Original = "Original";
        private const string Normalized = "Normalized";
        private const string Inverse = "Inverse";

        public static float Min = 0.6f;
        public static float Max = 1f;

        public static void UpsertOriginalImage(this Dictionary<string, Image> imageTable, 
            Image image)
        {
            imageTable.Add(Original, image);
            imageTable[Normalized] = null;
            imageTable[Inverse] = null;
        }

        public static Image GetOriginalImage(this Dictionary<string, Image> imageTable)
        {
            if (imageTable.ContainsKey(Original))
                return imageTable[Original];

            throw new ArgumentNullException("Original image wasn't seted up!");
        }

        public static Image GetNormalizedImage(this Dictionary<string, Image> imageTable)
        {
            if (!imageTable.ContainsKey(Normalized) || imageTable[Normalized] == null)
                imageTable.AddNormalizedImage();

            return imageTable[Normalized];
        }

        public static Image GetInverseImage(this Dictionary<string, Image> imageTable)
        {
            if (!imageTable.ContainsKey(Inverse) || imageTable[Inverse] == null)
                imageTable.AddInverseImage();

            return imageTable[Inverse];
        }

        private static void AddNormalizedImage(this Dictionary<string, Image> imageTable)
        {
            var original = imageTable.GetOriginalImage();

            var normalizedImage = GetImageWithSameDimentions(original);

            float m = original.ImageArray.Max();
            float min = m*Min, max = m*Max;

            for (int i = 0; i < normalizedImage.ImageArray.Length; i++)
                normalizedImage.ImageArray[i] =
                        (byte)((original.ImageArray[i] - min) / (max - min) * m);

            imageTable[Normalized] = normalizedImage;
        }

        private static void AddInverseImage(this Dictionary<string, Image> imageTable)
        {
            var original = imageTable.GetOriginalImage();

            var inverseImage = GetImageWithSameDimentions(original);

            float m = original.ImageArray.Max();

            for (int i = 0; i < inverseImage.ImageArray.Length; i++)
                inverseImage.ImageArray[i] = (byte)(m - original.ImageArray[i]);

            imageTable[Inverse] = inverseImage;
        }

        private static Image GetImageWithSameDimentions(Image image)
        {
            return new Image
            {
                Height = image.Height,
                Width = image.Width,
                ImageArray = new byte[image.ImageArray.Length]
            };
        }
    }
}