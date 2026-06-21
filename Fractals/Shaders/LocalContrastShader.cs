using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader LocalContrast(int radius = 12, double amount = 1.25)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            radius = Math.Max(1, radius);
            amount = Math.Max(0, amount);
            int[] source = (int[])canvas.Clone();
            int[] blurred = BlurCanvas(source, radius);

            _ = Parallel.For(0, source.Length, i =>
            {
                canvas[i] = PackColor(
                    Red(source[i]) + amount * (Red(source[i]) - Red(blurred[i])),
                    Green(source[i]) + amount * (Green(source[i]) - Green(blurred[i])),
                    Blue(source[i]) + amount * (Blue(source[i]) - Blue(blurred[i])));
            });

            return this;
        }
    }
}
