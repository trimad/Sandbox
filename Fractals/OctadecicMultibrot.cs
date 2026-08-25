using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class OctadecicMultibrot : Fractal
    {
        private readonly int bailout;

        public OctadecicMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "OctadecicMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Octadecic Multibrot / degree-18 Mandelbrot-family set:
            // z_{n+1} = z_n^18 + c, z_0 = 0.
            // Degree d Multibrots have (d - 1)-fold rotational symmetry in the
            // parameter plane; the octadecic case has seventeen compact arms.
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

                        // Compute z^18 by repeated complex multiplication to avoid
                        // fragile expanded-polynomial coefficient mistakes.
                        double powerR = zr;
                        double powerI = zi;
                        for (int power = 2; power <= 18; power++)
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
