using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Clahe(int tileSize = 256, int bins = 256, double clipLimit = 4.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || width <= 0 || height <= 0)
            {
                return this;
            }

            tileSize = Math.Max(16, tileSize);
            bins = Math.Max(16, bins);
            clipLimit = Math.Max(1, clipLimit);
            double[] values = GetNormalizedExposure();
            EnsureCanvasLength(values.Length);

            int tileColumns = (width + tileSize - 1) / tileSize;
            int tileRows = (height + tileSize - 1) / tileSize;

            _ = Parallel.For(0, tileRows, tileY =>
            {
                for (int tileX = 0; tileX < tileColumns; tileX++)
                {
                    int x0 = tileX * tileSize;
                    int y0 = tileY * tileSize;
                    int x1 = Math.Min(width, x0 + tileSize);
                    int y1 = Math.Min(height, y0 + tileSize);
                    int tilePixelCount = (x1 - x0) * (y1 - y0);
                    int[] histogram = new int[bins];

                    for (int y = y0; y < y1; y++)
                    {
                        int rowStart = y * width;
                        for (int x = x0; x < x1; x++)
                        {
                            int bin = Math.Min(bins - 1, (int)(values[rowStart + x] * (bins - 1)));
                            histogram[bin]++;
                        }
                    }

                    int maxBinCount = Math.Max(1, (int)(clipLimit * tilePixelCount / bins));
                    int clipped = 0;
                    for (int i = 0; i < histogram.Length; i++)
                    {
                        if (histogram[i] > maxBinCount)
                        {
                            clipped += histogram[i] - maxBinCount;
                            histogram[i] = maxBinCount;
                        }
                    }

                    int redistribute = clipped / bins;
                    for (int i = 0; i < histogram.Length; i++)
                    {
                        histogram[i] += redistribute;
                    }

                    double[] cdf = new double[bins];
                    int cumulative = 0;
                    for (int i = 0; i < bins; i++)
                    {
                        cumulative += histogram[i];
                        cdf[i] = tilePixelCount == 0 ? 0 : (double)cumulative / tilePixelCount;
                    }

                    for (int y = y0; y < y1; y++)
                    {
                        int rowStart = y * width;
                        for (int x = x0; x < x1; x++)
                        {
                            int index = rowStart + x;
                            int bin = Math.Min(bins - 1, (int)(values[index] * (bins - 1)));
                            int shade = ClampToByte(cdf[bin] * 255);
                            canvas[index] = PackColor(shade, shade, shade);
                        }
                    }
                }
            });

            return this;
        }
    }
}
