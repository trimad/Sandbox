using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader ScreenStretch(double lowPercentile = 0.001, double highPercentile = 0.999, double strength = 12.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            const int bins = 4096;
            lowPercentile = Clamp01(lowPercentile);
            highPercentile = Clamp01(highPercentile);
            if (highPercentile < lowPercentile)
            {
                (lowPercentile, highPercentile) = (highPercentile, lowPercentile);
            }

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

            long lowTarget = (long)((total - 1) * lowPercentile);
            long highTarget = (long)((total - 1) * highPercentile);
            long cumulative = 0;
            int lowBin = 0;
            int highBin = bins - 1;

            for (int i = 0; i < bins; i++)
            {
                cumulative += histogram[i];
                if (cumulative > lowTarget)
                {
                    lowBin = i;
                    break;
                }
            }

            cumulative = 0;
            for (int i = 0; i < bins; i++)
            {
                cumulative += histogram[i];
                if (cumulative > highTarget)
                {
                    highBin = i;
                    break;
                }
            }

            double low = (double)lowBin / (bins - 1);
            double high = Math.Max(low + 0.000001, (double)highBin / (bins - 1));
            double range = high - low;

            _ = Parallel.For(0, values.Length, i =>
            {
                values[i] = ApplyAsinh((values[i] - low) / range, strength);
            });

            WriteGrayscale(values);
            return this;
        }
    }
}
