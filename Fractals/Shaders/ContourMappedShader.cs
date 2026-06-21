using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader ContourMapped(int bands = 16)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            bands = Math.Max(2, bands);
            double[] normalized = GetNormalizedExposure();
            _ = Parallel.For(0, normalized.Length, i =>
            {
                double stretched = ApplyLog1p(normalized[i], 64.0);
                double scaled = stretched * bands;
                int band = Math.Min(bands - 1, (int)Math.Floor(scaled));
                double baseShade = (double)band / (bands - 1);
                double fractional = scaled - Math.Floor(scaled);
                double line = fractional < 0.08 ? 0.2 : 1.0;
                normalized[i] = baseShade * line;
            });

            WriteGrayscale(normalized);
            return this;
        }
    }
}
