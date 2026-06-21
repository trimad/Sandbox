using System;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader GaussianBlur(int radius = 3)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            EnsureCanvasFromExposure();
            if (canvas == null || canvas.Length == 0)
            {
                return this;
            }

            canvas = BlurCanvas(canvas, radius);
            return this;
        }
    }
}
