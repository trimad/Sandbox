using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader SobelEdges()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0 || width < 3 || height < 3)
            {
                return this;
            }

            int[] source = (int[])canvas.Clone();
            int[] edges = new int[source.Length];
            _ = Parallel.For(1, height - 1, y =>
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int idx = y * width + x;
                    double topLeft = Luma(source[idx - width - 1]);
                    double top = Luma(source[idx - width]);
                    double topRight = Luma(source[idx - width + 1]);
                    double left = Luma(source[idx - 1]);
                    double right = Luma(source[idx + 1]);
                    double bottomLeft = Luma(source[idx + width - 1]);
                    double bottom = Luma(source[idx + width]);
                    double bottomRight = Luma(source[idx + width + 1]);

                    double gx = -topLeft + topRight - 2 * left + 2 * right - bottomLeft + bottomRight;
                    double gy = topLeft + 2 * top + topRight - bottomLeft - 2 * bottom - bottomRight;
                    double magnitude = Math.Min(255, Math.Sqrt(gx * gx + gy * gy));
                    edges[idx] = PackColor(magnitude, magnitude, magnitude);
                }
            });

            canvas = edges;
            return this;
        }
    }
}
