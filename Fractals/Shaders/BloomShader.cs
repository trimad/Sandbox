using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Bloom(int radius = 6, double threshold = 0.6, double intensity = 0.75)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            threshold = Clamp01(threshold);
            intensity = Math.Max(0, intensity);

            int[] source = (int[])canvas.Clone();
            int[] highlights = new int[source.Length];
            _ = Parallel.For(0, source.Length, i =>
            {
                double lightness = Luma(source[i]) / 255.0;
                if (lightness <= threshold)
                {
                    highlights[i] = PackColor(0, 0, 0);
                    return;
                }

                double scale = threshold >= 1 ? 0 : (lightness - threshold) / (1 - threshold);
                highlights[i] = PackColor(
                    Red(source[i]) * scale,
                    Green(source[i]) * scale,
                    Blue(source[i]) * scale);
            });

            int[] glow = BlurCanvas(highlights, radius);
            _ = Parallel.For(0, source.Length, i =>
            {
                canvas[i] = PackColor(
                    Red(source[i]) + Red(glow[i]) * intensity,
                    Green(source[i]) + Green(glow[i]) * intensity,
                    Blue(source[i]) + Blue(glow[i]) * intensity);
            });

            return this;
        }
    }
}
