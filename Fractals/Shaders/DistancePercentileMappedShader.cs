using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistancePercentileMapped(double lowPercentile = 0.01, double highPercentile = 0.995)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0)
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

            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            long[] histogram = new long[bins];
            object histogramLock = new object();

            _ = Parallel.For(
                0,
                distance.Length,
                () => new long[bins],
                (i, _, local) =>
                {
                    double normalized = NormalizeDistanceValue(distance[i], min, max);
                    if (normalized <= 0)
                    {
                        return local;
                    }

                    int bin = Math.Min(bins - 1, (int)(normalized * (bins - 1)));
                    local[bin]++;
                    return local;
                },
                local =>
                {
                    lock (histogramLock)
                    {
                        for (int i = 0; i < bins; i++)
                        {
                            histogram[i] += local[i];
                        }
                    }
                });

            long total = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                total += histogram[i];
            }

            if (total == 0)
            {
                Array.Clear(canvas, 0, canvas.Length);
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

            _ = Parallel.For(0, distance.Length, i =>
            {
                double normalized = NormalizeDistanceValue(distance[i], min, max);
                int shade = ClampToByte(Clamp01((normalized - low) / range) * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
