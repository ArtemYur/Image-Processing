using System;
using System.Drawing;
using System.Windows.Forms;
using ImageProcessing2.Common;
using OpenTK.Graphics.OpenGL;
using Image = ImageProcessing2.Common.Image;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace ImageProcessing2
{
    public partial class MainForm
    {
        private Image _image;
        private static int _texId;
        private static bool _textureRendered;
        private TransformationObject _transformationObject;

        private static readonly DicomImageProvider DicomImageProvider = new DicomImageProvider();

        private async void glControl_Load(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);

            _image = await DicomImageProvider.LoadDcmImage();

            ResizeWindow(_image.Width * 2, _image.Height * 2);

            this.Invoke(new MethodInvoker(glControl.Refresh));

            glControl.SwapBuffers();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (!_textureRendered && _image != null)
            {
                GL.Ortho(-glControl.Width / 2, glControl.Width / 2, -glControl.Height / 2, glControl.Height / 2, -1.0, 1.0);
                GenerateAndBindTexture();
                _textureRendered = true; GL.LoadIdentity();
            }

            OnRenderFrame(e);
            glControl.SwapBuffers();
        }

        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Q))
            {
                var transformationObj = new TransformationObject();

                var answer = Prompt.ShowDialog("Enter shear angle", "Shear");
                transformationObj.ShearAngle = Convert.ToDouble(answer);

                answer = Prompt.ShowDialog("Enter value for x reference line", "Shear reference line");
                transformationObj.ShearXReferenceLine = Convert.ToDouble(answer);

                var shearMatrix =
                    MatrixHelper.GetShearMatrix(transformationObj.ShearAngle, transformationObj.ShearXReferenceLine);

                answer = Prompt.ShowDialog("Enter rotation angle", "Rotation");
                transformationObj.RotationAngle = Convert.ToDouble(answer);

                answer = Prompt.ShowDialog("Enter x coordinate", "Rotation");
                transformationObj.RotationXPoint = Convert.ToDouble(answer);

                answer = Prompt.ShowDialog("Enter y coordinate", "Rotation");
                transformationObj.RotationYPoint = Convert.ToDouble(answer);

                var rotationMatrix =
                    MatrixHelper.GetRotationMatrix(
                        transformationObj.RotationAngle,
                        transformationObj.RotationXPoint,
                        transformationObj.RotationYPoint);

                transformationObj.TransformationMatrix =
                    MatrixHelper.MatrixMultiplication(shearMatrix, rotationMatrix);

                _transformationObject = transformationObj;
            }

            if (e.KeyChar == Convert.ToChar(Keys.W))
            {
                if (_transformationObject != null)
                {
                    var invShearMatrix = MatrixHelper.GetInverseShearMatrix(
                                            _transformationObject.ShearAngle,
                                            _transformationObject.ShearXReferenceLine);

                    var invRotationMatrix = MatrixHelper.GetInverseRotationMatrix(
                                                _transformationObject.RotationAngle,
                                                _transformationObject.RotationXPoint,
                                                _transformationObject.RotationYPoint);

                    var invTransformationMatrix = MatrixHelper.MatrixMultiplication(invRotationMatrix, invShearMatrix);

                    _transformationObject.TransformationMatrix = invTransformationMatrix;
                }
            }

            this.Invoke(new MethodInvoker(glControl.Refresh));
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
            GL.MatrixMode(MatrixMode.Projection);

            if (_transformationObject != null)
            {
                GL.MultMatrix(_transformationObject.TransformationMatrix);
            }

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(0.5f, -0.5f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(-0.5f, -0.5f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-0.5f, 0.5f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0.5f, 0.5f);

            GL.End();
            GL.Flush();
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

        class TransformationObject
        {
            public double[] TransformationMatrix;
            public double ShearAngle;
            public double ShearXReferenceLine;
            public double RotationAngle;
            public double RotationXPoint;
            public double RotationYPoint;
        }
    }
}
