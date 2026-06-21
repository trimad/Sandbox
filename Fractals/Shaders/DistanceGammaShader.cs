using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceGamma(double gamma = 2.2)
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

            double exponent = gamma <= 0 ? 1 : 1.0 / gamma;
            _ = Parallel.For(0, distance.Length, i =>
            {
                double value = Math.Pow(NormalizeDistanceValue(distance[i], min, max), exponent);
                int shade = ClampToByte(value * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
