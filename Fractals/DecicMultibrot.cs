using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class DecicMultibrot : Fractal
    {
        private readonly int bailout;

        public DecicMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "DecicMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Decic Multibrot / degree-10 Mandelbrot-family set:
            // z_{n+1} = z_n^10 + c, z_0 = 0.
            // Degree d Multibrots have (d - 1)-fold rotational symmetry in the
            // parameter plane; the decic case has nine compact arms and very fine tendrils.
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

                        // Compute z^10 by repeated complex multiplication. This keeps
                        // the renderer easy to audit and avoids fragile expanded-polynomial
                        // coefficient mistakes for a high-degree daily renderer.
                        double powerR = zr;
                        double powerI = zi;
                        for (int power = 2; power <= 10; power++)
                        {
                            double nextPowerR = powerR * zr - powerI * zi;
                            double nextPowerI = powerR * zi + powerI * zr;
                            powerR = nextPowerR;
                            powerI = nextPowerI;
                        }

                        zr = powerR + cr;
                        zi = powerI + ci;

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
