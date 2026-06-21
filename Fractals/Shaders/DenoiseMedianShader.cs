using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DenoiseMedian()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            int[] source = (int[])canvas.Clone();
            _ = Parallel.For(0, height, y =>
            {
                int[] r = new int[9];
                int[] g = new int[9];
                int[] b = new int[9];

                for (int x = 0; x < width; x++)
                {
                    int count = 0;
                    for (int yy = y - 1; yy <= y + 1; yy++)
                    {
                        int sampleY = ClampIndex(yy, height);
                        for (int xx = x - 1; xx <= x + 1; xx++)
                        {
                            int pixel = source[sampleY * width + ClampIndex(xx, width)];
                            r[count] = Red(pixel);
                            g[count] = Green(pixel);
                            b[count] = Blue(pixel);
                            count++;
                        }
                    }

                    Array.Sort(r);
                    Array.Sort(g);
                    Array.Sort(b);
                    canvas[y * width + x] = PackColor(r[4], g[4], b[4]);
                }
            });

            return this;
        }
    }
}
