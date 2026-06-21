using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceEmbossLight(double lightX = 1.0, double lightY = -1.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0 || width <= 0 || height <= 0)
            {
                return this;
            }

            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            double lightZ = 1.0;
            double lightLength = Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
            if (lightLength == 0)
            {
                lightLength = 1;
            }

            double lx = lightX / lightLength;
            double ly = lightY / lightLength;
            double lz = lightZ / lightLength;

            _ = Parallel.For(0, height, y =>
            {
                int y0 = ClampIndex(y - 1, height);
                int y1 = ClampIndex(y + 1, height);
                for (int x = 0; x < width; x++)
                {
                    int x0 = ClampIndex(x - 1, width);
                    int x1 = ClampIndex(x + 1, width);
                    int idx = y * width + x;

                    double left = NormalizeDistanceValue(distance[y * width + x0], min, max);
                    double right = NormalizeDistanceValue(distance[y * width + x1], min, max);
                    double up = NormalizeDistanceValue(distance[y0 * width + x], min, max);
                    double down = NormalizeDistanceValue(distance[y1 * width + x], min, max);

                    double nx = -(right - left);
                    double ny = -(down - up);
                    double nz = 1.0;
                    double normalLength = Math.Sqrt(nx * nx + ny * ny + nz * nz);
                    if (normalLength == 0)
                    {
                        normalLength = 1;
                    }

                    nx /= normalLength;
                    ny /= normalLength;
                    nz /= normalLength;

                    double shade = Math.Max(0, nx * lx + ny * ly + nz * lz);
                    double value = Clamp01(NormalizeDistanceValue(distance[idx], min, max) * (0.4 + shade * 0.9));
                    int output = ClampToByte(value * 255);
                    canvas[idx] = PackColor(output, output, output);
                }
            });

            return this;
        }
    }
}
