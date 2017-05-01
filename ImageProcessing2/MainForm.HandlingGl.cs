using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImageProcessing2.Common;
using OpenTK.Graphics.OpenGL;
using Image = ImageProcessing2.Common.Image;

namespace ImageProcessing2
{
    public partial class MainForm
    {
        private static int _texId;
        private static ImageLoadingState _imageLoadingState = ImageLoadingState.NotLoaded;

        private static DicomImageLoader _dicomImageLoader;
        private static Dictionary<string, Image> _imageProvider;
        private Image _image;

        private async void glControl_Load(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            _dicomImageLoader = new DicomImageLoader();

            _imageProvider = new Dictionary<string, Image>();

            _image = await _dicomImageLoader.LoadDcmImage();

            _imageProvider.UpsertOriginalImage(_image);

            _imageLoadingState = ImageLoadingState.Loaded;

            this.Invoke(new MethodInvoker(glControl.Refresh));

            glControl.SwapBuffers();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (_dicomImageLoader != null)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (_imageLoadingState == ImageLoadingState.Loaded)
                {
                    GenerateAndBindTexture();
                    _imageLoadingState = ImageLoadingState.Rendered;
                }

                OnRenderFrame(e);
                glControl.SwapBuffers();
            }
        }

        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_imageProvider != null)
            {
                if (e.KeyChar == Convert.ToChar(Keys.Q))
                {
                    this.Text = "Original matrix";
                    _image = _imageProvider.GetOriginalImage();
                }

                if (e.KeyChar == Convert.ToChar(Keys.W))
                {
                    this.Text = "N matrix";
                    _image = _imageProvider.GetFilteredImageWithNMask();
                }

                if (e.KeyChar == Convert.ToChar(Keys.E))
                {
                    this.Text = "S matrix";
                    _image = _imageProvider.GetFilteredImageWithSMask();
                }

                if (e.KeyChar == Convert.ToChar(Keys.R))
                {
                    this.Text = "Se matrix";
                    _image = _imageProvider.GetFilteredImageWithSeMask();
                }

                BindTexture(_image);

                this.Invoke(new MethodInvoker(glControl.Refresh));
            }
        }
        
        private void GenerateAndBindTexture()
        {
            GL.GenTextures(1, out _texId);
            BindTexture(_image);
        }
        
        private void BindTexture(Image image, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Luminance,
            PixelFormat pixelFormat = PixelFormat.Luminance)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, image.Width, image.Height, 0,
                pixelFormat, PixelType.UnsignedByte, image.ImageArray);
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

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            brightnessDetail.Text = _image?.ImageArray[
                (_image.Height - e.Y - 1) * _image.Width + e.X].ToString() ?? "";
        }

        enum ImageLoadingState
        {
            NotLoaded, Loaded, Rendered
        }
    }
}
