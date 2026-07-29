using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class SexticMultibrot : Fractal
    {
        private readonly int bailout;

        public SexticMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "SexticMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Sextic Multibrot / degree-6 Mandelbrot-family set:
            // z_{n+1} = z_n^6 + c, z_0 = 0.
            // The even power creates fivefold rotational symmetry in the parameter
            // plane, with compressed central lobes and very thin exterior tendrils.
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

                        // (zr + i zi)^6 = (zr^6 - 15 zr^4 zi^2 + 15 zr^2 zi^4 - zi^6)
                        //              + i(6 zr^5 zi - 20 zr^3 zi^3 + 6 zr zi^5)
                        double nextR = zr6 - 15.0 * zr4 * zi2 + 15.0 * zr2 * zi4 - zi6 + cr;
                        double nextI = 6.0 * zr5 * zi - 20.0 * zr3 * zi3 + 6.0 * zr * zi5 + ci;

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
