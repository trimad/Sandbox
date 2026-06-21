using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader SigmoidContrast(double midpoint = 0.35, double strength = 12.0)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0)
            {
                return this;
            }

            double[] normalized = GetNormalizedExposure();
            _ = Parallel.For(0, normalized.Length, i =>
            {
                normalized[i] = ApplySigmoid(normalized[i], midpoint, strength);
            });

            WriteGrayscale(normalized);
            return this;
        }
    }
}
