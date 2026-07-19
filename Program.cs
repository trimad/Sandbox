//https://www.youtube.com/watch?v=8RcjQPbvvRU
//https://www.youtube.com/watch?v=8RcjQPbvvRU
//https://www.youtube.com/watch?v=8RcjQPbvvRU

using Sandbox.Fractals;
using System;
using System.Globalization;

namespace Sandbox
{
    class Program
    {
        private const int DefaultWidth = 32768;
        private const int DefaultHeight = 32768;
        private const int DefaultHighest = 50;
        private const int DefaultCoverage = 50;
        private const int DefaultCutoff = 50;
        private const int DefaultBailout = 2000;
        private const double DefaultMinReal = -2.0;
        private const double DefaultMaxReal = 2.0;
        private const double DefaultMinImaginary = -2.0;
        private const double DefaultMaxImaginary = 2.0;

        private static readonly string[] AllShaderKeywords =
        {
            "distance-binned",
            "distance-hsv",
            "distance-mapped",
            "exponential",
            "distance-log1p-mapped",
            "distance-asinh-stretch",
            "distance-gamma",
            "distance-percentile-mapped",
            "distance-palette-mapped",
            "distance-contour-mapped",
            "distance-sobel-edges",
            "distance-emboss-light",
            "gamma",
            "asinh-stretch",
            "histogram-equalized",
            "clahe",
            "black-point",
            "screen-stretch",
            "percentile-mapped",
            "palette-mapped",
            "false-color-palette",
            "distance-exposure-hsv",
            "exposure-distance-hsv",
            "distance-exposure-contour",
            "distance-exposure-palette",
            "distance-exposure-relief",
            "flow-direction-hsv",
            "flow-coherence",
            "flow-current",
            "contour-mapped",
            "sigmoid-contrast",
            "log1p-mapped",
            "background-gradient-removal",
            "local-contrast",
            "gaussian-blur",
            "bloom",
            "star-reduction",
            "star-mask",
            "denoise-median",
            "bilateral-smooth",
            "deconvolution-sharpen",
            "background-neutralize",
            "saturation-boost",
            "curves",
            "sobel-edges",
            "emboss-light",
            "unsharp-mask",
            "exposure-binned",
            "exposure-hsv",
            "hex-color",
            "log-base-highest",
            "mapped",
            "smooth-step"
        };

