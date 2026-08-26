using System;
using NumericsComplex = System.Numerics.Complex;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    internal static class RareFractalMath
    {
        public static int MapToPixel(double value, double min, double max, int length)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || max <= min)
            {
                return -1;
            }

            int mapped = (int)Math.Round((value - min) / (max - min) * (length - 1));
            if (mapped < 0 || mapped >= length)
            {
                return -1;
            }

            return mapped;
        }

        public static void UpdateHighest(Fractal fractal, int value)
        {
            int current;
            while (value > (current = fractal.highestActual))
            {
                if (Interlocked.CompareExchange(ref fractal.highestActual, value, current) == current)
                {
                    break;
                }
            }
        }
    }

    public class PickoverBiomorph : Fractal
    {
        private readonly int bailout;
        private readonly double escapeComponent;
        private readonly NumericsComplex c;

        public PickoverBiomorph(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "PickoverBiomorph";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
            escapeComponent = 10.0;
            c = new NumericsComplex(0.1, 0.6);
        }

        public override Fractal Render()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    NumericsComplex z = new NumericsComplex(domain[x][y][0], domain[x][y][1]);
                    int iterations = 0;
                    double totalDistance = 0.0;
                    while (iterations < bailout && Math.Abs(z.Real) <= escapeComponent && Math.Abs(z.Imaginary) <= escapeComponent)
                    {
                        NumericsComplex next = NumericsComplex.Sin(z) + NumericsComplex.Exp(z) + c;
                        totalDistance += (next - z).Magnitude;
                        z = next;
                        iterations++;
                        if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                        {
                            break;
                        }
                    }

                    int index = x + y * width;
                    double componentExcess = Math.Max(Math.Abs(z.Real), Math.Abs(z.Imaginary));
                    exposure[index] = iterations;
                    distance[index] = Math.Log(1.0 + totalDistance + componentExcess);
                    RareFractalMath.UpdateHighest(this, exposure[index]);
                }
            });
            return this;
        }
    }

    public class PopcornFractal : Fractal
    {
        private readonly int bailout;
        private readonly double h;
        private const double PlotMin = -3.0;
        private const double PlotMax = 3.0;

        public PopcornFractal(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "Popcorn";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
            h = 0.05;
        }

        public override Fractal Render()
        {
            int steps = Math.Min(bailout, 400);
            Parallel.For(0, width, px =>
            {
                for (int py = 0; py < height; py++)
                {
                    double x = domain[px][py][0];
                    double y = domain[px][py][1];
                    for (int i = 0; i < steps; i++)
                    {
                        double nextX = x - h * Math.Sin(y + Math.Tan(3.0 * y));
                        double nextY = y - h * Math.Sin(x + Math.Tan(3.0 * x));
                        x = nextX;
                        y = nextY;
                        int ix = RareFractalMath.MapToPixel(x, PlotMin, PlotMax, width);
                        int iy = RareFractalMath.MapToPixel(y, PlotMin, PlotMax, height);
                        if (ix >= 0 && iy >= 0)
                        {
                            int value = Interlocked.Increment(ref exposure[ix + iy * width]);
                            if (value > highestExposureTarget * 256) { break; }
                        }
                        if (double.IsNaN(x) || double.IsNaN(y) || Math.Abs(x) > 1.0e6 || Math.Abs(y) > 1.0e6) { break; }
                    }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class CliffordAttractor : Fractal
    {
        private readonly int bailout;
        private const double A = -1.4, B = 1.6, C = 1.0, D = 0.7;
        private const double PlotMin = -2.5, PlotMax = 2.5;

        public CliffordAttractor(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "CliffordAttractor";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            int seeds = Math.Max(1, Math.Min(width * height, width * 16));
            int steps = Math.Min(Math.Max(bailout, 256), 10000);
            Parallel.For(0, seeds, seed =>
            {
                double x = -0.01 + seed * 0.000001;
                double y = 0.01 - seed * 0.000001;
                for (int i = 0; i < steps; i++)
                {
                    double nx = Math.Sin(A * y) + C * Math.Cos(A * x);
                    double ny = Math.Sin(B * x) + D * Math.Cos(B * y);
                    x = nx; y = ny;
                    if (i < 16) { continue; }
                    int ix = RareFractalMath.MapToPixel(x, PlotMin, PlotMax, width);
                    int iy = RareFractalMath.MapToPixel(y, PlotMin, PlotMax, height);
                    if (ix >= 0 && iy >= 0) { Interlocked.Increment(ref exposure[ix + iy * width]); }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class HopalongAttractor : Fractal
    {
        private readonly int bailout;
        private const double A = -2.0, B = -0.33, C = 0.01;
        private const double PlotMin = -18.0, PlotMax = 18.0;

        public HopalongAttractor(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "HopalongAttractor";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            int seeds = Math.Max(1, Math.Min(width * height, width * 16));
            int steps = Math.Min(Math.Max(bailout, 256), 10000);
            Parallel.For(0, seeds, seed =>
            {
                double x = (seed % width - width / 2.0) / width * 0.02;
                double y = (seed / Math.Max(1, width) - height / 2.0) / height * 0.02;
                for (int i = 0; i < steps; i++)
                {
                    double sign = x < 0 ? -1.0 : 1.0;
                    double nx = y - sign * Math.Sqrt(Math.Abs(B * x - C));
                    double ny = A - x;
                    x = nx; y = ny;
                    if (i < 8) { continue; }
                    int ix = RareFractalMath.MapToPixel(x, PlotMin, PlotMax, width);
                    int iy = RareFractalMath.MapToPixel(y, PlotMin, PlotMax, height);
                    if (ix >= 0 && iy >= 0) { Interlocked.Increment(ref exposure[ix + iy * width]); }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class GumowskiMiraAttractor : Fractal
    {
        private readonly int bailout;
        private const double A = 0.008, B = 0.05;
        private const double PlotMin = -24.0, PlotMax = 24.0;

        public GumowskiMiraAttractor(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "GumowskiMira";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        private static double F(double x)
        {
            return A * x + (2.0 * (1.0 - A) * x * x) / (1.0 + x * x);
        }

        public override Fractal Render()
        {
            int seeds = Math.Max(1, Math.Min(width * height, width * 16));
            int steps = Math.Min(Math.Max(bailout, 256), 10000);
            Parallel.For(0, seeds, seed =>
            {
                double angle = 2.0 * Math.PI * seed / seeds;
                double x = 10.0 + 0.01 * Math.Cos(angle);
                double y = 0.01 * Math.Sin(angle);
                for (int i = 0; i < steps; i++)
                {
                    double xn = y + B * (1.0 - 0.05 * y * y) * y + F(x);
                    double yn = -x + F(xn);
                    x = xn; y = yn;
                    if (i < 32) { continue; }
                    int ix = RareFractalMath.MapToPixel(x, PlotMin, PlotMax, width);
                    int iy = RareFractalMath.MapToPixel(y, PlotMin, PlotMax, height);
                    if (ix >= 0 && iy >= 0) { Interlocked.Increment(ref exposure[ix + iy * width]); }
                    if (Math.Abs(x) > 1.0e6 || Math.Abs(y) > 1.0e6 || double.IsNaN(x) || double.IsNaN(y)) { break; }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class IkedaAttractor : Fractal
    {
        private readonly int bailout;
        private const double U = 0.918;
        private const double PlotMin = -2.0, PlotMax = 3.0;

        public IkedaAttractor(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "IkedaAttractor";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            int seeds = Math.Max(1, Math.Min(width * height, width * 16));
            int steps = Math.Min(Math.Max(bailout, 256), 10000);
            Parallel.For(0, seeds, seed =>
            {
                double angle = 2.0 * Math.PI * seed / seeds;
                double radius = 0.5 + (seed % 997) / 997.0 * 1.5;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                for (int i = 0; i < steps; i++)
                {
                    double t = 0.4 - 6.0 / (1.0 + x * x + y * y);
                    double nx = 1.0 + U * (x * Math.Cos(t) - y * Math.Sin(t));
                    double ny = U * (x * Math.Sin(t) + y * Math.Cos(t));
                    x = nx; y = ny;
                    if (i < 64) { continue; }
                    int ix = RareFractalMath.MapToPixel(x, PlotMin, PlotMax, width);
                    int iy = RareFractalMath.MapToPixel(y, PlotMin, PlotMax, height);
                    if (ix >= 0 && iy >= 0) { Interlocked.Increment(ref exposure[ix + iy * width]); }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class TinkerbellAttractor : Fractal
    {
        private readonly int bailout;
        private const double A = 0.9, B = -0.6013, C = 2.0, D = 0.5;
        private const double PlotMinX = -2.0, PlotMaxX = 2.0, PlotMinY = -2.0, PlotMaxY = 1.2;

        public TinkerbellAttractor(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "TinkerbellAttractor";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            int seeds = Math.Max(1, Math.Min(width * height, width * 16));
            int steps = Math.Min(Math.Max(bailout, 256), 10000);
            Parallel.For(0, seeds, seed =>
            {
                double angle = 2.0 * Math.PI * seed / seeds;
                double x = -0.72 + 0.002 * Math.Cos(angle);
                double y = -0.64 + 0.002 * Math.Sin(angle);
                for (int i = 0; i < steps; i++)
                {
                    double nx = x * x - y * y + A * x + B * y;
                    double ny = 2.0 * x * y + C * x + D * y;
                    x = nx; y = ny;
                    if (i < 32) { continue; }
                    int ix = RareFractalMath.MapToPixel(x, PlotMinX, PlotMaxX, width);
                    int iy = RareFractalMath.MapToPixel(y, PlotMinY, PlotMaxY, height);
                    if (ix >= 0 && iy >= 0) { Interlocked.Increment(ref exposure[ix + iy * width]); }
                    if (Math.Abs(x) > 1.0e6 || Math.Abs(y) > 1.0e6 || double.IsNaN(x) || double.IsNaN(y)) { break; }
                }
            });
            FinalizeHistogramDistance();
            return this;
        }

        private void FinalizeHistogramDistance()
        {
            Parallel.For(0, exposure.Length, i => distance[i] = Math.Log(1.0 + exposure[i]));
            foreach (int v in exposure) { RareFractalMath.UpdateHighest(this, v); }
        }
    }

    public class KleinianInversionLimitSet : Fractal
    {
        private readonly int bailout;

        public KleinianInversionLimitSet(int width, int height, int bailout, int highestExposureTarget)
        {
            name = "KleinianInversion";
            this.width = width;
            this.height = height;
            this.bailout = bailout;
            this.highestExposureTarget = highestExposureTarget;
        }

        public override Fractal Render()
        {
            int iterationsLimit = Math.Min(Math.Max(bailout, 64), 512);
            Parallel.For(0, width, xPixel =>
            {
                for (int yPixel = 0; yPixel < height; yPixel++)
                {
                    double x = domain[xPixel][yPixel][0];
                    double y = domain[xPixel][yPixel][1];
                    double scale = 1.0;
                    double minRadius = double.MaxValue;
                    int iterations = 0;
                    for (; iterations < iterationsLimit; iterations++)
                    {
                        // A compact Schottky/Kleinian-inspired fold: reflect into a quadrant,
                        // invert through the unit circle, then translate to the next circle pair.
                        x = Math.Abs(x);
                        y = Math.Abs(y);
                        double r2 = x * x + y * y;
                        minRadius = Math.Min(minRadius, Math.Sqrt(r2));
                        if (r2 < 1.0)
                        {
                            double k = 1.0 / Math.Max(r2, 1.0e-12);
                            x *= k;
                            y *= k;
                            scale *= k;
                        }

                        x -= 1.35;
                        y = Math.Abs(y - 0.35) - 0.35;
                        if (x * x + y * y > 256.0) { break; }
                    }

                    int index = xPixel + yPixel * width;
                    exposure[index] = iterations;
                    distance[index] = Math.Log(1.0 + minRadius + Math.Log(1.0 + scale));
                    RareFractalMath.UpdateHighest(this, exposure[index]);
                }
            });
            return this;
        }
    }
}
