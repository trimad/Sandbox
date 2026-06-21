using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class NewtonFractal : Fractal
    {
        private int degree;

        public NewtonFractal(int _width, int _height, int _highestExposureTarget, int _degree)
        {
            this.name = "NewtonFractal";
            this.width = _width;
            this.height = _height;
            this.highestExposureTarget = _highestExposureTarget;
            this.degree = _degree;
        }

        // Newton iteration: z -> z - (z^d - 1) / (d * z^(d-1))
        private Complex NewtonStep(Complex z, int d)
        {
            Complex zn = z.Power(d);
            Complex zn1 = z.Power(d - 1);
            double nr = zn.r - 1.0;
            double ni = zn.i;
            // denominator = d * z^(d-1)
            double dr = d * zn1.r;
            double di = d * zn1.i;
            // quotient = numerator / denominator
            double denom = dr * dr + di * di;
            double qr = (nr * dr + ni * di) / denom;
            double qi = (ni * dr - nr * di) / denom;
            return new Complex(z.r - qr, z.i - qi);
        }

        public override Fractal Render()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double r = domain[x][y][0];
                    double i = domain[x][y][1];
                    Complex z = new Complex(r, i);
                    Complex last = new Complex(0, 0);
                    int iterations = 0;
                    double totalDistance = 0.0;
                    do
                    {
                        last = z;
                        z = NewtonStep(z, degree);
                        totalDistance += z.Distance(last);
                    } while (z.Distance(last) > 1e-8 && iterations++ < highestExposureTarget);
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
