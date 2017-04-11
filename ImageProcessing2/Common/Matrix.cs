namespace ImageProcessing2.Common
{
    public class Matrix
    {
        private readonly byte[] _data;
        private readonly int _height;
        private readonly int _width;

        public Matrix(byte[] data, int height, int width)
        {
            _data = data;
            _height = height;
            _width = width;
        }

        public byte this[int x, int y]
        {
            get { return _data[x * _width + y]; }
            set { _data[x * _width + y] = value; }
        }

        public int Heigth => _height;
        public int Width => _width;
        public byte[] GetData() => _data;
    }
}
