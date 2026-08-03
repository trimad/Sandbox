using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class NonicMultibrot : Fractal
    {
        private readonly int bailout;

        public NonicMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "NonicMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Nonic Multibrot / degree-9 Mandelbrot-family set:
            // z_{n+1} = z_n^9 + c, z_0 = 0.
            // Degree d Multibrots have (d - 1)-fold rotational symmetry in the
            // parameter plane; the nonic case has eight compact arms and thin tendrils.
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

                        // Compute z^9 by repeated complex multiplication. This keeps
                        // the renderer easy to audit while avoiding constructor-time
                        // allocations or per-pixel domain remapping inside the loop.
                        double powerR = zr;
                        double powerI = zi;
                        for (int power = 2; power <= 9; power++)
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
