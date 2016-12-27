using EvilDICOM.Core;
using EvilDICOM.Core.Element;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGR_1s_5c
{
    partial class MainForm
    {
        private int _maxWidth;
        private int _maxHeight;

        public MainForm()
        {
            InitializeComponent();

            _maxWidth = 1300;
            _maxHeight = 800;
        }

        private async void openDcmButton_Click(object sender, EventArgs e)
        {
            openDcmFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (openDcmFileDialog.ShowDialog() == DialogResult.OK)
            {
                var dcmPath = openDcmFileDialog.FileName;

                await LoadDcmImage(dcmPath);
            }
        }

        public Task LoadDcmImage(string dcmPath)
        {
            return Task.Run(() =>
            {
                var dcmObj = DICOMObject.Read(dcmPath);

                var rowsTag = new Tag("0028", "0010");
                var columnsTag = new Tag("0028", "0011");
                var bitsAllocatedTag = new Tag("0028", "0100");
                var imageOrientationTag = new Tag("0020", "0037");
                var imagePositionTag = new Tag("0020", "0032");
                var patientPositionTag = new Tag("0018", "5100");
                var patientOrientationTag = new Tag("0020", "0020");
                var pixelSpacingTag = new Tag("0028", "0030");
                //var anatomicalOrientationTypeTag = new Tag("0010", "2210");

                var width = dcmObj.FindFirst(columnsTag);
                var height = dcmObj.FindFirst(rowsTag);
                var bitsAllocated = dcmObj.FindFirst(bitsAllocatedTag);
                
                var stream = dcmObj.PixelStream;

                _imageOrientation = dcmObj.FindFirst(imageOrientationTag);
                _imagePosition = dcmObj.FindFirst(imagePositionTag);
                _patientPosition = dcmObj.FindFirst(patientPositionTag);
                _patientOrientation = dcmObj.FindFirst(patientOrientationTag);
                _pixelSpacing = dcmObj.FindFirst(pixelSpacingTag);
                //var anatomicalOrientationType = dcmObj.FindFirst(anatomicalOrientationTypeTag);

                switch ((ushort)bitsAllocated.DData)
                {
                    case 8:
                        var pixels = new byte[stream.Length];
                        stream.Read(pixels, 0, pixels.Length);

                        UpsertImageDetails(pixels, (ushort)width.DData, (ushort)height.DData);
                        ResizeWindow((ushort)width.DData, (ushort)height.DData);

                        this.Invoke(new MethodInvoker(delegate () { glControl.Refresh(); }));

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
                this.Invoke(new MethodInvoker(delegate () 
                {
                    this.ClientSize = new Size(windowWidth, windowHeight);
                }));
            }
            else
            {
                MessageBox.Show("Size of the given image is unsupported!");
            }
        }
    }
}