        private static void ApplyShaderKeyword(Fractal fractal, string shader)
        {
            switch (shader)
            {
                case "distance-binned":
                    fractal.DistanceBinned();
                    break;
                case "distance-hsv":
                    fractal.DistanceHSV();
                    break;
                case "distance-mapped":
                    fractal.DistanceMapped();
                    break;
                case "exponential":
                    fractal.Exponential(2);
                    break;
                case "distance-log1p-mapped":
                    fractal.DistanceLog1pMapped();
                    break;
                case "distance-asinh-stretch":
                    fractal.DistanceAsinhStretch();
                    break;
                case "distance-gamma":
                    fractal.DistanceGamma();
                    break;
                case "distance-percentile-mapped":
                    fractal.DistancePercentileMapped();
                    break;
                case "distance-palette-mapped":
                    fractal.DistancePaletteMapped();
                    break;
                case "distance-contour-mapped":
                    fractal.DistanceContourMapped();
                    break;
                case "distance-sobel-edges":
                    fractal.DistanceSobelEdges();
                    break;
                case "distance-emboss-light":
                    fractal.DistanceEmbossLight();
                    break;
                case "gamma":
                    fractal.Gamma();
                    break;
                case "asinh-stretch":
                    fractal.AsinhStretch();
                    break;
                case "histogram-equalized":
                    fractal.HistogramEqualized();
                    break;
                case "clahe":
                    fractal.Clahe();
                    break;
                case "black-point":
                    fractal.BlackPoint();
                    break;
                case "screen-stretch":
                    fractal.ScreenStretch();
                    break;
                case "percentile-mapped":
                    fractal.PercentileMapped();
                    break;
                case "palette-mapped":
                    fractal.PaletteMapped();
                    break;
                case "false-color-palette":
                    fractal.FalseColorPalette();
                    break;
                case "distance-exposure-hsv":
                    fractal.DistanceExposureHSV();
                    break;
                case "exposure-distance-hsv":
                    fractal.ExposureDistanceHSV();
                    break;
                case "distance-exposure-contour":
                    fractal.DistanceExposureContour();
                    break;
                case "distance-exposure-palette":
                    fractal.DistanceExposurePalette();
                    break;
                case "distance-exposure-relief":
                    fractal.DistanceExposureRelief();
                    break;
                case "flow-direction-hsv":
                    fractal.FlowDirectionHSV();
                    break;
                case "flow-coherence":
                    fractal.FlowCoherence();
                    break;
                case "flow-current":
                    fractal.FlowCurrent();
                    break;
                case "contour-mapped":
                    fractal.ContourMapped();
                    break;
                case "sigmoid-contrast":
                    fractal.SigmoidContrast();
                    break;
                case "log1p-mapped":
                    fractal.Log1pMapped();
                    break;
                case "background-gradient-removal":
                    fractal.BackgroundGradientRemoval();
                    break;
                case "local-contrast":
                    fractal.LocalContrast();
                    break;
                case "gaussian-blur":
                    fractal.GaussianBlur();
                    break;
                case "bloom":
                    fractal.Bloom();
                    break;
                case "star-reduction":
                    fractal.StarReduction();
                    break;
                case "star-mask":
                    fractal.StarMask();
                    break;
                case "denoise-median":
                    fractal.DenoiseMedian();
                    break;
                case "bilateral-smooth":
                    fractal.BilateralSmooth();
                    break;
                case "deconvolution-sharpen":
                    fractal.DeconvolutionSharpen();
                    break;
                case "background-neutralize":
                    fractal.BackgroundNeutralize();
                    break;
                case "saturation-boost":
                    fractal.SaturationBoost();
                    break;
                case "curves":
                    fractal.Curves();
                    break;
                case "sobel-edges":
                    fractal.SobelEdges();
                    break;
                case "emboss-light":
                    fractal.EmbossLight();
                    break;
                case "unsharp-mask":
                    fractal.UnsharpMask();
                    break;
                case "exposure-binned":
                    fractal.ExposureBinned();
                    break;
                case "exposure-hsv":
                    fractal.ExposureHSV();
                    break;
                case "hex-color":
                    fractal.HexColor();
                    break;
                case "log-base-highest":
                    fractal.LogBaseHighest();
                    break;
                case "mapped":
                    fractal.Mapped();
                    break;
                case "smooth-step":
                    fractal.SmoothStep();
                    break;
                default:
                    throw new ArgumentException($"Unknown shader keyword: {shader}");
            }
        }

        private static string GetShaderDataSource(string shader)
        {
            switch (shader)
            {
                case "distance-binned":
                case "distance-hsv":
                case "distance-mapped":
                case "exponential":
                case "distance-log1p-mapped":
                case "distance-asinh-stretch":
                case "distance-gamma":
                case "distance-percentile-mapped":
                case "distance-palette-mapped":
                case "distance-contour-mapped":
                case "distance-sobel-edges":
                case "distance-emboss-light":
                    return "distance.dat";
                case "distance-exposure-hsv":
                case "exposure-distance-hsv":
                case "distance-exposure-contour":
                case "distance-exposure-palette":
                case "distance-exposure-relief":
                case "false-color-palette":
                    return "distance.dat+exposure.dat";
                case "flow-direction-hsv":
                case "flow-coherence":
                case "flow-current":
                    return "flow.dat";
                case "background-gradient-removal":
                case "local-contrast":
                case "gaussian-blur":
                case "bloom":
                case "star-reduction":
                case "star-mask":
                case "denoise-median":
                case "bilateral-smooth":
                case "deconvolution-sharpen":
                case "background-neutralize":
                case "saturation-boost":
                case "curves":
                case "sobel-edges":
                case "emboss-light":
                case "unsharp-mask":
                    return "canvas";
                default:
                    return "exposure.dat";
            }
        }

