using EvilDICOM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGR_1s_5c
{
    static class DcmHandler
    {


        public static Task LoadDcmImage(string dcmPath)
        {
            return Task.Run(() =>
            {
                var dcmObj = DICOMObject.Read(dcmPath);

                var rowsTag = new Tag("0028", "0010");
                var columnsTag = new Tag("0028", "0011");
                var bitsAllocatedTag = new Tag("0028", "0100");
                var imageOrientationTag = new Tag("0020", "0037");

                var width = dcmObj.FindFirst(columnsTag);
                var height = dcmObj.FindFirst(rowsTag);
                var bitsAllocated = dcmObj.FindFirst(bitsAllocatedTag);
                var imageOrientation = dcmObj.FindFirst(imageOrientationTag);
                var stream = dcmObj.PixelStream;

                switch ((ushort)bitsAllocated.DData)
                {
                    case 8:
                        var pixels = new byte[stream.Length];
                        stream.Read(pixels, 0, pixels.Length);

                        GlControlHandler.UpsertImageDetails(pixels, (ushort)width.DData, (ushort)height.DData);
                        ResizeWindow((ushort)width.DData, (ushort)height.DData);
                        glControl.Refresh();

                        break;

                    default:
                        MessageBox.Show($"Given bits allocated value ({bitsAllocated}) isn't supported!");
                        break;
                }
            });
        }

        private void ResizeWindow(int width, int height)
        {
            var windowWidth = (int)Math.Ceiling((width * 2) / 0.6);
            var windowHeight = (int)Math.Ceiling((double)(height * 2));

            if (windowWidth < _maxWidth && windowHeight < _maxHeight)
            {
                this.ClientSize = new Size(windowWidth, windowHeight);
            }
            else
            {
                MessageBox.Show("Size of the given image is unsupported!");
            }
        }
    }
}
