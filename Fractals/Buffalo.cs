using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class Buffalo : Fractal
    {
        private readonly int bailout;

        public Buffalo(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "Buffalo";
            this.width = _width;
            this.height = _height;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Buffalo Mandelbrot variant:
            // z_{n+1} = |Re(z_n^2)| + i|Im(z_n^2)| + c, z_0 = 0.
            // Taking absolute values after squaring creates horn-like folds while
            // preserving the familiar escape-time parameter-plane workflow.
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double cReal = domain[x][y][0];
                    double cImaginary = domain[x][y][1];
                    double zx = 0.0;
                    double zy = 0.0;
                    int iterations = 0;
                    double totalDistance = 0.0;

                    do
                    {
                        double lastX = zx;
                        double lastY = zy;
                        double squaredReal = zx * zx - zy * zy;
                        double squaredImaginary = 2.0 * zx * zy;

                        zx = Math.Abs(squaredReal) + cReal;
                        zy = Math.Abs(squaredImaginary) + cImaginary;

                        double dx = zx - lastX;
                        double dy = zy - lastY;
                        totalDistance += Math.Sqrt(dx * dx + dy * dy);
                    } while (zx * zx + zy * zy <= 4.0 && iterations++ < bailout);

                    int index = x + y * width;
                    exposure[index] = iterations;
                    distance[index] = Math.Log(totalDistance + 1.0);
                    if (highestActual < exposure[index])
                    {
                        highestActual = exposure[index];
                    }
                }
            });

            return this;
        }
    }
}
