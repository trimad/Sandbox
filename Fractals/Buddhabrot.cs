using System;
using System.Security.Cryptography;
using System.Threading;

namespace Sandbox.Fractals
{
    public class Buddhabrot : Fractal
    {
        readonly int cutoff, bailout;
        //Complex zero = new Complex(0, 0);
        public Buddhabrot(int _width, int _height, int _cutoff, int _bailout, int _highestExposureTarget)
        {
            this.name = "Buddhabrot";
            this.width = _width;
            this.height = _height;
            this.cutoff = _cutoff;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
        }

        public override Fractal Render()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
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
            while (highestActual < highestExposureTarget)
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

            Complex last;//for shading
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
                    //double Cx = 0;
                    double Dx = width;
                    //int rx = (int)((z.i - Ax) / (Bx - Ax) * (Dx - Cx) + Cx);
                    int rx = (int)((z.i - Ax) / (Bx - Ax) * Dx);

                    double Ay = domain[0][0][1];
                    double By = domain[0][height - 1][1];
                    //double Cy = 0;
                    double Dy = height;

                    //int iy = (int)((z.r - Ay) / (By - Ay) * (Dy - Cy) + Cy);
                    int iy = (int)((z.r - Ay) / (By - Ay) * Dy);

                    if (rx >= 0 && iy >= 0 && iy < height && rx < width)
                    {
                        int index = rx + iy * width;
                        exposure[index]++;//1 divided by 255
                        distance[index] = Math.Log(totalDistance);

                        if (highestActual < exposure[index])
                        {
                            highestActual = exposure[index];
                            //if (Thread.CurrentThread.ManagedThreadId == 18) { Console.WriteLine(highestActual); }
                        }

                    }
                }

                if (z.Magnitude() > 2.0)
                {
                    // escapes
                    return true;
                }

            }
            while (iterations++ < bailout);
            //does not escape
            return false;
        }

    }
}