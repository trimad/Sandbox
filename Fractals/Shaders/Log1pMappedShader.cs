using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Log1pMapped(double strength = 64.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int peak = GetExposurePeak();

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalized = peak <= 0 ? 0 : Clamp01((double)exposure[i] / peak);
                int shade = ClampToByte(ApplyLog1p(normalized, strength) * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
