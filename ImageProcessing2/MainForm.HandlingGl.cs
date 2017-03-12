using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;
using ImageProcessing2.Common;
using OpenTK.Graphics.OpenGL;

namespace ImageProcessing2
{
    public partial class MainForm
    {
        private static int _texId;
        private static byte[] _image;
        private static int _width;
        private static int _height;
        private static bool _textureRendered;

        private async void glControl_Load(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            await LoadDcmImage($"{Application.StartupPath}\\DICOM_Image.dcm");

            glControl.SwapBuffers();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (_image != null)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (!_textureRendered)
                {
                    GenerateAndBindTexture();
                    _textureRendered = true;
                }

                OnRenderFrame(e);
                glControl.SwapBuffers();
            }
        }

        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Q))
                BindTexture(_image);

            if (e.KeyChar == Convert.ToChar(Keys.W))
                BindTexture(PixelTransformation.ApplyBufferBackground(_image, _width, _height));

            if (e.KeyChar == Convert.ToChar(Keys.E))
                BindTexture(
                    ConvertToRgbWithBlueChannel(
                        PixelTransformation.ApplyColorTransformation(_image, _width, _height)));

            this.Invoke(new MethodInvoker(glControl.Refresh));
        }

        internal static void UpsertImageDetails(byte[] image, int width, int height)
        {
            _image = image;
            _width = width;
            _height = height;
            _textureRendered = false;
        }

        private void GenerateAndBindTexture()
        {
            GL.GenTextures(1, out _texId);
            BindTexture(_image);
        }

        private void BindTexture(byte[] image, PixelFormat pixelFormat = PixelFormat.Luminance)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, _width, _height, 0,
                pixelFormat, PixelType.UnsignedByte, image);
        }

        private void BindTexture(byte[,] image)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, _width, _height, 0,
                PixelFormat.Rgb, PixelType.UnsignedByte, image);
        }

        private byte[,] ConvertToRgbWithBlueChannel(byte[] image)
        {
            var output = new byte[image.Length, 3];
            for (int i = 0; i < image.Length; i++)
            {
                output[i, 0] = output[i, 1] = 0;
                output[i, 2] = image[i];
            }
            return output;
        }

        private void OnRenderFrame(PaintEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.BindTexture(TextureTarget.Texture2D, _texId);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, 1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, -1f);

            GL.End();
        }

        public Task LoadDcmImage(string dcmPath)
        {
            return Task.Run(() =>
            {
                var dcmObj = DICOMObject.Read(dcmPath);

                var rowsTag = new Tag("0028", "0010");
                var columnsTag = new Tag("0028", "0011");
                var bitsAllocatedTag = new Tag("0028", "0100");

                var width = dcmObj.FindFirst(columnsTag);
                var height = dcmObj.FindFirst(rowsTag);
                var bitsAllocated = dcmObj.FindFirst(bitsAllocatedTag);

                var stream = dcmObj.PixelStream;

                switch ((ushort)bitsAllocated.DData)
                {
                    case 8:
                        var pixels = new byte[stream.Length];
                        stream.Read(pixels, 0, pixels.Length);

                        UpsertImageDetails(pixels, (ushort)width.DData, (ushort)height.DData);
                        ResizeWindow((ushort)width.DData, (ushort)height.DData);

                        this.Invoke(new MethodInvoker(glControl.Refresh));

                        break;

                    default:
                        MessageBox.Show($"Given bits allocated value ({bitsAllocated}) isn't supported!");
                        break;
                }
            });
        }

        private void ResizeWindow(int width, int height)
        {
            if (width >= _maxWidth || height >= _maxHeight)
            {
                MessageBox.Show("Size of the given image is unsupported!");
                return;
            }

            if (ClientSize.Width != width || ClientSize.Height != height)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    this.ClientSize = new Size(width, height);
                }));
            }
        }
    }
}
