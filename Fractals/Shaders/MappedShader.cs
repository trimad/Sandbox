using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader Mapped()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            highestActual = Helper.GetMinMax(exposure).Item2;

            _ = Parallel.For(0, exposure.Length, i =>
            {
                int y = highestActual <= 0 ? 0 : (int)((double)exposure[i] / highestActual * 255);
                if (y < 0 || y > 255) { throw new Exception(y + " is too dim or too bright!"); }
                canvas[i] = PackColor(y, y, y);
            });

            return this;
        }
    }
}
