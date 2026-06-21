using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Exponential(double exponent)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0)
            {
                return this;
            }

            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            _ = Parallel.For(0, distance.Length, i =>
            {
                int y = ClampToByte(Math.Pow(NormalizeDistanceValue(distance[i], min, max), exponent) * 255);
                canvas[i] = PackColor(y, y, y);
            });

            return this;
        }
    }
}
