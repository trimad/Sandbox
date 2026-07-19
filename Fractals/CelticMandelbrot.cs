using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    class CelticMandelbrot : Fractal
    {
        private readonly int bailout;

        public CelticMandelbrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "Celtic Mandelbrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
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

                    while (iterations < bailout)
                    {
                        double zr2 = zr * zr;
                        double zi2 = zi * zi;
                        double nextR = Math.Abs(zr2 - zi2) + cr;
                        double nextI = 2.0 * zr * zi + ci;

                        double stepR = nextR - zr;
                        double stepI = nextI - zi;
                        totalDistance += Math.Sqrt(stepR * stepR + stepI * stepI);

                        zr = nextR;
                        zi = nextI;
                        iterations++;

                        if (zr * zr + zi * zi >= 4.0)
                        {
                            break;
                        }
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
