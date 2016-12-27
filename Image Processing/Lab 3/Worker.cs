namespace Image_Processing.Lab_3
{
    using OpenTK;
    using System;
    using System.Drawing;
    using OpenTK.Graphics.OpenGL;
    using System.Drawing.Imaging;
    using System.Linq;
    using EvilDICOM.Core;
    using EvilDICOM.Core.Element;
    using OpenTK.Input;
    using System.IO;

    static class Worker
    {
        public static void Execute()
        {
            var dcm = DICOMObject.Read("DICOM_Image_for_Lab_2.dcm");

            var rows = new Tag("0028", "0010");
            var columns = new Tag("0028", "0011");

            var width = dcm.FindFirst(columns);
            var height = dcm.FindFirst(rows);

            var group0028 = dcm.AllElements.Where(o => o.Tag.Group == "0028");
            var bitsAllocated = dcm.FindFirst(new Tag("0028", "0100"));

            switch ((ushort)bitsAllocated.DData)
            {
                case 8:
                    var studyDateTag = new Tag("0008", "0020");
                    var studyDate = dcm.FindFirst(studyDateTag);

                    var stream = dcm.PixelStream;

                    var pixels = new byte[stream.Length];
                    stream.Read(pixels, 0, pixels.Length);

                    var bitmap = pixels.ToList().ToBitmap((ushort)width.DData, (ushort)height.DData);

                    var scene = new Scene(pixels, (System.DateTime)studyDate.DData, (ushort)width.DData, (ushort)height.DData);
                    scene.Run(30.0);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    class Scene1
    {

    }

    class Scene : GameWindow
    {
        Bitmap ImageBitmap { get; set; }
        System.DateTime StudyDate { get; set; }
        TextRenderer Renderer { get; set; }
        Font SerifFont = new Font(FontFamily.GenericSerif, 14);
        byte[] image;

        Graphics textGraphics;

        int texId;

        public Scene(byte[] image, System.DateTime studyDate, int width, int height) : base(width, height)
        {
            this.image = image;
            //ImageBitmap = image;
            StudyDate = studyDate;

            GL.Enable(EnableCap.Texture2D);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.Texture2D);
            Keyboard.KeyDown += KeyboardKeyDown;

            DrawImage();
        }

        private void KeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.E])
            {
                DrawString();
                SwapBuffers();
            }
        }

        private void DrawImage()
        {
            //ImageBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            GL.GenTextures(1, out texId);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            //BitmapData data = ImageBitmap.LockBits(new Rectangle(0, 0, ImageBitmap.Width, ImageBitmap.Height),
            //    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Luminance, PixelType.UnsignedByte, image);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, image);//data.Scan0);

            //ImageBitmap.UnlockBits(data);
        }

        private void DrawString()
        {
            //textGraphics = Graphics.FromImage(image.ToList().ToBitmap(Width, Height));
            //textGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //textGraphics.DrawString($"Stduy date: {StudyDate.ToString("dd.MM.yyyy")}", SerifFont, Brushes.White, 0, 0);
            Console.WriteLine($"Stduy date: {StudyDate.ToString("dd.MM.yyyy")}");
            //DrawImage();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, texId);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, 1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, -1f);

            GL.End();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
    }
}


