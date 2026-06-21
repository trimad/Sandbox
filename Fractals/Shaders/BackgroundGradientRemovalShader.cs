using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader BackgroundGradientRemoval(int radius = 32)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            radius = Math.Max(1, radius);
            int[] source = (int[])canvas.Clone();
            int[] background = BlurCanvas(source, radius);
            double avgR = 0;
            double avgG = 0;
            double avgB = 0;

            for (int i = 0; i < background.Length; i++)
            {
                avgR += Red(background[i]);
                avgG += Green(background[i]);
                avgB += Blue(background[i]);
            }

            avgR /= background.Length;
            avgG /= background.Length;
            avgB /= background.Length;

            _ = Parallel.For(0, source.Length, i =>
            {
                canvas[i] = PackColor(
                    Red(source[i]) - Red(background[i]) + avgR,
                    Green(source[i]) - Green(background[i]) + avgG,
                    Blue(source[i]) - Blue(background[i]) + avgB);
            });

            return this;
        }
    }
}
