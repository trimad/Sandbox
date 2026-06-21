using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader ExposureDistanceHSV()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || distance == null || distance.Length != exposure.Length)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();
            bool hasDistanceRange = TryGetDistanceRange(out double distanceMin, out double distanceMax);

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double value = ApplyLog1p(normalizedExposure, 64.0);
                double normalizedDistance = hasDistanceRange
                    ? NormalizeDistanceValue(distance[i], distanceMin, distanceMax)
                    : 0;

                double hue = normalizedExposure * 360.0;
                double saturation = Clamp01(0.6 + normalizedDistance * 0.4);
                canvas[i] = PackHsv(hue, saturation, value);
            });

            return this;
        }

        public Shader DistanceExposureContour()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || distance == null || distance.Length != exposure.Length)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();
            bool hasDistanceRange = TryGetDistanceRange(out double distanceMin, out double distanceMax);

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double intensity = ApplyAsinh(normalizedExposure, 8.0);
                double normalizedDistance = hasDistanceRange
                    ? NormalizeDistanceValue(distance[i], distanceMin, distanceMax)
                    : 0;

                int bands = 10;
                double contour = Math.Floor(normalizedDistance * bands) / Math.Max(1, bands - 1);
                double hue = contour * 360.0;
                double value = Clamp01(0.15 + intensity * 0.85);
                canvas[i] = PackHsv(hue, 0.9, value);
            });

            return this;
        }

        public Shader DistanceExposurePalette()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || distance == null || distance.Length != exposure.Length)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            int exposurePeak = GetExposurePeak();
            bool hasDistanceRange = TryGetDistanceRange(out double distanceMin, out double distanceMax);

            _ = Parallel.For(0, exposure.Length, i =>
            {
                double normalizedExposure = exposurePeak <= 0 ? 0 : Clamp01((double)exposure[i] / exposurePeak);
                double exposureTone = ApplyLog1p(normalizedExposure, 96.0);
                double normalizedDistance = hasDistanceRange
                    ? NormalizeDistanceValue(distance[i], distanceMin, distanceMax)
                    : exposureTone;

                double palettePosition = Clamp01(normalizedDistance * 0.65 + exposureTone * 0.35);
                Color color = InterpolatePalette(DefaultPaletteStops, palettePosition);
                double brightness = Clamp01(0.2 + exposureTone * 0.8);

                canvas[i] = PackColor(
                    color.R * brightness,
                    color.G * brightness,
                    color.B * brightness);
            });

            return this;
        }

        public Shader DistanceExposureRelief()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (exposure == null || exposure.Length == 0 || distance == null || distance.Length != exposure.Length || width <= 0 || height <= 0)
            {
                return this;
            }

            EnsureCanvasLength(exposure.Length);
            double[] normalizedDistance = GetNormalizedDistance();
            double[] normalizedExposure = GetNormalizedExposure();
            double lightX = 0.5;
            double lightY = 0.5;
            double lightZ = 1.0;
            double lightLength = Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
            lightX /= lightLength;
            lightY /= lightLength;
            lightZ /= lightLength;

            _ = Parallel.For(0, exposure.Length, i =>
            {
                int x = i % width;
                int y = i / width;
                double dzdx;
                double dzdy;

                if (x > 0 && x < width - 1)
                {
                    dzdx = (normalizedDistance[i + 1] - normalizedDistance[i - 1]) * 0.5;
                }
                else if (x > 0)
                {
                    dzdx = normalizedDistance[i] - normalizedDistance[i - 1];
                }
                else if (x < width - 1)
                {
                    dzdx = normalizedDistance[i + 1] - normalizedDistance[i];
                }
                else
                {
                    dzdx = 0;
                }

                if (y > 0 && y < height - 1)
                {
                    dzdy = (normalizedDistance[i + width] - normalizedDistance[i - width]) * 0.5;
                }
                else if (y > 0)
                {
                    dzdy = normalizedDistance[i] - normalizedDistance[i - width];
                }
                else if (y < height - 1)
                {
                    dzdy = normalizedDistance[i + width] - normalizedDistance[i];
                }
                else
                {
                    dzdy = 0;
                }

                double nx = -dzdx;
                double ny = -dzdy;
                double nz = 1.0;
                double length = Math.Sqrt(nx * nx + ny * ny + nz * nz);
                if (length == 0)
                {
                    length = 1;
                }

                double dot = Clamp01((nx * lightX + ny * lightY + nz * lightZ) / length);
                double exposureStrength = Clamp01(normalizedExposure[i]);
                double ambient = 0.2;
                double diffuse = Clamp01(ambient + dot * 0.8);
                double specular = Math.Pow(exposureStrength, 3.0) * 0.35;
                double brightness = Clamp01(diffuse + specular);
                double baseHue = normalizedDistance[i] * 240.0;
                double saturation = 0.8;
                double value = Clamp01(0.15 + exposureStrength * 0.75);
                int reliefColor = PackHsv(baseHue, saturation, value);

                canvas[i] = PackColor(
                    Red(reliefColor) * brightness,
                    Green(reliefColor) * brightness,
                    Blue(reliefColor) * brightness);
            });

            return this;
        }
    }
}
