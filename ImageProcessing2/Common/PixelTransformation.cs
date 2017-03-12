namespace ImageProcessing2.Common
{
    public class PixelTransformation
    {
        public static byte[] ApplyBufferBackground(byte[] inputImage, int width, int height)
        {
            var outputImage = new byte[inputImage.Length];
            
            for (int i = 0; i < inputImage.Length; i++)
                outputImage[i] = (byte)(inputImage[i] | GetValueFromBuffer(i, width, height));

            return outputImage;
        }

        public static byte[] ApplyColorTransformation(byte[] inputImage, int width, int height)
        {
            var outputImage = new byte[inputImage.Length];

            for (int i = 0; i < inputImage.Length; i++)
                outputImage[i] = GetColorValueFromGradientMeasure(inputImage[i]);

            return outputImage;
        }

        private static byte GetColorValueFromGradientMeasure(byte b)
        {
            return (byte)(128 > b ? b * 2 : 255 - (b - 128) * 2);
        }

        private static byte GetValueFromBuffer(int i, int width, int height)
        {
            return (byte)(height / 2 * width > i ? 0 : 255);
        }
    }
}