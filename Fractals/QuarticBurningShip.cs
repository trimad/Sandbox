using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class QuarticBurningShip : Fractal
    {
        private readonly int bailout;

        public QuarticBurningShip(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "QuarticBurningShip";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Quartic Burning Ship / degree-4 Burning Ship variant:
            // z_{n+1} = (|Re(z_n)| + i|Im(z_n)|)^4 + c, z_0 = 0.
            // Folding both axes before a quartic Multibrot step keeps the
            // Burning Ship cusp behavior while adding higher-order symmetry.
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double cr = domain[x][y][0];
                    double ci = domain[x][y][1];
                    double zr = 0.0;
                    double zi = 0.0;
                    double previousR = zr;
                    double previousI = zi;
                    double totalDistance = 0.0;
                    int iterations = 0;

                    while (iterations < bailout && zr * zr + zi * zi <= 4.0)
                    {
                        previousR = zr;
                        previousI = zi;

                        double ar = Math.Abs(zr);
                        double ai = Math.Abs(zi);
                        double ar2 = ar * ar;
                        double ai2 = ai * ai;

                        // (ar + i ai)^4 = (ar^4 - 6 ar^2 ai^2 + ai^4)
                        //              + i(4 ar^3 ai - 4 ar ai^3)
                        zr = ar2 * ar2 - 6.0 * ar2 * ai2 + ai2 * ai2 + cr;
                        zi = 4.0 * ar * ai * (ar2 - ai2) + ci;

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
