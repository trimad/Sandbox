using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceBinned()
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

            double scale = 255.0 / (max - min);
            _ = Parallel.For(0, distance.Length, i =>
            {
                double value = distance[i];
                int shade = double.IsNaN(value) || double.IsInfinity(value)
                    ? 0
                    : ClampToByte((value - min) * scale);

                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
