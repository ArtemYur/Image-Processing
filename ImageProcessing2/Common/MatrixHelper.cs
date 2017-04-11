using System;
using OpenTK;

namespace ImageProcessing2.Common
{
    public class MatrixHelper
    {
        public static double[] GetShearMatrix(double angle, double xRef)
        {
            var tan = Math.Tan(MathHelper.DegreesToRadians(angle));

            return new double[]
            {
                1, tan, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, -tan*xRef, 0, 1
            };
        }

        public static double[] GetInverseShearMatrix(double angle, double xRef)
        {
            return GetShearMatrix(-angle, xRef);
        }
        
        public static double[] GetRotationMatrix(double angle, double xPoint, double yPoint)
        {
            double cos = Math.Cos(MathHelper.DegreesToRadians(angle)),
                sin = Math.Sin(MathHelper.DegreesToRadians(angle));

            return new double[]
            {
                cos, sin, 0, 0,
                -sin, cos, 0, 0,
                0, 0, 1, 0,
                -xPoint * cos + yPoint * sin + xPoint, -xPoint * sin - yPoint * cos + yPoint, 0, 1
            };
        }

        public static double[] GetInverseRotationMatrix(double angle, double xPoint, double yPoint)
        {
            return GetRotationMatrix(-angle, xPoint, yPoint);
        }

        public static double[] MatrixMultiplication(double[] m1, double[] m2)
        {
            var res = new double[16];

            for (int i = 0; i < 16; i++)
                for (int k = 0; k < 4; k++)
                    res[i] += m1[(i / 4) * 4 + k] * m2[k * 4 + (i % 4)];

            return res;
        }
    }
}