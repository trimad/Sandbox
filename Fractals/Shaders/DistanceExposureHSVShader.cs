using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceExposureHSV()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || distance == null || distance.Length != exposure.Length)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();
            bool hasDistanceRange = TryGetDistanceRange(out double distanceMin, out double distanceMax);

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double value = ApplyLog1p(normalizedExposure, 64.0);
                if (value <= 0)
                {
                    canvas[i] = PackColor(0, 0, 0);
                    return;
                }

                double normalizedDistance = hasDistanceRange
                    ? NormalizeDistanceValue(distance[i], distanceMin, distanceMax)
                    : 0;
                canvas[i] = PackHsv(normalizedDistance * 360, 1, value);
            });

            return this;
        }
    }
}
