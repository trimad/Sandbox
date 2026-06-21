using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader LogBaseHighest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            highestActual = Helper.GetMinMax(exposure).Item2;

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double log = highestActual <= 1 || exposure[i] <= 0
                    ? 0
                    : Math.Log(exposure[i], highestActual);
                int shade = (int)Helper.Map(log, 0d, 1d, 0d, 255d);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }
    }
}
