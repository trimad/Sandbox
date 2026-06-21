using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceSobelEdges()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0 || width < 3 || height < 3)
            {
                return this;
            }

            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            _ = Parallel.For(1, height - 1, y =>
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int idx = y * width + x;
                    double topLeft = NormalizeDistanceValue(distance[idx - width - 1], min, max);
                    double top = NormalizeDistanceValue(distance[idx - width], min, max);
                    double topRight = NormalizeDistanceValue(distance[idx - width + 1], min, max);
                    double left = NormalizeDistanceValue(distance[idx - 1], min, max);
                    double right = NormalizeDistanceValue(distance[idx + 1], min, max);
                    double bottomLeft = NormalizeDistanceValue(distance[idx + width - 1], min, max);
                    double bottom = NormalizeDistanceValue(distance[idx + width], min, max);
                    double bottomRight = NormalizeDistanceValue(distance[idx + width + 1], min, max);

                    double gx = -topLeft + topRight - 2 * left + 2 * right - bottomLeft + bottomRight;
                    double gy = topLeft + 2 * top + topRight - bottomLeft - 2 * bottom - bottomRight;
                    double magnitude = Clamp01(Math.Sqrt(gx * gx + gy * gy));
                    int shade = ClampToByte(magnitude * 255);
                    canvas[idx] = PackColor(shade, shade, shade);
                }
            });

            return this;
        }
    }
}