        private static string GetShaderDescription(string shader)
        {
            switch (shader)
            {
                case "distance-binned": return "Fixed-range distance grayscale bins";
                case "distance-hsv": return "Distance mapped to hue";
                case "distance-mapped": return "Linear distance grayscale";
                case "exponential": return "Distance exponential stretch";
                case "distance-log1p-mapped": return "Log1p distance grayscale";
                case "distance-asinh-stretch": return "Asinh distance stretch";
                case "distance-gamma": return "Gamma corrected distance grayscale";
                case "distance-percentile-mapped": return "Percentile clipped distance grayscale";
                case "distance-palette-mapped": return "Color palette distance map";
                case "distance-contour-mapped": return "Quantized distance contours";
                case "distance-sobel-edges": return "Sobel edges from distance field";
                case "distance-emboss-light": return "Embossed distance height map";
                case "gamma": return "Gamma corrected exposure grayscale";
                case "asinh-stretch": return "Asinh exposure stretch";
                case "histogram-equalized": return "Global histogram equalization";
                case "clahe": return "Adaptive histogram equalization";
                case "black-point": return "Subtract a dark background floor";
                case "screen-stretch": return "Astrophotography preview stretch";
                case "percentile-mapped": return "Percentile clipped exposure grayscale";
                case "palette-mapped": return "Color palette exposure map";
                case "false-color-palette": return "False color from exposure and distance";
                case "distance-exposure-hsv": return "Distance hue, exposure value";
                case "exposure-distance-hsv": return "Exposure hue, distance saturation";
                case "distance-exposure-contour": return "Distance contours with exposure brightness";
                case "distance-exposure-palette": return "Exposure-driven palette tinted by distance";
                case "distance-exposure-relief": return "Relief lighting from distance and exposure";
                case "flow-direction-hsv": return "Average orbit segment direction as HSV";
                case "flow-coherence": return "Flow vector coherence grayscale";
                case "flow-current": return "Flow direction with current-like bands";
                case "contour-mapped": return "Quantized exposure contours";
                case "sigmoid-contrast": return "Sigmoid exposure contrast";
                case "log1p-mapped": return "Log1p exposure grayscale";
                case "background-gradient-removal": return "Subtract low-frequency gradients";
                case "local-contrast": return "Boost broad local contrast";
                case "gaussian-blur": return "Gaussian blur current canvas";
                case "bloom": return "Glow bright regions";
                case "star-reduction": return "Reduce compact bright peaks";
                case "star-mask": return "Mask compact bright peaks";
                case "denoise-median": return "Median denoise hot pixels";
                case "bilateral-smooth": return "Edge-preserving smoothing";
                case "deconvolution-sharpen": return "Small Richardson-Lucy style sharpen";
                case "background-neutralize": return "Neutralize dark background color casts";
                case "saturation-boost": return "Boost color saturation";
                case "curves": return "S-curve contrast";
                case "sobel-edges": return "Sobel edges from current canvas";
                case "emboss-light": return "Emboss current canvas";
                case "unsharp-mask": return "Sharpen current canvas";
                case "exposure-binned": return "Rank-order exposure grayscale bins";
                case "exposure-hsv": return "Exposure mapped to hue";
                case "hex-color": return "Exposure bits as RGB";
                case "log-base-highest": return "Log exposure grayscale";
                case "mapped": return "Linear exposure grayscale";
                case "smooth-step": return "Smooth-step exposure grayscale";
                default: return string.Empty;
            }
        }

        private static string GetShaderOutputName(string shader)
        {
            string sourcePrefix = GetShaderDataSource(shader)
                .Replace(".dat", string.Empty)
                .Replace("+", "-");

            if (shader.StartsWith(sourcePrefix + "-", StringComparison.OrdinalIgnoreCase))
            {
                return shader;
            }

            return $"{sourcePrefix}-{shader}";
        }

