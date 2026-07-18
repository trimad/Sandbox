using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class MagnetType2 : Fractal
    {
        private readonly int bailout;
        private const double ConvergenceEpsilonSquared = 1.0e-12;
        private const double DenominatorEpsilonSquared = 1.0e-24;

        public MagnetType2(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "MagnetType2";
            this.width = _width;
            this.height = _height;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Magnet Type II rational map:
            // z_{n+1} = ((z^3 + 3(c - 1)z + (c - 1)(c - 2)) /
            //            (3z^2 + 3(c - 2)z + (c - 1)(c - 2) + 1))^2, z_0 = 0.
            // The parameter c is colored by convergence toward the attracting fixed point z = 1.
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
                        double previousR = zr;
                        double previousI = zi;

                        // z^2 and z^3
                        double z2r = zr * zr - zi * zi;
                        double z2i = 2.0 * zr * zi;
                        double z3r = z2r * zr - z2i * zi;
                        double z3i = z2r * zi + z2i * zr;

                        // c - 1 and c - 2
                        double c1r = cr - 1.0;
                        double c1i = ci;
                        double c2r = cr - 2.0;
                        double c2i = ci;

                        // (c - 1)(c - 2)
                        double c12r = c1r * c2r - c1i * c2i;
                        double c12i = c1r * c2i + c1i * c2r;

                        // numerator = z^3 + 3(c - 1)z + (c - 1)(c - 2)
                        double c1zr = c1r * zr - c1i * zi;
                        double c1zi = c1r * zi + c1i * zr;
                        double nr = z3r + 3.0 * c1zr + c12r;
                        double ni = z3i + 3.0 * c1zi + c12i;

                        // denominator = 3z^2 + 3(c - 2)z + (c - 1)(c - 2) + 1
                        double c2zr = c2r * zr - c2i * zi;
                        double c2zi = c2r * zi + c2i * zr;
                        double dr = 3.0 * z2r + 3.0 * c2zr + c12r + 1.0;
                        double di = 3.0 * z2i + 3.0 * c2zi + c12i;
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
