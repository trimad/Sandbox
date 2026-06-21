using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader StarMask(double threshold = 0.75)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0 || width < 3 || height < 3)
            {
                return this;
            }

            threshold = Clamp01(threshold);
            int[] source = (int[])canvas.Clone();

            _ = Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    double current = Luma(source[index]) / 255.0;
                    double neighborhood = 0;
                    int samples = 0;

                    for (int yy = Math.Max(0, y - 1); yy <= Math.Min(height - 1, y + 1); yy++)
                    {
                        for (int xx = Math.Max(0, x - 1); xx <= Math.Min(width - 1, x + 1); xx++)
                        {
                            if (xx == x && yy == y)
                            {
                                continue;
                            }

                            neighborhood += Luma(source[yy * width + xx]) / 255.0;
                            samples++;
                        }
                    }

                    double localAverage = samples == 0 ? current : neighborhood / samples;
                    double isStar = current > threshold && current - localAverage > 0.05 ? 255 : 0;
                    canvas[index] = PackColor(isStar, isStar, isStar);
                }
            });

            return this;
        }
    }
}
