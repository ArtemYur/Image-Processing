using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace RGR_1s_5c
{
    public static class GlControlHandler
    {
        private static OpenTK.GLControl _glControl;
        private static int _texId;
        private static byte[] _image;
        private static int _width;
        private static int _height;
        private static bool _textureRendered;

        public static OpenTK.GLControl GlControl { set
            {
                _width = value.Width;
                _height = value.Height;
                _glControl = value;
            }
        }

        internal static void UpsertImageDetails(byte[] image, int width, int height)
        {
            _image = image;
            _width = width;
            _height = height;
            _textureRendered = false;
        }

        internal static void GlControlPaint(object sender, PaintEventArgs e)
        {
            if(_image != null)
            {
                // describes a transformation that produces a parallel projection
                //GL.Ortho(-_width / 2, _width / 2, -_height / 2, _height / 2, -1.0, 1.0);
                //SetOffset();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                
                if (!_textureRendered)
                {   
                    DrawImage();
                }                

                OnRenderFrame();
                _glControl.SwapBuffers();
            }
        }

        internal static void GlControlLoad(object sender, EventArgs e)
        {
            // describes a transformation that produces a parallel projection
            //SetOffset();
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            //MainForm.LoadDcmImage("");

            _glControl.SwapBuffers();
        }

        private static void SetOffset()
        {
            //GL.Ortho(-_width / 2, _width / 2, -_height / 2, _height / 2, -1.0, 1.0);
            GL.Ortho(-_glControl.Width / 2, _glControl.Width / 2, - _glControl.Height / 2, _glControl.Height / 2, -1.0, 1.0);
        }

        private static void DrawImage()
        {
            SetOffset();

            GL.GenTextures(1, out _texId);
            GL.BindTexture(TextureTarget.Texture2D, _texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, _width, _height, 0,
                PixelFormat.Luminance, PixelType.UnsignedByte, _image);

            _textureRendered = true;
        }

        private static void OnRenderFrame()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, _texId);

            GL.Begin(BeginMode.Quads);

            //GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, -1f);
            //GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, 1f);
            //GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, 1f);
            //GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, -1f);
            // System.Windows.Forms.Control.MousePosition

            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(0f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, 0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, -1f);

            //GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(0f, 0f);
            //GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(-2f, 0f);
            //GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-2f, -2f);
            //GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0f, -2f);

            GL.End();
        }

        internal static void GlControlMouseHover(object sender, EventArgs e)
        {
            if(_textureRendered)
            {

            }
        }
    }
}
