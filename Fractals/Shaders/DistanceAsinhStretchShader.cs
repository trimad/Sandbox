using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceAsinhStretch(double strength = 8.0)
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
                double value = ApplyAsinh(NormalizeDistanceValue(distance[i], min, max), strength);
                int shade = ClampToByte(value * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });
            return this;
        }
    }
}
