using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Drawing;
using EvilDICOM.Core.Interfaces;

namespace RGR_1s_5c
{
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
            //var cursorPosition = glControl.PointToClient(Cursor.Position);
            //if (_textureRendered && cursorPosition.X < glControl.Width / 2 && cursorPosition.Y > glControl.Height / 2)
            if (_textureRendered && e.X < glControl.Width / 2 && e.Y > glControl.Height / 2)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                var data = ((IEnumerable<double>)_imagePosition.DData_).ToList();

                var pixelSpacing = (double)_pixelSpacing.DData;

                textOutput.Text = $"{(float)(data[0] + pixelSpacing*(glControl.Width / 2 - e.X))}; {(float)(data[1] + pixelSpacing * (glControl.Height - e.Y))}; {data[2]}";
                
                //textOutput.Text = $"{data[0] - glControl.Width / 2 + e.X}; {data[1] - glControl.Height + e.Y}; {data[2]}";

                //var @var = e.X + ((glControl.Height - e.Y - 1) * glControl.Width);
            }
            else
            {
                textOutput.Text = "";
            }

            textBoxW.Text = $"{e.X - glControl.Width / 2}; {glControl.Height / 2 - e.Y}";
        }
        
        //private void glControl_MouseHover(object sender, EventArgs e)
        //{
        //    var cursorPosition = glControl.PointToClient(Cursor.Position);
        //    if (_textureRendered && cursorPosition.X < glControl.Width / 2 && cursorPosition.Y > glControl.Height / 2)
        //    {
        //        if (Debugger.IsAttached)
        //        {
        //            Debugger.Break();
        //        }
        //    }
        //}

        internal static void UpsertImageDetails(byte[] image, int width, int height)
        {
            _image = image;
            _width = width;
            _height = height;
            _textureRendered = false;
        }
        
        private void DrawImage()
        {
            try
            {
                SetOffset();
                
                Bitmap textBmp = _image.ToList().ToBitmap(glControl.Width / 2, glControl.Height / 2);
                                
                GL.GenTextures(1, out _texId);
                GL.BindTexture(TextureTarget.Texture2D, _texId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

                Graphics gfx = Graphics.FromImage(textBmp);
                gfx.DrawString("L", (Font)Font.Clone(), Brushes.White, 10, 10);

                System.Drawing.Imaging.BitmapData data = textBmp.LockBits(new Rectangle(0, 0, textBmp.Width, textBmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, _width, _height, 0,
                    PixelFormat.Luminance, PixelType.UnsignedByte, _image);

                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, glControl.Width / 2, glControl.Height / 2, 0,
                //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                textBmp.UnlockBits(data);

                _textureRendered = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        private void SetOffset()
        {
            GL.Ortho(-glControl.Width / 2, glControl.Width / 2, -glControl.Height / 2, glControl.Height / 2, -1.0, 1.0);
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

                    //GL.TexCoord2(glControl.Width / 2, 0);
                    //GL.Vertex2(0, 0);

                    //GL.TexCoord2(glControl.Width / 2, glControl.Height / 2);
                    //GL.Vertex2(-glControl.Width / 2, 0);

                    //GL.TexCoord2(0, glControl.Height / 2);
                    //GL.Vertex2(-glControl.Width / 2, -glControl.Height / 2);

                    //GL.TexCoord2(0, 0);
                    //GL.Vertex2(0, -glControl.Height / 2);

                    GL.TexCoord2(glControl.Width / 2, 0);
                    GL.Vertex2(-glControl.Width / 2, -glControl.Height / 2);

                    GL.TexCoord2(glControl.Width / 2, glControl.Height / 2);
                    GL.Vertex2(-glControl.Width / 2, 0);

                    GL.TexCoord2(0, glControl.Height / 2);
                    GL.Vertex2(0, 0);

                    GL.TexCoord2(0, 0);
                    GL.Vertex2(0, -glControl.Height / 2);

                    //GL.TexCoord2(0, glControl.Height / 2);
                    //GL.Vertex2(-glControl.Width / 2, 0);

                    //GL.TexCoord2(glControl.Width / 2, glControl.Height / 2);
                    //GL.Vertex2(0, 0);

                    //GL.TexCoord2(glControl.Width / 2, 0);
                    //GL.Vertex2(0, -glControl.Height / 2);

                    //GL.TexCoord2(0, 0);
                    //GL.Vertex2(-glControl.Width / 2, -glControl.Height / 2);

                    GL.End();
                    break;
                default:
                    break;
            }         
        }


    }

    static class Converter
    {
        public static Bitmap ToBitmap(this List<byte> pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height); // экземпляр точечного рисунка
            int index = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(pixels[index], pixels[index], pixels[index]));
                    index++;
                }
            }
            return bitmap;
        }
    }
}
