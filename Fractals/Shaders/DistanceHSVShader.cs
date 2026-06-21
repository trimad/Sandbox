using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceHSV()
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
                double normalized = NormalizeDistanceValue(distance[i], min, max);
                canvas[i] = PackHsv(normalized * 360, 1, 1);
            });

            return this;
        }
    }
}
