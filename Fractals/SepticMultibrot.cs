using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class SepticMultibrot : Fractal
    {
        private readonly int bailout;

        public SepticMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "SepticMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Septic Multibrot / degree-7 Mandelbrot-family set:
            // z_{n+1} = z_n^7 + c, z_0 = 0.
            // The high odd power produces sixfold rotational symmetry in the
            // parameter plane, with compact central lobes and very thin tendrils.
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double cr = domain[x][y][0];
                    double ci = domain[x][y][1];
                    double zr = 0.0;
                    double zi = 0.0;
                    double totalDistance = 0.0;
                    int iterations = 0;

                    while (iterations < bailout && zr * zr + zi * zi <= 4.0)
                    {
                        double previousR = zr;
                        double previousI = zi;

                        double zr2 = zr * zr;
                        double zi2 = zi * zi;
                        double zr3 = zr2 * zr;
                        double zi3 = zi2 * zi;
                        double zr4 = zr2 * zr2;
                        double zi4 = zi2 * zi2;
                        double zr5 = zr4 * zr;
                        double zi5 = zi4 * zi;
                        double zr6 = zr3 * zr3;
                        double zi6 = zi3 * zi3;
                        double zr7 = zr6 * zr;
                        double zi7 = zi6 * zi;

                        // (zr + i zi)^7 = (zr^7 - 21 zr^5 zi^2 + 35 zr^3 zi^4 - 7 zr zi^6)
                        //              + i(7 zr^6 zi - 35 zr^4 zi^3 + 21 zr^2 zi^5 - zi^7)
                        double nextR = zr7 - 21.0 * zr5 * zi2 + 35.0 * zr3 * zi4 - 7.0 * zr * zi6 + cr;
                        double nextI = 7.0 * zr6 * zi - 35.0 * zr4 * zi3 + 21.0 * zr2 * zi5 - zi7 + ci;

                        zr = nextR;
                        zi = nextI;

                        double stepR = zr - previousR;
                        double stepI = zi - previousI;
                        totalDistance += Math.Sqrt(stepR * stepR + stepI * stepI);
                        iterations++;
                    }

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
