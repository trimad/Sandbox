using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader SaturationBoost(double amount = 1.35)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            amount = Math.Max(0, amount);
            int[] source = (int[])canvas.Clone();

            _ = Parallel.For(0, source.Length, i =>
            {
                double r = Red(source[i]);
                double g = Green(source[i]);
                double b = Blue(source[i]);
                double gray = (r + g + b) / 3.0;

                canvas[i] = PackColor(
                    gray + (r - gray) * amount,
                    gray + (g - gray) * amount,
                    gray + (b - gray) * amount);
            });

            return this;
        }
    }
}
