namespace RGR_1s_5c
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using OpenTK.Graphics.OpenGL;
    using EvilDICOM.Core.Interfaces;

    partial class MainForm
    {
        private static int _texId;
        private static byte[] _image;
        private static int _width;
        private static int _height;
        private static bool _textureRendered;

        private IDICOMElement _imageOrientation;
        private IDICOMElement _imagePosition;
        private IDICOMElement _patientPosition;
        private IDICOMElement _patientOrientation;
        private IDICOMElement _pixelSpacing;

        private async void glControl_Load(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            await LoadDcmImage(@"D:\Projects\Image Processing\RGR_1s_5c\bin\Debug\DICOM_Image_for_Lab_2_new.dcm");

            glControl.SwapBuffers();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (_image != null)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (!_textureRendered)
                {
                    DrawImage();
                }

                OnRenderFrame();
                glControl.SwapBuffers();
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_textureRendered && e.X < glControl.Width / 2 && e.Y > glControl.Height / 2)
            {
                var data = ((IEnumerable<double>)_imagePosition.DData_).ToList();

                var pixelSpacing = (double)_pixelSpacing.DData;

                textOutput.Text = $"{(float)(data[0] + pixelSpacing * (glControl.Width / 2 - e.X))}; {(float)(data[1] + pixelSpacing * (glControl.Height - e.Y))}; {data[2]}";
            }
            else
            {
                textOutput.Text = "";
            }

            textBoxW.Text = $"{e.X - glControl.Width / 2}; {glControl.Height / 2 - e.Y}";
        }

        internal static void UpsertImageDetails(byte[] image, int width, int height)
        {
            _image = image;
            _width = width;
            _height = height;
            _textureRendered = false;
        }

        private void DrawImage()
        {
            GL.Ortho(-glControl.Width / 2, glControl.Width / 2, -glControl.Height / 2, glControl.Height / 2, -1.0, 1.0);

            GL.GenTextures(1, out _texId);
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, _width, _height, 0,
                PixelFormat.Luminance, PixelType.UnsignedByte, _image);

            _textureRendered = true;

        }

        private void OnRenderFrame()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            switch (_patientOrientation.DData.ToString())
            {
                case "L\\PH":
                    GL.BindTexture(TextureTarget.Texture2D, _texId);

                    GL.Begin(BeginMode.Quads);

                    GL.TexCoord2(glControl.Width / 2, 0);
                    GL.Vertex2(-glControl.Width / 2, -glControl.Height / 2);

                    GL.TexCoord2(glControl.Width / 2, glControl.Height / 2);
                    GL.Vertex2(-glControl.Width / 2, 0);

                    GL.TexCoord2(0, glControl.Height / 2);
                    GL.Vertex2(0, 0);

                    GL.TexCoord2(0, 0);
                    GL.Vertex2(0, -glControl.Height / 2);

                    GL.End();
                    break;
                default:
                    break;
            }
        }
    }
}
