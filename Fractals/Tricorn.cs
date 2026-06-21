using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class Tricorn : Fractal
    {
        public Tricorn(int _width, int _height, int _highestExposureTarget)
        {
            this.name = "Tricorn";
            this.width = _width;
            this.height = _height;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Tricorn (Mandelbar): z_{n+1} = conj(z_n)^2 + c
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double r = domain[x][y][0];
                    double i = domain[x][y][1];
                    Complex c = new Complex(r, i);
                    Complex z = new Complex(0, 0);
                    int iterations = 0;
                    double totalDistance = 0.0;
                    Complex last = new Complex(0, 0);
                    do
                    {
                        last = z;
                        z.Square();
                        // Conjugate by flipping imaginary sign before adding c
                        z.i = -z.i;
                        z.Add(c);
                        totalDistance += z.Distance(last);
                    } while (z.MagnitudeOpt() <= 4.0 && iterations++ < highestExposureTarget);
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
