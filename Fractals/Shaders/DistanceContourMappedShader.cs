using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistanceContourMapped(int bands = 16)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0)
            {
                return this;
            }

            bands = Math.Max(2, bands);
            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            _ = Parallel.For(0, distance.Length, i =>
            {
                double stretched = ApplyLog1p(NormalizeDistanceValue(distance[i], min, max), 64.0);
                double scaled = stretched * bands;
                int band = Math.Min(bands - 1, (int)Math.Floor(scaled));
                double baseShade = (double)band / (bands - 1);
                double fractional = scaled - Math.Floor(scaled);
                double line = fractional < 0.08 ? 0.2 : 1.0;
                int shade = ClampToByte(baseShade * line * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
