using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        private int GetFlowPixelCount()
        {
            if (flowX == null || flowY == null || flowX.Length == 0 || flowX.Length != flowY.Length)
            {
                throw new InvalidOperationException("Flow data is not available. Render and save with buddhabrot-classic-gpu, then load flow.dat.");
            }

            return flowX.Length;
        }

        private static double FlowHue(float x, float y)
        {
            double hue = Math.Atan2(y, x) * 180.0 / Math.PI;
            if (hue < 0)
            {
                hue += 360.0;
            }

            return hue;
        }

        private static double FlowMagnitude(float x, float y)
        {
            return Clamp01(Math.Sqrt(x * x + y * y));
        }

        private double FlowExposureTone(int index, int exposurePeak, double fallback)
        {
            if (exposure == null || exposure.Length <= index || exposurePeak <= 0)
            {
                return fallback;
            }

            return ApplyLog1p(Clamp01((double)exposure[index] / exposurePeak), 96.0);
        }

        public Shader FlowDirectionHSV()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            int pixelCount = GetFlowPixelCount();
            EnsureCanvasLength(pixelCount);
            int exposurePeak = exposure != null && exposure.Length >= pixelCount ? GetExposurePeak() : 0;

            _ = Parallel.For(0, pixelCount, i =>
            {
                float x = flowX[i];
                float y = flowY[i];
                double coherence = FlowMagnitude(x, y);
                if (coherence <= 0)
                {
                    canvas[i] = PackColor(0, 0, 0);
                    return;
                }

                double value = FlowExposureTone(i, exposurePeak, coherence);
                canvas[i] = PackHsv(FlowHue(x, y), Clamp01(0.35 + coherence * 0.65), value);
            });

            return this;
        }

        public Shader FlowCoherence()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            int pixelCount = GetFlowPixelCount();
            EnsureCanvasLength(pixelCount);

            _ = Parallel.For(0, pixelCount, i =>
            {
                double coherence = FlowMagnitude(flowX[i], flowY[i]);
                int shade = ClampToByte(ApplyAsinh(coherence, 6.0) * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });

            return this;
        }

        public Shader FlowCurrent()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            int pixelCount = GetFlowPixelCount();
            EnsureCanvasLength(pixelCount);
            int exposurePeak = exposure != null && exposure.Length >= pixelCount ? GetExposurePeak() : 0;

            _ = Parallel.For(0, pixelCount, i =>
            {
                float xComponent = flowX[i];
                float yComponent = flowY[i];
                double coherence = FlowMagnitude(xComponent, yComponent);
                if (coherence <= 0)
                {
                    canvas[i] = PackColor(0, 0, 0);
                    return;
                }

                int x = width > 0 ? i % width : i;
                int y = width > 0 ? i / width : 0;
                double directionX = xComponent / coherence;
                double directionY = yComponent / coherence;
                double along = x * directionX + y * directionY;
                double across = x * -directionY + y * directionX;
                double currentBands = 0.5 + 0.5 * Math.Sin(across * 0.11 + along * 0.025);
                double fineBands = 0.5 + 0.5 * Math.Sin(along * 0.19);
                double texture = Clamp01(0.45 + currentBands * 0.4 + fineBands * 0.15);
                double value = Clamp01(FlowExposureTone(i, exposurePeak, coherence) * (0.45 + coherence * 0.55) * texture);
                double hue = FlowHue(xComponent, yComponent) + Math.Sin(along * 0.035) * 18.0;

                canvas[i] = PackHsv(hue, Clamp01(0.55 + coherence * 0.45), value);
            });

            return this;
        }
    }
}
