using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader DistancePaletteMapped()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (distance == null || distance.Length == 0)
            {
                return this;
            }

            EnsureCanvasLength(distance.Length);
            if (!TryGetDistanceRange(out double min, out double max))
            {
                Array.Clear(canvas, 0, canvas.Length);
                return this;
            }

            _ = Parallel.For(0, distance.Length, i =>
            {
                double t = ApplyLog1p(NormalizeDistanceValue(distance[i], min, max), 64.0);
                Color color = InterpolatePalette(DefaultPaletteStops, t);
                canvas[i] = PackColor(color.R, color.G, color.B);
            });

            return this;
        }
    }
}
