namespace ImageProcessing2.Common
{
    public class RgbImage<T>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public T[,] ImmageArray { get; set; }
    }
}