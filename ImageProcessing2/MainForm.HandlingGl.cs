using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProcessing2.Common;
using OpenTK.Graphics.OpenGL;

namespace ImageProcessing2
{
    public partial class MainForm
    {
        private static int _texId;
        private static bool _textureRendered;

        private static RgbImageProvider _rgbImageProvider;

        private async void glControl_Load(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            _rgbImageProvider = new RgbImageProvider();

            var image = await _rgbImageProvider.GetOriginalImage();
            
            ResizeWindow(image.Width, image.Height);

            this.Invoke(new MethodInvoker(glControl.Refresh));

            glControl.SwapBuffers();
        }

        private async void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (_rgbImageProvider != null)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (!_textureRendered)
                {
                    await GenerateAndBindTexture();
                    _textureRendered = true;
                }

                OnRenderFrame(e);
                glControl.SwapBuffers();
            }
        }

        private async void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_rgbImageProvider != null)
            {
                if (e.KeyChar == Convert.ToChar(Keys.Q))
                    BindTexture(await _rgbImageProvider.GetOriginalImage());

                if (e.KeyChar == Convert.ToChar(Keys.W))
                    BindTexture(await _rgbImageProvider.GetDisjunctionOfOriginalImageWithBackgroundBuffer());

                if (e.KeyChar == Convert.ToChar(Keys.E))
                    BindTexture(await _rgbImageProvider.GetGradientedOriginalImage());

                this.Invoke(new MethodInvoker(glControl.Refresh));
            }
        }

        private async Task GenerateAndBindTexture()
        {
            GL.GenTextures(1, out _texId);
            BindTexture(await _rgbImageProvider.GetOriginalImage());
        }
        
        private void BindTexture(RgbImage<byte> rgbImage)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, rgbImage.Width, rgbImage.Height, 0,
                PixelFormat.Rgb, PixelType.UnsignedByte, rgbImage.ImmageArray);
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
