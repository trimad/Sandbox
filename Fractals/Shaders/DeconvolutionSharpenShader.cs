using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DeconvolutionSharpen(int radius = 1, int iterations = 2, double amount = 0.75)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            radius = Math.Max(1, radius);
            iterations = Math.Max(1, iterations);
            amount = Math.Max(0, amount);
            int[] source = (int[])canvas.Clone();
            double[] original = GetCanvasLumaNormalized();
            double[] estimate = (double[])original.Clone();

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                double[] blurred = BlurValues(estimate, radius);
                double[] ratio = new double[estimate.Length];

                _ = Parallel.For(0, ratio.Length, i =>
                {
                    ratio[i] = original[i] / Math.Max(0.000001, blurred[i]);
                });

                double[] correction = BlurValues(ratio, radius);
                _ = Parallel.For(0, estimate.Length, i =>
                {
                    estimate[i] = Clamp01(estimate[i] * correction[i]);
                });
            }

            _ = Parallel.For(0, source.Length, i =>
            {
                double delta = estimate[i] - original[i];
                double scale = original[i] <= 0 ? 1 : 1 + amount * delta / original[i];
                canvas[i] = PackColor(
                    Red(source[i]) * scale,
                    Green(source[i]) * scale,
                    Blue(source[i]) * scale);
            });

            return this;
        }
    }
}
