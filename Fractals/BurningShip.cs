using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class BurningShip : Fractal
    {
        public BurningShip(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            this.name = "BurningShip";
            this.width = _width;
            this.height = _height;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            // Burning Ship: z_{n+1} = (|Re(z_n)| + i|Im(z_n)|)^2 + c
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double r = domain[x][y][0];
                    double i = domain[x][y][1];
                    Complex c = new Complex(r, i);
                    double zx = 0.0;
                    double zy = 0.0;
                    int iterations = 0;
                    double totalDistance = 0.0;
                    double lastX = 0.0, lastY = 0.0;
                    do
                    {
                        lastX = zx;
                        lastY = zy;
                        double absZx = Math.Abs(zx);
                        double absZy = Math.Abs(zy);
                        double newZx = absZx * absZx - absZy * absZy + c.r;
                        double newZy = 2.0 * absZx * absZy + c.i;
                        zx = newZx;
                        zy = newZy;
                        totalDistance += Math.Sqrt((zx - lastX) * (zx - lastX) + (zy - lastY) * (zy - lastY));
                    } while (zx * zx + zy * zy <= 4.0 && iterations++ < highestExposureTarget);
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
