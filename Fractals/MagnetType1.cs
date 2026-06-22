using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class MagnetType1 : Fractal
    {
        private readonly int bailout;
        private const double ConvergenceEpsilonSquared = 1.0e-12;
        private const double DenominatorEpsilonSquared = 1.0e-24;

        public MagnetType1(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "MagnetType1";
            this.width = _width;
            this.height = _height;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Magnet Type I rational map:
            // z_{n+1} = ((z_n^2 + c - 1) / (2 z_n + c - 2))^2, z_0 = 0.
            // The magnet set is the parameter plane of points c whose orbit converges to 1.
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

                    while (iterations < bailout)
                    {
                        previousR = zr;
                        previousI = zi;

                        // numerator = z^2 + c - 1
                        double z2r = zr * zr - zi * zi;
                        double z2i = 2.0 * zr * zi;
                        double nr = z2r + cr - 1.0;
                        double ni = z2i + ci;

                        // denominator = 2z + c - 2
                        double dr = 2.0 * zr + cr - 2.0;
                        double di = 2.0 * zi + ci;
                        double denom = dr * dr + di * di;

                        if (denom < DenominatorEpsilonSquared)
                        {
                            break;
                        }

                        // ratio = numerator / denominator
                        double rr = (nr * dr + ni * di) / denom;
                        double ri = (ni * dr - nr * di) / denom;

                        // z_{n+1} = ratio^2
                        zr = rr * rr - ri * ri;
                        zi = 2.0 * rr * ri;

                        double stepR = zr - previousR;
                        double stepI = zi - previousI;
                        totalDistance += Math.Sqrt(stepR * stepR + stepI * stepI);
                        iterations++;

                        double toOneR = zr - 1.0;
                        if (toOneR * toOneR + zi * zi < ConvergenceEpsilonSquared)
                        {
                            break;
                        }

                        if (zr * zr + zi * zi > 1.0e12 || double.IsNaN(zr) || double.IsNaN(zi) || double.IsInfinity(zr) || double.IsInfinity(zi))
                        {
                            break;
                        }
                    }

                    int index = x + y * width;
                    exposure[index] = iterations;
                    // Blend orbit length with final distance to the attracting fixed point at 1.
                    double finalToOne = Math.Sqrt((zr - 1.0) * (zr - 1.0) + zi * zi);
                    distance[index] = Math.Log(totalDistance + finalToOne + 1.0);
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
