using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader ExposureHSV()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double value = ApplyLog1p(normalizedExposure, 64.0);
                double hue = normalizedExposure * 360.0;
                canvas[i] = PackHsv(hue, 1.0, value);
            });

            return this;
        }
    }
}