        private static void SaveAllShaderOutputs(Fractal fractal)
        {
            foreach (string shader in AllShaderKeywords)
            {
                Console.WriteLine($"Saving shader comparison: {shader} [{GetShaderDataSource(shader)}]");
                fractal.ClearCanvas();

                try
                {
                    ApplyShaderKeyword(fractal, shader);
                    fractal.SaveImage(GetShaderOutputName(shader));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{shader} failed: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        private static bool TryRender(Fractal fractal)
        {
            try
            {
                fractal.Render();
                return true;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private sealed class RenderSettings
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public double MinReal { get; private set; }
            public double MaxReal { get; private set; }
            public double MinImaginary { get; private set; }
            public double MaxImaginary { get; private set; }
            public int Highest { get; private set; }
            public int Coverage { get; private set; }
            public int ExposureThreshold { get; private set; }
            public int Cutoff { get; private set; }
            public int Bailout { get; private set; }
            public int GpuProgressInterval { get; private set; }
            public bool HasErrors { get; private set; }

            private RenderSettings()
            {
                Width = GetPositiveIntEnvironmentVariable("SANDBOX_WIDTH", DefaultWidth);
                Height = GetPositiveIntEnvironmentVariable("SANDBOX_HEIGHT", DefaultHeight);
                MinReal = GetDoubleEnvironmentVariable("SANDBOX_MIN_REAL", DefaultMinReal);
                MaxReal = GetDoubleEnvironmentVariable("SANDBOX_MAX_REAL", DefaultMaxReal);
                MinImaginary = GetDoubleEnvironmentVariable("SANDBOX_MIN_IMAGINARY", DefaultMinImaginary);
                MaxImaginary = GetDoubleEnvironmentVariable("SANDBOX_MAX_IMAGINARY", DefaultMaxImaginary);
                Highest = GetPositiveIntEnvironmentVariable("SANDBOX_HIGHEST", DefaultHighest);
                Coverage = GetPositiveIntEnvironmentVariable("SANDBOX_COVERAGE", DefaultCoverage);
                ExposureThreshold = GetNonNegativeIntEnvironmentVariable("SANDBOX_EXPOSURE_THRESHOLD", 0);
                Cutoff = GetNonNegativeIntEnvironmentVariable("SANDBOX_CUTOFF", DefaultCutoff);
                Bailout = GetPositiveIntEnvironmentVariable("SANDBOX_BAILOUT", DefaultBailout);
                GpuProgressInterval = GetPositiveIntEnvironmentVariable("SANDBOX_GPU_PROGRESS_INTERVAL", BuddhabrotGPU.DefaultProgressCheckBatchInterval);
            }

            public static RenderSettings FromArgs(string[] args)
            {
                RenderSettings settings = new RenderSettings();

                foreach (string arg in args)
                {
                    settings.TryApply(arg);
                }

                return settings;
            }

            public bool IsSetting(string arg)
            {
                return TrySplitSetting(arg, out _, out _);
            }

            private bool TryApply(string arg)
            {
                if (!TrySplitSetting(arg, out string key, out string value))
                {
                    return false;
                }

                if (key == "bailout" || key == "iterations")
                {
                    if (value.Equals("max", StringComparison.OrdinalIgnoreCase))
                    {
                        Bailout = int.MaxValue;
                        return true;
                    }
                }

                switch (key)
                {
                    case "width":
                    case "w":
                        return TryApplyPositiveInt(key, value, parsed => Width = parsed);
                    case "height":
                    case "h":
                        return TryApplyPositiveInt(key, value, parsed => Height = parsed);
                    case "highest":
                    case "target":
                    case "exposure-target":
                        return TryApplyPositiveInt(key, value, parsed => Highest = parsed);
                    case "coverage":
                    case "coverage-target":
                    case "coverage-threshold":
                        return TryApplyPositiveInt(key, value, parsed => Coverage = parsed);
                    case "exposure-threshold":
                    case "threshold-exposure":
                        return TryApplyNonNegativeInt(key, value, parsed => ExposureThreshold = parsed);
                    case "cutoff":
                        return TryApplyNonNegativeInt(key, value, parsed => Cutoff = parsed);
                    case "bailout":
                    case "iterations":
                        return TryApplyPositiveInt(key, value, parsed => Bailout = parsed);
                    case "gpu-progress-interval":
                    case "gpu-progress":
                        return TryApplyPositiveInt(key, value, parsed => GpuProgressInterval = parsed);
                    case "minreal":
                    case "min-re":
                    case "minre":
                    case "min-real":
                        return TryApplyDouble(key, value, parsed => MinReal = parsed);
                    case "maxreal":
                    case "max-re":
                    case "maxre":
                    case "max-real":
                        return TryApplyDouble(key, value, parsed => MaxReal = parsed);
                    case "minimaginary":
                    case "min-imaginary":
                    case "minimag":
                    case "min-im":
                        return TryApplyDouble(key, value, parsed => MinImaginary = parsed);
                    case "maximaginary":
                    case "max-imaginary":
                    case "maximag":
                    case "max-im":
                        return TryApplyDouble(key, value, parsed => MaxImaginary = parsed);
                }

                string suggestion = key == "heigt" ? " Did you mean height?" : string.Empty;
                Console.WriteLine($"Unknown setting: {arg}.{suggestion}");
                HasErrors = true;
                return true;
            }

            private static bool TrySplitSetting(string arg, out string key, out string value)
            {
                key = string.Empty;
                value = string.Empty;
                int equals = arg.IndexOf('=');
                if (equals <= 0 || equals == arg.Length - 1)
                {
                    return false;
                }

                key = arg.Substring(0, equals).Trim().TrimStart('-', '/').ToLowerInvariant();
                value = arg.Substring(equals + 1).Trim();
                return key.Length > 0 && value.Length > 0;
            }

            private bool TryApplyPositiveInt(string key, string value, Action<int> apply)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) && parsed > 0)
                {
                    apply(parsed);
                    return true;
                }

                Console.WriteLine($"Invalid value for {key}: {value}. Use a positive integer.");
                HasErrors = true;
                return false;
            }

            private bool TryApplyNonNegativeInt(string key, string value, Action<int> apply)
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) && parsed >= 0)
                {
                    apply(parsed);
                    return true;
                }

