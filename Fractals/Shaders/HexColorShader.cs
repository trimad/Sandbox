using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader HexColor()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            _ = Parallel.For(0, exposure.Length, i =>
            {
                uint value = (uint)exposure[i];
                byte r = (byte)(value >> 16);
                byte g = (byte)(value >> 8);
                byte b = (byte)value;
                canvas[i] = PackColor(r, g, b);
            });

            return this;
        }
    }
}
