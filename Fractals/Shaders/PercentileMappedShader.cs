using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader PercentileMapped(double lowPercentile = 0.01, double highPercentile = 0.995)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            int peak = GetExposurePeak();
            EnsureCanvasLength(exposure.Length);
            if (peak <= 0)
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            lowPercentile = Clamp01(lowPercentile);
            highPercentile = Clamp01(highPercentile);
            if (highPercentile < lowPercentile)
            {
                (lowPercentile, highPercentile) = (highPercentile, lowPercentile);
            }

            long[] histogram = new long[peak + 1];
            long nonZeroCount = 0;
            for (int i = 0; i < exposure.Length; i++)
            {
                int value = exposure[i];
                if (value <= 0)
                {
                    continue;
                }

                histogram[value]++;
                nonZeroCount++;
            }

            if (nonZeroCount == 0)
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            long lowTarget = (long)Math.Round((nonZeroCount - 1) * lowPercentile);
            long highTarget = (long)Math.Round((nonZeroCount - 1) * highPercentile);
            long cumulative = 0;
            int lowValue = 1;
            int highValue = peak;

            for (int i = 1; i < histogram.Length; i++)
            {
                cumulative += histogram[i];
                if (cumulative > lowTarget)
                {
                    lowValue = i;
                    break;
                }
            }

            cumulative = 0;
            for (int i = 1; i < histogram.Length; i++)
            {
                cumulative += histogram[i];
                if (cumulative > highTarget)
                {
                    highValue = i;
                    break;
                }
            }

            if (highValue <= lowValue)
            {
                highValue = lowValue + 1;
            }

            double scale = 255.0 / (highValue - lowValue);
            _ = Parallel.For(0, exposure.Length, i =>
            {
                int value = exposure[i];
                int shade;
                if (value <= 0 || value <= lowValue)
                {
                    shade = 0;
                }
                else if (value >= highValue)
                {
                    shade = 255;
                }
                else
                {
                    shade = ClampToByte((value - lowValue) * scale);
                }

                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
