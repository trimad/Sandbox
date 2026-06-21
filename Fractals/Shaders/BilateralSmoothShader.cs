using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader BilateralSmooth(int radius = 2, double sigmaSpatial = 2.0, double sigmaRange = 0.10)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            radius = Math.Max(1, radius);
            sigmaSpatial = Math.Max(0.000001, sigmaSpatial);
            sigmaRange = Math.Max(0.000001, sigmaRange);
            int[] source = (int[])canvas.Clone();
            double spatialDenominator = 2 * sigmaSpatial * sigmaSpatial;
            double rangeDenominator = 2 * sigmaRange * sigmaRange;

            _ = Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    double centerLuma = Luma(source[index]) / 255.0;
                    double totalWeight = 0;
                    double r = 0;
                    double g = 0;
                    double b = 0;

                    for (int yy = y - radius; yy <= y + radius; yy++)
                    {
                        int sampleY = ClampIndex(yy, height);
                        int dy = yy - y;
                        for (int xx = x - radius; xx <= x + radius; xx++)
                        {
                            int sampleX = ClampIndex(xx, width);
                            int dx = xx - x;
                            int pixel = source[sampleY * width + sampleX];
                            double lumaDelta = Luma(pixel) / 255.0 - centerLuma;
                            double spatial = Math.Exp(-(dx * dx + dy * dy) / spatialDenominator);
                            double range = Math.Exp(-(lumaDelta * lumaDelta) / rangeDenominator);
                            double weight = spatial * range;

                            r += Red(pixel) * weight;
                            g += Green(pixel) * weight;
                            b += Blue(pixel) * weight;
                            totalWeight += weight;
                        }
                    }

                    canvas[index] = totalWeight == 0
                        ? source[index]
                        : PackColor(r / totalWeight, g / totalWeight, b / totalWeight);
                }
            });

            return this;
        }
    }
}
