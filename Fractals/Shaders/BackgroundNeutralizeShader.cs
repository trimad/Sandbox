using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader BackgroundNeutralize(double backgroundPercentile = 0.25)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            backgroundPercentile = Clamp01(backgroundPercentile);
            int[] source = (int[])canvas.Clone();
            long[] histogram = new long[256];
            for (int i = 0; i < source.Length; i++)
            {
                histogram[ClampToByte(Luma(source[i]))]++;
            }

            long target = (long)((source.Length - 1) * backgroundPercentile);
            long cumulative = 0;
            int threshold = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                cumulative += histogram[i];
                if (cumulative >= target)
                {
                    threshold = i;
                    break;
                }
            }

            double avgR = 0;
            double avgG = 0;
            double avgB = 0;
            long count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (Luma(source[i]) > threshold)
                {
                    continue;
                }

                avgR += Red(source[i]);
                avgG += Green(source[i]);
                avgB += Blue(source[i]);
                count++;
            }

            if (count == 0)
            {
                return this;
            }

            avgR /= count;
            avgG /= count;
            avgB /= count;
            double neutral = (avgR + avgG + avgB) / 3.0;
            double dr = neutral - avgR;
            double dg = neutral - avgG;
            double db = neutral - avgB;

            _ = Parallel.For(0, source.Length, i =>
            {
                canvas[i] = PackColor(
                    Red(source[i]) + dr,
                    Green(source[i]) + dg,
                    Blue(source[i]) + db);
            });

            return this;
        }
    }
}
