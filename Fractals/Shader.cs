using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        private static readonly Color[] DefaultPaletteStops =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(12, 28, 92),
            Color.FromArgb(0, 132, 168),
            Color.FromArgb(255, 196, 64),
            Color.FromArgb(255, 255, 255)
        };

        private static readonly Color[] FalseColorPaletteStops =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(5, 20, 95),
            Color.FromArgb(0, 125, 145),
            Color.FromArgb(185, 112, 28),
            Color.FromArgb(255, 230, 165),
            Color.FromArgb(255, 255, 255)
        };

        internal int width, height;
        internal int[] exposure, canvas;
        internal int highestActual, highestExposureTarget;
        internal double[] distance;
        internal float[] flowX, flowY;

        private static int PackColor(double r, double g, double b)
        {
            int rr = ClampToByte(r);
            int gg = ClampToByte(g);
            int bb = ClampToByte(b);
            return 255 << 24 | rr << 16 | gg << 8 | bb << 0;
        }

        private static int ClampToByte(double value)
        {
            if (value <= 0) { return 0; }
            if (value >= 255) { return 255; }
            return (int)Math.Round(value);
        }

        private static double Clamp01(double value)
        {
            if (double.IsNaN(value) || value <= 0) { return 0; }
            if (value >= 1) { return 1; }
            return value;
        }

        private static int ClampIndex(int value, int length)
        {
            if (value <= 0) { return 0; }
            int upper = length - 1;
            if (value >= upper) { return upper; }
            return value;
        }

        private static int Red(int pixel) => pixel >> 16 & 255;
        private static int Green(int pixel) => pixel >> 8 & 255;
        private static int Blue(int pixel) => pixel & 255;

        private static double Luma(int pixel)
        {
            return Red(pixel) * 0.2126 + Green(pixel) * 0.7152 + Blue(pixel) * 0.0722;
        }

        private static double ApplyAsinh(double value, double strength)
        {
            value = Clamp01(value);
            if (strength <= 0)
            {
                return value;
            }

            double denominator = Math.Asinh(strength);
            if (denominator == 0)
            {
                return value;
            }

            return Clamp01(Math.Asinh(strength * value) / denominator);
        }

        private static double ApplyLog1p(double value, double strength)
        {
            value = Clamp01(value);
            if (strength <= 0)
            {
                return value;
            }

            double denominator = Math.Log(1 + strength);
            if (denominator == 0)
            {
                return value;
            }

            return Clamp01(Math.Log(1 + strength * value) / denominator);
        }

        private static double ApplySigmoid(double value, double midpoint, double strength)
        {
            value = Clamp01(value);

            double Sigmoid(double x)
            {
                return 1.0 / (1.0 + Math.Exp(-strength * (x - midpoint)));
            }

            double low = Sigmoid(0);
            double high = Sigmoid(1);
            if (high == low)
            {
                return value;
            }

            return Clamp01((Sigmoid(value) - low) / (high - low));
        }

        private static Color InterpolatePalette(Color[] stops, double t)
        {
            if (stops == null || stops.Length == 0)
            {
                return Color.Black;
            }

            if (stops.Length == 1)
            {
                return stops[0];
            }

            t = Clamp01(t);
            double scaled = t * (stops.Length - 1);
            int index = Math.Min(stops.Length - 2, (int)Math.Floor(scaled));
            double localT = scaled - index;
            Color a = stops[index];
            Color b = stops[index + 1];

            int r = ClampToByte(a.R + (b.R - a.R) * localT);
            int g = ClampToByte(a.G + (b.G - a.G) * localT);
            int blue = ClampToByte(a.B + (b.B - a.B) * localT);
            return Color.FromArgb(r, g, blue);
        }

        private static int PackHsv(double hue, double saturation, double value)
        {
            hue %= 360;
            if (hue < 0)
            {
                hue += 360;
            }

            saturation = Clamp01(saturation);
            value = Clamp01(value);

            double c = value * saturation;
            double x = c * (1 - Math.Abs(hue / 60.0 % 2 - 1));
            double m = value - c;
            double r;
            double g;
            double b;

            if (hue < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (hue < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (hue < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (hue < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (hue < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            return PackColor((r + m) * 255, (g + m) * 255, (b + m) * 255);
        }

        private int GetExposurePeak()
        {
            if (exposure == null || exposure.Length == 0)
            {
                highestActual = 0;
                return 0;
            }

            int max = 0;
            for (int i = 0; i < exposure.Length; i++)
            {
                if (exposure[i] > max)
                {
                    max = exposure[i];
                }
            }

            highestActual = max;
            return max;
        }

        private void EnsureCanvasLength(int length)
        {
            if (length <= 0)
            {
                return;
            }

            if (canvas == null || canvas.Length != length)
            {
                canvas = new int[length];
            }
        }

        public Shader ClearCanvas()
        {
            int length = exposure?.Length ?? width * height;
            if (length <= 0)
            {
                return this;
            }

            canvas = new int[length];
            return this;
        }

        private bool CanvasHasContent()
        {
            if (canvas == null)
            {
                return false;
            }

            for (int i = 0; i < canvas.Length; i++)
            {
                if ((canvas[i] & 0x00FFFFFF) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void EnsureCanvasFromExposure()
        {
            if (exposure == null || exposure.Length == 0)
            {
                return;
            }

            EnsureCanvasLength(exposure.Length);
            if (!CanvasHasContent())
            {
                Log1pMapped();
            }
        }

        private double[] GetNormalizedExposure()
        {
            int peak = GetExposurePeak();
            double[] normalized = new double[exposure?.Length ?? 0];
            if (peak <= 0 || normalized.Length == 0)
            {
                return normalized;
            }

            double scale = 1.0 / peak;
            _ = Parallel.For(0, normalized.Length, i =>
            {
                normalized[i] = Clamp01(exposure[i] * scale);
            });

            return normalized;
        }

        private double[] GetNormalizedDistance()
        {
            double[] normalized = new double[distance?.Length ?? 0];
            if (distance == null || distance.Length == 0)
            {
                return normalized;
            }

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < distance.Length; i++)
            {
                double value = distance[i];
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    continue;
                }

                if (value < min)
                {
                    min = value;
                }
                if (value > max)
                {
                    max = value;
                }
            }

            if (min == double.MaxValue || max <= min)
            {
                return normalized;
            }

            double scale = 1.0 / (max - min);
            _ = Parallel.For(0, distance.Length, i =>
            {
                double value = distance[i];
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    normalized[i] = 0;
                    return;
                }

                normalized[i] = Clamp01((value - min) * scale);
            });

            return normalized;
        }

        private bool TryGetDistanceRange(out double min, out double max)
        {
            double globalMin = double.MaxValue;
            double globalMax = double.MinValue;
            if (distance == null || distance.Length == 0)
            {
                min = globalMin;
                max = globalMax;
                return false;
            }

            object minMaxLock = new object();
            _ = Parallel.For(
                0,
                distance.Length,
                () => (Min: double.MaxValue, Max: double.MinValue),
                (i, _, local) =>
                {
                    double value = distance[i];
                    if (double.IsNaN(value) || double.IsInfinity(value))
                    {
                        return local;
                    }

                    if (value < local.Min)
                    {
                        local.Min = value;
                    }

                    if (value > local.Max)
                    {
                        local.Max = value;
                    }

                    return local;
                },
                local =>
                {
                    if (local.Min > local.Max)
                    {
                        return;
                    }

                    lock (minMaxLock)
                    {
                        if (local.Min < globalMin)
                        {
                            globalMin = local.Min;
                        }

                        if (local.Max > globalMax)
                        {
                            globalMax = local.Max;
                        }
                    }
                });

            min = globalMin;
            max = globalMax;
            return min != double.MaxValue && max > min;
        }

        private static double NormalizeDistanceValue(double value, double min, double max)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || max <= min)
            {
                return 0;
            }

            return Clamp01((value - min) / (max - min));
        }

        private void WriteGrayscale(double[] values)
        {
            EnsureCanvasLength(values.Length);
            _ = Parallel.For(0, values.Length, i =>
            {
                int shade = ClampToByte(Clamp01(values[i]) * 255);
                canvas[i] = PackColor(shade, shade, shade);
            });
        }

        private static double[] BuildGaussianKernel(int radius)
        {
            if (radius <= 0)
            {
                return new[] { 1.0 };
            }

            int size = radius * 2 + 1;
            double sigma = Math.Max(0.5, radius / 2.0);
            double denominator = 2 * sigma * sigma;
            double[] kernel = new double[size];
            double sum = 0;

            for (int i = 0; i < size; i++)
            {
                int offset = i - radius;
                double weight = Math.Exp(-(offset * offset) / denominator);
                kernel[i] = weight;
                sum += weight;
            }

            for (int i = 0; i < size; i++)
            {
                kernel[i] /= sum;
            }

            return kernel;
        }

        private int[] BlurCanvas(int[] source, int radius)
        {
            if (source == null || source.Length == 0 || radius <= 0 || width <= 0 || height <= 0)
            {
                return source == null ? Array.Empty<int>() : (int[])source.Clone();
            }

            double[] kernel = BuildGaussianKernel(radius);
            int[] horizontal = new int[source.Length];
            int[] blurred = new int[source.Length];

            _ = Parallel.For(0, height, y =>
            {
                int rowStart = y * width;
                for (int x = 0; x < width; x++)
                {
                    double r = 0;
                    double g = 0;
                    double b = 0;

                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleX = ClampIndex(x + k, width);
                        int pixel = source[rowStart + sampleX];
                        double weight = kernel[k + radius];

                        r += Red(pixel) * weight;
                        g += Green(pixel) * weight;
                        b += Blue(pixel) * weight;
                    }

                    horizontal[rowStart + x] = PackColor(r, g, b);
                }
            });

            _ = Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double r = 0;
                    double g = 0;
                    double b = 0;

                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleY = ClampIndex(y + k, height);
                        int pixel = horizontal[sampleY * width + x];
                        double weight = kernel[k + radius];

                        r += Red(pixel) * weight;
                        g += Green(pixel) * weight;
                        b += Blue(pixel) * weight;
                    }

                    blurred[y * width + x] = PackColor(r, g, b);
                }
            });

            return blurred;
        }

        private double[] BlurValues(double[] source, int radius)
        {
            if (source == null || source.Length == 0 || radius <= 0 || width <= 0 || height <= 0)
            {
                return source == null ? Array.Empty<double>() : (double[])source.Clone();
            }

            double[] kernel = BuildGaussianKernel(radius);
            double[] horizontal = new double[source.Length];
            double[] blurred = new double[source.Length];

            _ = Parallel.For(0, height, y =>
            {
                int rowStart = y * width;
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleX = ClampIndex(x + k, width);
                        sum += source[rowStart + sampleX] * kernel[k + radius];
                    }

                    horizontal[rowStart + x] = sum;
                }
            });

            _ = Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double sum = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleY = ClampIndex(y + k, height);
                        sum += horizontal[sampleY * width + x] * kernel[k + radius];
                    }

                    blurred[y * width + x] = sum;
                }
            });

            return blurred;
        }

        private double[] GetCanvasLumaNormalized()
        {
            EnsureCanvasFromExposure();
            double[] values = new double[canvas?.Length ?? 0];
            if (canvas == null)
            {
                return values;
            }

            _ = Parallel.For(0, canvas.Length, i =>
            {
                values[i] = Luma(canvas[i]) / 255.0;
            });

            return values;
        }
    }
}
