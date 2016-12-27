namespace Image_Processing.Lab_1
{
    using System;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System.Drawing;

    static class Worker
    {
        private static Scene _workerScene = new Scene();

        public static void InputPints()
        {
            if (_workerScene != null)
            {
                string userInput;
                Console.WriteLine();

                _workerScene.Point0 = new Point();
                _workerScene.Point1 = new Point();

                Console.WriteLine("Input new x0:");
                userInput = Console.ReadLine();
                _workerScene.Point0.X = Convert.ToInt32(userInput);

                Console.WriteLine("Input new y0:");
                userInput = Console.ReadLine();
                _workerScene.Point0.Y = Convert.ToInt32(userInput);

                Console.WriteLine("Input new x1:");
                userInput = Console.ReadLine();
                _workerScene.Point1.X = Convert.ToInt32(userInput);

                Console.WriteLine("Input new y1:");
                userInput = Console.ReadLine();
                _workerScene.Point1.Y = Convert.ToInt32(userInput);
            }
        }

        public static void Execute()
        {
            using (_workerScene = new Scene())
            {
#if (flag)
                InputPints();
#else
                _workerScene.Point0 = new Point();
                _workerScene.Point1 = new Point();

                _workerScene.CartesianPoint0 = new Point(1, 1);
                _workerScene.CartesianPoint1 = new Point(64, 20);

                _workerScene.CylindricalPoint0 = new CylindricalPoint(1, 1);
                _workerScene.CylindricalPoint1 = new CylindricalPoint(90, 85);
#endif

                _workerScene.Run(30.0);
            }
        }
    }

    class CylindricalPoint
    {
        public CylindricalPoint(double r, double phi)
        {
            R = r;
            Phi = phi;
        }

        public double R { get; set; }
        public double Phi { get; set; }
    }

    static class MathHelper
    {
        public static Point ConvertToCartesian(this CylindricalPoint cylindricalPoiint)
        {
            var cartesianPoint = new Point();

            cartesianPoint.X = (int)(cylindricalPoiint.R * Math.Cos(cylindricalPoiint.Phi.ConvertToRadians()));
            cartesianPoint.Y = (int)(cylindricalPoiint.R * Math.Sin(cylindricalPoiint.Phi.ConvertToRadians()));

            return cartesianPoint;
        }

        public static double ConvertToRadians(this double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
    
    class Scene : GameWindow
    {
        public Point Point0;
        public Point Point1;
        
        public CylindricalPoint CylindricalPoint0;
        public CylindricalPoint CylindricalPoint1;

        public Point CartesianPoint0;
        public Point CartesianPoint1;

        public static int Length = 512;

        public static int SceneWidth = Length, SceneHeight = Length;
        public static int SceneSize { get { return SceneWidth * SceneHeight; } }

        /*
         * GameWindow
         *  GraphicsContext
         *      IPlatformFactory(WinFactory).CreateGLContext()
         *          WinGLContext
         *              NativeWindow
         *                  IPlatformFactory(WinFactory).CreateNativeWindow()
         *                      WinGLNative.CreateWindowEx()
         *                          call extern "user32.dll"
         */

        public Scene() : base(SceneWidth, SceneHeight)
        {
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // describes a transformation that produces a parallel projection
            GL.Ortho(-Length / 2, Length / 2, -Length / 2, Length / 2, -1.0, 1.0);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            InitStateFromCylindric();
            DrawLine();
            SwapBuffers();
            Keyboard.KeyDown += Keyboard_KeyDown;
        }
        
        private void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            // when space is clicked change state to use cartesian points
            if(Keyboard[OpenTK.Input.Key.Space])
            {
                Point0 = CartesianPoint0;
                Point1 = CartesianPoint1;
                DrawLine();
            }
        }

        private void DrawLine()
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.Begin(BeginMode.Points);
            Bresenham8Line();
            GL.End();

            
        }

        protected void InitStateFromCylindric()
        {
            Point0 = CylindricalPoint0.ConvertToCartesian();
            Point1 = CylindricalPoint1.ConvertToCartesian();
        }
        
        public void Bresenham8Line()
        {
            int x0 = Point0.X, y0 = Point0.Y, x1 = Point1.X, y1 = Point1.Y;

            int dx = x1 - x0;
            int dy = y1 - y0;

            int d = 2 * dy - dx;

            int x = x0; int y = y0;

            if (dy > dx)
            {
                for (int i = 0; i < Length / 2; i++)
                {
                    if (d > 0)
                    {
                        x = x + 1;
                        d = d + 2 * (dx - dy);
                    }
                    else
                    {
                        d = d + 2 * dx;
                    }
                    GL.Vertex2(x, y + i);  
                }
            }
            else
            {
                for (int i = 0; i < Length / 2; i++)
                {
                    if (d > 0)
                    {
                        y = y + 1;
                        d = d + 2 * (dy - dx);
                    }
                    else
                    {
                        d = d + 2 * dy;
                    }
                    GL.Vertex2(x + i, y); 
                }
            }
        }
    }
}