                Console.WriteLine($"Invalid value for {key}: {value}. Use a non-negative integer.");
                HasErrors = true;
                return false;
            }

            private bool TryApplyDouble(string key, string value, Action<double> apply)
            {
                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double parsed))
                {
                    apply(parsed);
                    return true;
                }

                Console.WriteLine($"Invalid value for {key}: {value}. Use a numeric value.");
                HasErrors = true;
                return false;
            }

            private static double GetDoubleEnvironmentVariable(string name, double defaultValue)
            {
                string value = Environment.GetEnvironmentVariable(name);
                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double parsed))
                {
                    return parsed;
                }

                return defaultValue;
            }

            public override string ToString()
            {
                string bailoutValue = Bailout == int.MaxValue ? "max" : Bailout.ToString(CultureInfo.InvariantCulture);
                return $"width={Width} height={Height} minReal={MinReal.ToString(CultureInfo.InvariantCulture)} maxReal={MaxReal.ToString(CultureInfo.InvariantCulture)} minImaginary={MinImaginary.ToString(CultureInfo.InvariantCulture)} maxImaginary={MaxImaginary.ToString(CultureInfo.InvariantCulture)} highest={Highest} coverage={Coverage} cutoff={Cutoff} bailout={bailoutValue} gpu-progress-interval={GpuProgressInterval}";
            }
        }

        private static int GetPositiveIntEnvironmentVariable(string name, int defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (int.TryParse(value, out int parsed) && parsed > 0)
            {
                return parsed;
            }

            return defaultValue;
        }

        private static int GetNonNegativeIntEnvironmentVariable(string name, int defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (int.TryParse(value, out int parsed) && parsed >= 0)
            {
                return parsed;
            }

            return defaultValue;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ", args));
            Console.WriteLine("Environment Version: {0}", Environment.Version.ToString());

            RenderSettings settings = RenderSettings.FromArgs(args);
            Console.WriteLine($"Render settings: {settings}");
            if (settings.HasErrors)
            {
                Console.WriteLine("One or more settings were invalid. No render was started.");
                return;
            }

            Fractal fractal = new Fractal();
            bool hasFractalData = false;

            foreach (string a in args)
            {
                if (settings.IsSetting(a))
                {
                    continue;
                }

                switch (a)
                {
                    // case "animate":
                    //     int upper = 900;
                    //     for (int i = 0; i < upper; i++)
                    //     {
                    //         double zoom = Helper.Map(i, 0, upper, 1, 5);
                    //         //centered on (-0.54,0.0)
                    //         //fractal = new Glynn(W, H, i,0,0).Init(-0.79 + zoom, -0.29 - zoom, -0.25 + zoom, 0.25 - zoom);//focuses on the top tree
                    //         //fractal = new Glynn(W, H, HIGHEST, zoom, 1).Init(-0.8, 1.4, -1.2, 1.2);//focuses on all the trees
                    //         double x0 = Helper.Map(i, 0, upper, -2, 1.225);
                    //         double x1 = Helper.Map(i, 0, upper, 2, 1.325);
                    //         double y0 = Helper.Map(i, 0, upper, -2, -0.05);
                    //         double y1 = Helper.Map(i, 0, upper, 2, 0.05);

                    //         fractal = new Glynn(W, H, HIGHEST, zoom, 1).Init(x0, x1, y0, y1);
                    //         fractal.Render().DistanceMapped();
                    //         //DistanceHSV();
                    //         fractal.SaveImage(i.ToString("D5"));

                    //     }
                        // break;
                    case "buddhabrot": //fractal
                        fractal = new Buddhabrot(settings.Width, settings.Height, settings.Cutoff, settings.Bailout, settings.Highest)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                            //.Init(-1.9, 1.9, -2.1, 1.7);
                        hasFractalData = false;
                        break;
                    case "buddhabrot-gpu": //fractal
                        fractal = new BuddhabrotGPU(settings.Width, settings.Height, settings.Cutoff, settings.Bailout, settings.Highest, settings.GpuProgressInterval)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "buddhabrot-classic-gpu": //fractal
                        fractal = new BuddhabrotClassicGPU(settings.Width, settings.Height, settings.Cutoff, settings.Bailout, settings.Highest, settings.GpuProgressInterval)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "buddhabrot-primes-gpu": //fractal
                        fractal = new BuddhabrotPrimesGPU(settings.Width, settings.Height, settings.Cutoff, settings.Bailout, settings.Highest, settings.GpuProgressInterval)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "buddhabrot-coverage": //fractal
                        fractal = new BuddhabrotCoverage(settings.Width, settings.Height, settings.Cutoff, settings.Bailout, settings.Coverage)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "glynn": //fractal
                        fractal = new Glynn(settings.Width, settings.Height, settings.Highest, 0, 0)
                        //.Init(0.066, 0.42, -0.6677, -0.323);
                        .Init(1, -1, -1, 1);
                        //.InitPoint(-0.539, 0, 0.2, 0.2);//r, i, width, height
                        hasFractalData = false;
                        break;
                    case "julia": //fractal
                        fractal = new Julia(settings.Width, settings.Height, settings.Highest)
                        //.Init(-0.39, -0.7, 0.19, -0.12);
                        .Init(-0.8, 0.8, -0.8, 0.8);
                        hasFractalData = false;
                        break;
                    case "mandelbrot": //fractal
                        fractal = new Mandelbrot(settings.Width, settings.Height, settings.Bailout, settings.Highest)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "burningship": //fractal
                        fractal = new BurningShip(settings.Width, settings.Height, settings.Bailout, settings.Highest)
                            .Init(-1.8, 1.8, -0.2, 1.8);
                        hasFractalData = false;
                        break;
                    case "tricorn": //fractal
                        fractal = new Tricorn(settings.Width, settings.Height, settings.Highest)
                            .Init(-1.5, 1.5, -1.5, 1.5);
                        hasFractalData = false;
                        break;
                    case "multibrot3": //fractal
                        fractal = new Multibrot(settings.Width, settings.Height, settings.Highest, 3)
                            .Init(-1.2, 1.2, -1.2, 1.2);
                        hasFractalData = false;
                        break;
                    case "newton": //fractal
                        fractal = new NewtonFractal(settings.Width, settings.Height, settings.Highest, 3)
                            .Init(-1.5, 1.5, -1.5, 1.5);
                        hasFractalData = false;
                        break;
                    case "celtic": //fractal
                        fractal = new CelticMandelbrot(settings.Width, settings.Height, settings.Bailout, settings.Highest)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "magnet1": //fractal
                        fractal = new MagnetType1(settings.Width, settings.Height, settings.Bailout, settings.Highest)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "magnet2": //fractal
                        fractal = new MagnetType2(settings.Width, settings.Height, settings.Bailout, settings.Highest)
                            .Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary);
                        hasFractalData = false;
                        break;
                    case "load":
                        fractal.LoadSettings("settings.json").LoadDistance("distance.dat").LoadExposure("exposure.dat").TryLoadFlow("flow.dat");

                        if (settings.ExposureThreshold > 0)
                        {
                            fractal.ThresholdExposure(settings.ExposureThreshold);
                        }

                        hasFractalData = true;
                        break;
                    case "logisticmap":
                        fractal = new LogisticMap(settings.Width, settings.Height, settings.Highest)
                            .Init(0, 0, 0, 0);
                        hasFractalData = false;
                        break;
                    case "render":
                        if (!TryRender(fractal))
                        {
                            return;
                        }

                        if (settings.ExposureThreshold > 0)
                        {
                            fractal.ThresholdExposure(settings.ExposureThreshold);
                        }

                        hasFractalData = true;
                        break;
                    case "all":
                        if (!hasFractalData)
                        {
                            if (!TryRender(fractal))
                            {
                                return;
                            }

                            hasFractalData = true;
                        }

                        SaveAllShaderOutputs(fractal);
                        break;
                    case "distance-binned": //shade
                        fractal.DistanceBinned();
                        break;
                    case "distance-hsv": //shade
                        fractal.DistanceHSV();
                        break;
                    case "distance-mapped": //shade
                        fractal.DistanceMapped();
                        break;
                    case "exponential": //shade
                        fractal.Exponential(2);
                        break;
                    case "distance-log1p-mapped": //shade
                        fractal.DistanceLog1pMapped();
                        break;
                    case "distance-asinh-stretch": //shade
                        fractal.DistanceAsinhStretch();
                        break;
                    case "distance-gamma": //shade
                        fractal.DistanceGamma();
                        break;
                    case "distance-percentile-mapped": //shade
                        fractal.DistancePercentileMapped();
                        break;
                    case "distance-palette-mapped": //shade
                        fractal.DistancePaletteMapped();
                        break;
                    case "distance-contour-mapped": //shade
                        fractal.DistanceContourMapped();
                        break;
                    case "distance-sobel-edges": //shade
                        fractal.DistanceSobelEdges();
                        break;
                    case "distance-emboss-light": //shade
                        fractal.DistanceEmbossLight();
                        break;
                    case "gamma": //shade
                        fractal.Gamma();
                        break;
                    case "asinh-stretch": //shade
                        fractal.AsinhStretch();
                        break;
                    case "histogram-equalized": //shade
                        fractal.HistogramEqualized();
                        break;
                    case "clahe": //shade
                        fractal.Clahe();
                        break;
                    case "black-point": //shade
                        fractal.BlackPoint();
                        break;
                    case "screen-stretch": //shade
                        fractal.ScreenStretch();
                        break;
                    case "percentile-mapped": //shade
                        fractal.PercentileMapped();
                        break;
                    case "palette-mapped": //shade
                        fractal.PaletteMapped();
                        break;
                    case "false-color-palette": //shade
                        fractal.FalseColorPalette();
                        break;
                    case "distance-exposure-hsv": //shade
                        fractal.DistanceExposureHSV();
                        break;
                    case "flow-direction-hsv": //shade
                        fractal.FlowDirectionHSV();
                        break;
                    case "flow-coherence": //shade
                        fractal.FlowCoherence();
                        break;
                    case "flow-current": //shade
                        fractal.FlowCurrent();
                        break;
                    case "contour-mapped": //shade
                        fractal.ContourMapped();
                        break;
                    case "sigmoid-contrast": //shade
                        fractal.SigmoidContrast();
                        break;
                    case "log1p-mapped": //shade
                        fractal.Log1pMapped();
                        break;
                    case "background-gradient-removal": //shade
                        fractal.BackgroundGradientRemoval();
                        break;
                    case "local-contrast": //shade
                        fractal.LocalContrast();
                        break;
                    case "gaussian-blur": //shade
                        fractal.GaussianBlur();
                        break;
                    case "bloom": //shade
                        fractal.Bloom();
                        break;
                    case "star-reduction": //shade
                        fractal.StarReduction();
                        break;
                    case "star-mask": //shade
                        fractal.StarMask();
                        break;
                    case "denoise-median": //shade
                        fractal.DenoiseMedian();
                        break;
                    case "bilateral-smooth": //shade
                        fractal.BilateralSmooth();
                        break;
                    case "deconvolution-sharpen": //shade
                        fractal.DeconvolutionSharpen();
                        break;
                    case "background-neutralize": //shade
                        fractal.BackgroundNeutralize();
                        break;
                    case "saturation-boost": //shade
                        fractal.SaturationBoost();
                        break;
                    case "curves": //shade
                        fractal.Curves();
                        break;
                    case "sobel-edges": //shade
                        fractal.SobelEdges();
                        break;
                    case "emboss-light": //shade
                        fractal.EmbossLight();
                        break;
                    case "unsharp-mask": //shade
                        fractal.UnsharpMask();
                        break;
                    case "exposure-binned": //shade
                        fractal.ExposureBinned();
                        break;
                    case "exposure-hsv": //shade
                        fractal.ExposureHSV();
                        break;
                    case "hex-color": //shade
                        fractal.HexColor();
                        break;
                    case "log-base-highest": //shade
                        fractal.LogBaseHighest();
                        break;
                    case "mapped": //shade
                        fractal.Mapped();
                        break;
                    case "smooth-step": //shade
                        fractal.SmoothStep();
                        break;
                    case "save":
                        fractal.SaveSettings("settings.json", new Settings
                        {
                            Name = fractal.name,
                            AspectRatio = settings.Width > 0 && settings.Height > 0 ? (double)settings.Width / settings.Height : fractal.aspectRatio,
                            Height = settings.Height,
                            Width = settings.Width,
                            Highest = fractal.highestActual,
                            HighestTarget = settings.Highest,
                            Cutoff = settings.Cutoff,
                            Bailout = settings.Bailout,
                            MinReal = settings.MinReal,
                            MaxReal = settings.MaxReal,
                            MinImaginary = settings.MinImaginary,
                            MaxImaginary = settings.MaxImaginary,
                            GpuProgressInterval = settings.GpuProgressInterval,
                            Time = fractal.TimeStamp
                        }).SaveDistance("distance.dat").SaveExposure("exposure.dat").SaveFlow("flow.dat");
                        break;
                    case "draw":
                        fractal.Draw();
                        break;
                    case "/?":
                        Console.WriteLine("Options:");
                        Console.WriteLine(String.Format("{0,-24} {1}", "/?", "Display this help message"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "buddhabrot", "Render the Buddhabrot fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "buddhabrot-gpu", "Render the Buddhabrot fractal with ILGPU/CUDA"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "buddhabrot-classic-gpu", "Render the classic Buddhabrot fractal with ILGPU/CUDA bailout control"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "buddhabrot-primes-gpu", "Render the Buddhabrot fractal using only prime-length escape orbits up to 1613"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "buddhabrot-coverage", "Render the Buddhabrot fractal until a percentage of pixels receive at least one hit"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "glynn", "Render the Glynn fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "julia", "Render the Julia fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "mandelbrot", "Render the Mandelbrot fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "burningship", "Render the Burning Ship fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "tricorn", "Render the Tricorn fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "multibrot3", "Render the Multibrot 3 fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "newton", "Render the Newton fractal (z^3-1)"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "celtic", "Render the Celtic Mandelbrot fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "magnet1", "Render the Magnet Type I rational-map fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "magnet2", "Render the Magnet Type II rational-map fractal"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "logisticmap", "Render the logistic map"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "render/load/save/draw", "Core pipeline commands"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "all", "Render or load once, then save every shader output"));
                        Console.WriteLine("Settings:");
                        Console.WriteLine(String.Format("{0,-24} {1}", "width=32768", "Render width; alias w=32768"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "height=32768", "Render height; alias h=32768"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "highest=50", "Target peak exposure before render stops"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "coverage=50", "Target percent of pixels with at least one hit for buddhabrot-coverage"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "cutoff=50", "Skip the first N orbit iterations; cutoff=0 plots immediately"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "bailout=2000", "Maximum orbit iterations; use bailout=max for no limit"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "exposure-threshold=10", "Zero exposure values below this threshold before shading"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "min-real=-2", "Minimum real coordinate"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "max-real=2", "Maximum real coordinate"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "min-imaginary=-2", "Minimum imaginary coordinate"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "max-imaginary=2", "Maximum imaginary coordinate"));
                        Console.WriteLine(String.Format("{0,-24} {1}", "gpu-progress-interval=8", "GPU batches queued before progress check"));
                        Console.WriteLine("Shaders:");
                        foreach (string shader in AllShaderKeywords)
                        {
                            Console.WriteLine(String.Format("{0,-32} {1,-27} {2}", shader, $"[{GetShaderDataSource(shader)}]", GetShaderDescription(shader)));
                        }
                        break;
                }
            }
        }
    }
}
