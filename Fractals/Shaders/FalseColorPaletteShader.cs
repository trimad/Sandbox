using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader FalseColorPalette()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();
            double distanceMin = 0;
            double distanceMax = 0;
            bool hasDistanceRange = distance != null
                && distance.Length == exposure.Length
                && TryGetDistanceRange(out distanceMin, out distanceMax);

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double intensity = ApplyLog1p(normalizedExposure, 64.0);
                double distanceTone = hasDistanceRange
                    ? NormalizeDistanceValue(distance[i], distanceMin, distanceMax)
                    : intensity;
                double palettePosition = Clamp01(intensity * 0.75 + distanceTone * 0.25);
                Color color = InterpolatePalette(FalseColorPaletteStops, palettePosition);

                canvas[i] = PackColor(
                    color.R * intensity,
                    color.G * intensity,
                    color.B * intensity);
            });

            return this;
        }
    }
}
