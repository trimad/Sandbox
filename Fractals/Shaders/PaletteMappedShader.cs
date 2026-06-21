using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader PaletteMapped()
        {
            return PaletteMapped(DefaultPaletteStops);
        }

        public Shader PaletteMapped(Color[] stops)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            double[] normalized = GetNormalizedExposure();
            EnsureCanvasLength(normalized.Length);
            _ = Parallel.For(0, normalized.Length, i =>
            {
                double t = ApplyLog1p(normalized[i], 64.0);
                Color color = InterpolatePalette(stops, t);
                canvas[i] = PackColor(color.R, color.G, color.B);
            });

            return this;
        }
    }
}
