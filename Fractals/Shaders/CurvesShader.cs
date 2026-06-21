using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Curves(double amount = 1.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            amount = Clamp01(amount);
            int[] source = (int[])canvas.Clone();

            double Curve(double channel)
            {
                double t = Clamp01(channel / 255.0);
                double curved = t < 0.5
                    ? 2 * t * t
                    : 1 - 2 * (1 - t) * (1 - t);
                return (t + (curved - t) * amount) * 255;
            }

            _ = Parallel.For(0, source.Length, i =>
            {
                canvas[i] = PackColor(
                    Curve(Red(source[i])),
                    Curve(Green(source[i])),
                    Curve(Blue(source[i])));
            });

            return this;
        }
    }
}
