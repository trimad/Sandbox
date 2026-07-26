using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class CubicTricorn : Fractal
    {
        private readonly int bailout;

        public CubicTricorn(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "CubicTricorn";
            this.width = _width;
            this.height = _height;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Cubic Tricorn / degree-3 multicorn:
            // z_{n+1} = conjugate(z_n)^3 + c, z_0 = 0.
            // For odd powers the conjugation changes the handedness of the
            // cubic Multibrot lobes and produces the multicorn's anti-holomorphic
            // parameter-plane structure.
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

                        // conjugate first: (zx - i zy)^3
                        double conjugateImaginary = -zy;
                        double squareReal = zx * zx - conjugateImaginary * conjugateImaginary;
                        double squareImaginary = 2.0 * zx * conjugateImaginary;
                        double cubicReal = squareReal * zx - squareImaginary * conjugateImaginary;
                        double cubicImaginary = squareReal * conjugateImaginary + squareImaginary * zx;

                        zx = cubicReal + cReal;
                        zy = cubicImaginary + cImaginary;

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
