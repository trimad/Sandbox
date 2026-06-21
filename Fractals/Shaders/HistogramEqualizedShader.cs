using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader HistogramEqualized(int bins = 4096)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            bins = Math.Max(16, bins);
            double[] values = GetNormalizedExposure();
            long[] histogram = new long[bins];
            long total = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] <= 0)
                {
                    continue;
                }

                int bin = Math.Min(bins - 1, (int)(values[i] * (bins - 1)));
                histogram[bin]++;
                total++;
            }

            if (total == 0)
            {
                WriteGrayscale(values);
                return this;
            }

            double[] cdf = new double[bins];
            long cumulative = 0;
            for (int i = 0; i < bins; i++)
            {
                cumulative += histogram[i];
                cdf[i] = (double)cumulative / total;
            }

            EnsureCanvasLength(values.Length);
            _ = Parallel.For(0, values.Length, i =>
            {
                if (values[i] <= 0)
                {
                    canvas[i] = PackColor(0, 0, 0);
                    return;
                }

                int bin = Math.Min(bins - 1, (int)(values[i] * (bins - 1)));
                int shade = ClampToByte(cdf[bin] * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
