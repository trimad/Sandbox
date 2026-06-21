using System;
using System.Security.Cryptography;
using System.Threading;

namespace Sandbox.Fractals
{
    public class BuddhabrotCoverage : Fractal
    {
        readonly int cutoff, bailout;
        readonly int coveragePercent;
        int coverageTarget;
        int coveredPixels;

        public BuddhabrotCoverage(int _width, int _height, int _cutoff, int _bailout, int _coveragePercent)
        {
            this.name = "Buddhabrot-Coverage";
            this.width = _width;
            this.height = _height;
            this.cutoff = _cutoff;
            this.bailout = _bailout;
            this.coveragePercent = Math.Max(1, Math.Min(100, _coveragePercent));
        }

        public override Fractal Render()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            highestActual = 0;
            coveredPixels = 0;
            coverageTarget = (int)Math.Max(1, Math.Ceiling((double)width * height * coveragePercent / 100.0));

            Thread[] pool = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new Thread(Plot);
            }

            foreach (Thread thread in pool)
            {
                thread.Start();
            }

            foreach (Thread thread in pool)
            {
                thread.Join();
            }

            return this;
        }

        private void Plot()
        {
            byte[] randomBytes = new byte[8];
            while (coveredPixels < coverageTarget)
            {
                double r = NextUnit(randomBytes) * (domain[width - 1][0][0] - domain[0][0][0]) + domain[0][0][0];
                double i = NextUnit(randomBytes) * (domain[0][height - 1][1] - domain[0][0][1]) + domain[0][0][1];

                if (Iterate(r, i, false))
                {
                    Iterate(r, i, true);
                }
            }
        }

        private static double NextUnit(byte[] buffer)
        {
            RandomNumberGenerator.Fill(buffer);
            ulong value = BitConverter.ToUInt64(buffer, 0) >> 11;
            return value * (1.0 / (1UL << 53));
        }

        private bool Iterate(double r, double i, bool drawIt)
        {
            Complex c = new Complex(r, i);
            Complex z = new Complex(0, 0);
            double iterations = 0;

            Complex last; // for shading
            double totalDistance = 0;
            do
            {
                last = z;
                z.Square();
                z.AddRotated(c);
                totalDistance += z.Distance(last);

                if (drawIt && iterations >= cutoff)
                {
                    double Ax = domain[0][0][0];
                    double Bx = domain[width - 1][0][0];
                    double Dx = width;
                    int rx = (int)((z.i - Ax) / (Bx - Ax) * Dx);

                    double Ay = domain[0][0][1];
                    double By = domain[0][height - 1][1];
                    double Dy = height;
                    int iy = (int)((z.r - Ay) / (By - Ay) * Dy);

                    if (rx >= 0 && iy >= 0 && iy < height && rx < width)
                    {
                        int index = rx + iy * width;
                        int newExposure = Interlocked.Increment(ref exposure[index]);
                        if (newExposure == 1)
                        {
                            Interlocked.Increment(ref coveredPixels);
                        }

                        distance[index] = Math.Log(totalDistance);
                        UpdateHighest(newExposure);
                    }
                }

                if (z.Magnitude() > 2.0)
                {
                    return true;
                }
            }
            while (iterations++ < bailout);

            return false;
        }

        private void UpdateHighest(int value)
        {
            int current = highestActual;
            while (value > current)
            {
                int prior = Interlocked.CompareExchange(ref highestActual, value, current);
                if (prior == current)
                {
                    return;
                }

                current = prior;
            }
        }
    }
}
