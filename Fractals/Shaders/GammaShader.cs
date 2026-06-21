using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Gamma(double gamma = 2.2)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            double exponent = gamma <= 0 ? 1 : 1.0 / gamma;
            EnsureCanvasLength(exposure.Length);
            int peak = GetExposurePeak();

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalized = peak <= 0 ? 0 : Clamp01((double)exposure[i] / peak);
                int shade = ClampToByte(Math.Pow(normalized, exponent) * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
