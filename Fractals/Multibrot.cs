using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class Multibrot : Fractal
    {
        private int power;

        public Multibrot(int _width, int _height, int _highestExposureTarget, int _power)
        {
            this.name = "Multibrot";
            this.width = _width;
            this.height = _height;
            this.highestExposureTarget = _highestExposureTarget;
            this.power = _power;
        }

        public override Fractal Render()
        {
            // Multibrot: z_{n+1} = z_n^power + c
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
                        z = z.Power(power);
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
