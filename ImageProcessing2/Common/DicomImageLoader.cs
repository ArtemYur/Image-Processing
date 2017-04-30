using System.Threading.Tasks;
using System.Windows.Forms;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;

namespace ImageProcessing2.Common
{
    public class DicomImageLoader
    {
        private readonly string _imagePath;

        public DicomImageLoader(string imagePath = null)
        {
            _imagePath = imagePath ?? $"{Application.StartupPath}\\DICOM_Image.dcm";
        }

        public Task<Image> LoadDcmImage()
        {
            return Task.Run(() =>
            {
                var dcmObj = DICOMObject.Read(_imagePath);

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

                        return new Image
                        {
                            ImageArray = pixels,
                            Width = (ushort)dcmObj.FindFirst(columnsTag).DData,
                            Height = (ushort)dcmObj.FindFirst(rowsTag).DData
                        };

                    default:
                        MessageBox.Show($"Given bits allocated value ({bitsAllocated}) isn't supported!");
                        break;
                }

                return null;
            });
        }
    }
}
