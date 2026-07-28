using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class QuinticMultibrot : Fractal
    {
        private readonly int bailout;

        public QuinticMultibrot(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "QuinticMultibrot";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Quintic Multibrot / degree-5 Mandelbrot-family set:
            // z_{n+1} = z_n^5 + c, z_0 = 0.
            // The odd power produces fivefold rotational structure with thin
            // dendritic filaments around the parameter-plane boundary.
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
                        double zr4 = zr2 * zr2;
                        double zi4 = zi2 * zi2;

                        // (zr + i zi)^5 = (zr^5 - 10 zr^3 zi^2 + 5 zr zi^4)
                        //              + i(5 zr^4 zi - 10 zr^2 zi^3 + zi^5)
                        double nextR = zr * zr4 - 10.0 * zr * zr2 * zi2 + 5.0 * zr * zi4 + cr;
                        double nextI = 5.0 * zr4 * zi - 10.0 * zr2 * zi * zi2 + zi * zi4 + ci;

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
