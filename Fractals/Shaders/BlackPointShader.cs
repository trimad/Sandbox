using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader BlackPoint(double blackPoint = 0.03)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            blackPoint = Clamp01(blackPoint);
            double divisor = Math.Max(0.000001, 1 - blackPoint);
            double[] values = GetNormalizedExposure();

            _ = Parallel.For(0, values.Length, i =>
            {
                values[i] = Clamp01((values[i] - blackPoint) / divisor);
            });

            WriteGrayscale(values);
            return this;
        }
    }
}
