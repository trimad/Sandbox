using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader SmoothStep()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            double[] normalized = Helper.NormalizeArray(exposure);

            _ = Parallel.For(0, normalized.Length, i =>
            {
                double k = Clamp01(normalized[i]);
                double smooth = (k * k) * (3 - (2 * k));
                int shade = (int)(smooth * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
