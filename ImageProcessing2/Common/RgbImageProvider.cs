using System.Threading.Tasks;
using System.Windows.Forms;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;

namespace ImageProcessing2.Common
{
    public class RgbImageProvider
    {
        private RgbImage<byte> _originalImage;
        private RgbImage<byte> _disjunctionOfOriginalImageWithBackgroundBuffer;
        private RgbImage<byte> _gradientedOriginalImage;
        private int RowsCount => _originalImage.ImmageArray.Length / (_originalImage.ImmageArray.Rank + 1);
        
        private static RgbImage<byte> _backgroundBuffer;
        private readonly Task _loadImageTask;
        
        public RgbImageProvider(string imagePath = null)
        {
            if (imagePath == null)
                imagePath = $"{Application.StartupPath}\\DICOM_Image.dcm";

            _loadImageTask = LoadDcmImage(imagePath);
        }

        public async Task<RgbImage<byte>> GetOriginalImage()
        {
            if (_originalImage == null)
                await _loadImageTask;

            return _originalImage;
        }

        public async Task<RgbImage<byte>> GetDisjunctionOfOriginalImageWithBackgroundBuffer()
        {
            if (_disjunctionOfOriginalImageWithBackgroundBuffer == null)
                await DisjunctOriginalImageWithBackgroundBuffer();

            return _disjunctionOfOriginalImageWithBackgroundBuffer;
        }

        public async Task<RgbImage<byte>> GetGradientedOriginalImage()
        {
            if (_gradientedOriginalImage == null)
                await GenerateGradientedOriginalImage();

            return _gradientedOriginalImage;
        }

        private Task LoadDcmImage(string dcmPath)
        {
            return Task.Run(() =>
            {
                var dcmObj = DICOMObject.Read(dcmPath);

                var rowsTag = new Tag("0028", "0010");
                var columnsTag = new Tag("0028", "0011");
                var bitsAllocatedTag = new Tag("0028", "0100");
                
                var bitsAllocated = dcmObj.FindFirst(bitsAllocatedTag);

                var stream = dcmObj.PixelStream;

                switch ((ushort)bitsAllocated.DData)
                {
                    case 8:

                        var pixels = new byte[stream.Length];
                        stream.Read(pixels, 0, pixels.Length);

                        _originalImage = new RgbImage<byte>
                        {
                            ImmageArray = ConvertToRgb(pixels),
                            Width = (ushort)dcmObj.FindFirst(columnsTag).DData,
                            Height = (ushort)dcmObj.FindFirst(rowsTag).DData
                        };

                        break;

                    default:
                        MessageBox.Show($"Given bits allocated value ({bitsAllocated}) isn't supported!");
                        break;
                }
            });
        }
        
        private async Task DisjunctOriginalImageWithBackgroundBuffer()
        {
            await _loadImageTask;

            if (_backgroundBuffer == null
                || _backgroundBuffer.Width != _originalImage.Width
                || _backgroundBuffer.Height != _originalImage.Height)
                GenerateBackgroundBuffer(_originalImage.Width, _originalImage.Height);

            _disjunctionOfOriginalImageWithBackgroundBuffer = new RgbImage<byte>
            {
                Width = _originalImage.Width,
                Height = _originalImage.Height,
                ImmageArray = new byte[RowsCount, 3]
            };

            for (int i = 0; i < RowsCount; i++)
            {
                _disjunctionOfOriginalImageWithBackgroundBuffer.ImmageArray[i, 0] = 
                    (byte)(_originalImage.ImmageArray[i, 0] | _backgroundBuffer.ImmageArray[i, 0]);

                _disjunctionOfOriginalImageWithBackgroundBuffer.ImmageArray[i, 1] = 
                    (byte)(_originalImage.ImmageArray[i, 1] | _backgroundBuffer.ImmageArray[i, 1]);

                _disjunctionOfOriginalImageWithBackgroundBuffer.ImmageArray[i, 2] =
                            (byte)(_originalImage.ImmageArray[i, 2] | _backgroundBuffer.ImmageArray[i, 2]);
            }
                
        }

        private void GenerateBackgroundBuffer(int width, int height)
        {
            _backgroundBuffer = new RgbImage<byte>
            {
                Width = width,
                Height = height,
                ImmageArray = new byte[RowsCount, 3]
            };

            for (int i = 0; i < RowsCount; i++)
                _backgroundBuffer.ImmageArray[i, 0] = 
                    _backgroundBuffer.ImmageArray[i, 1] = 
                    _backgroundBuffer.ImmageArray[i, 2] = height / 2 * width > i ? byte.MinValue : byte.MaxValue;
        }
        
        private async Task GenerateGradientedOriginalImage()
        {
            await _loadImageTask;

            _gradientedOriginalImage = new RgbImage<byte>()
            {
                Width = _originalImage.Width,
                Height = _originalImage.Height,
                ImmageArray = new byte[RowsCount, 3]
            };

            for (int i = 0; i < RowsCount; i++)
            {
                _gradientedOriginalImage.ImmageArray[i, 0] =
                    _gradientedOriginalImage.ImmageArray[i, 1] =
                        0;

                _gradientedOriginalImage.ImmageArray[i, 2] =
                    GetColorValueFromGradientMeasure(_originalImage.ImmageArray[i, 2]);
            }
        }

        private static byte GetColorValueFromGradientMeasure(byte b)
        {
            return (byte)(byte.MaxValue / 2 > b ? b * 2 : byte.MaxValue - b*2 + byte.MaxValue);
        }

        private byte[,] ConvertToRgb(byte[] image)
        {
            var rgbImage = new byte[image.Length, 3];

            for (int i = 0; i < image.Length; i++)
                rgbImage[i, 0] = rgbImage[i, 1] = rgbImage[i, 2] = image[i];

            return rgbImage;
        }
    }
}