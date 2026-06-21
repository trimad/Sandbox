using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader EmbossLight(double lightX = 1.0, double lightY = -1.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            int[] source = (int[])canvas.Clone();
            int[] embossed = new int[source.Length];
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

                    double left = Luma(source[y * width + x0]);
                    double right = Luma(source[y * width + x1]);
                    double up = Luma(source[y0 * width + x]);
                    double down = Luma(source[y1 * width + x]);

                    double nx = -(right - left) / 255.0;
                    double ny = -(down - up) / 255.0;
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
                    shade = 0.35 + shade * 0.65;

                    embossed[idx] = PackColor(
                        Red(source[idx]) * shade,
                        Green(source[idx]) * shade,
                        Blue(source[idx]) * shade);
                }
            });

            canvas = embossed;
            return this;
        }
    }
}
