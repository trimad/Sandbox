using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class CubicBurningShip : Fractal
    {
        private readonly int bailout;

        public CubicBurningShip(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "CubicBurningShip";
            this.width = _width;
            this.height = _height;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Cubic Burning Ship / degree-3 Burning Ship variant:
            // z_{n+1} = (|Re(z_n)| + i|Im(z_n)|)^3 + c, z_0 = 0.
            // Folding both axes before a cubic Multibrot step gives threefold
            // higher-power lobes while retaining Burning Ship-style cusps.
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

                        // (ar + i ai)^3 = (ar^3 - 3 ar ai^2) + i(3 ar^2 ai - ai^3)
                        double ar2 = ar * ar;
                        double ai2 = ai * ai;
                        zr = ar * ar2 - 3.0 * ar * ai2 + cr;
                        zi = 3.0 * ar2 * ai - ai * ai2 + ci;

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
