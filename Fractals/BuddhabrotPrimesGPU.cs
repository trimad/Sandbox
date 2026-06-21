using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace Sandbox.Fractals
{
    public class BuddhabrotPrimesGPU : Fractal
    {
        public const int DefaultProgressCheckBatchInterval = 8;
        private const int MaxPrimeOrbit = 3671;

        const int WorkItemsPerBatch = 262144;
        const int SamplesPerWorkItem = 4;

        readonly int cutoff, bailout;
        readonly int progressCheckBatchInterval;
        double realMin, realMax, imaginaryMin, imaginaryMax;
        bool initialized;
        int nextProgressPercent;

        public BuddhabrotPrimesGPU(int _width, int _height, int _cutoff, int _bailout, int _highestExposureTarget, int _progressCheckBatchInterval = DefaultProgressCheckBatchInterval)
        {
            this.name = "Buddhabrot-Primes-GPU";
            this.width = _width;
            this.height = _height;
            this.cutoff = _cutoff;
            this.bailout = _bailout;
            this.highestExposureTarget = _highestExposureTarget;
            this.progressCheckBatchInterval = Math.Max(1, _progressCheckBatchInterval);
        }

        public new Fractal Init(double x0, double x1, double y0, double y1)
        {
            highestActual = 0;
            aspectRatio = (double)width / height;
            realMin = x0 * aspectRatio;
            realMax = x1 * aspectRatio;
            imaginaryMin = y0;
            imaginaryMax = y1;
            initialized = true;
            return this;
        }

        public override Fractal Render()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("BuddhabrotPrimesGPU must be initialized before rendering.");
            }

            int pixelCount = checked(width * height);
            float realMinF = (float)realMin;
            float realRange = (float)(realMax - realMin);
            float imaginaryMinF = (float)imaginaryMin;
            float imaginaryRange = (float)(imaginaryMax - imaginaryMin);
            if (realRange == 0 || imaginaryRange == 0)
            {
                throw new InvalidOperationException("BuddhabrotPrimesGPU needs a non-zero render domain.");
            }

            string gpuName = TryGetNvidiaGpuName();
            if (!string.IsNullOrWhiteSpace(gpuName))
            {
                Console.WriteLine($"Detected NVIDIA GPU: {gpuName}");
            }

            try
            {
                using Context context = Context.Create(builder =>
                {
                    builder.Cuda();
                    builder.EnableAlgorithms();
                });
                using Accelerator accelerator = context.CreateCudaAccelerator(0);
                Console.WriteLine($"Using ILGPU CUDA accelerator: {accelerator.Name}");

                return RenderWithAccelerator(accelerator, pixelCount, realMinF, realRange, imaginaryMinF, imaginaryRange);
            }
            catch (Exception ex) when (IsCudaSetupException(ex))
            {
                string hardwareStatus = string.IsNullOrWhiteSpace(gpuName)
                    ? "No NVIDIA GPU was detected by nvidia-smi."
                    : $"Detected NVIDIA GPU: {gpuName}.";

                throw new NotSupportedException(
                    $"{hardwareStatus} BuddhabrotPrimesGPU could not create an ILGPU CUDA accelerator. " +
                    "Install or repair the NVIDIA CUDA driver runtime, then run 'buddhabrot-primes-gpu render' again.",
                    ex);
            }
        }

        private Fractal RenderWithAccelerator(
            Accelerator accelerator,
            int pixelCount,
            float realMinF,
            float realRange,
            float imaginaryMinF,
            float imaginaryRange)
        {
            nextProgressPercent = 0;
            ReportProgress(0);

            using MemoryBuffer1D<int, Stride1D.Dense> exposureBuffer = accelerator.Allocate1D<int>(pixelCount);
            using MemoryBuffer1D<float, Stride1D.Dense> distanceSumsBuffer = accelerator.Allocate1D<float>(pixelCount);
            using MemoryBuffer1D<int, Stride1D.Dense> highestBuffer = accelerator.Allocate1D<int>(1);
            exposureBuffer.MemSetToZero();
            distanceSumsBuffer.MemSetToZero();
            highestBuffer.MemSetToZero();

            var kernel = KernelLoaders.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView<int>,
                ArrayView<float>,
                ArrayView<int>,
                int,
                int,
                int,
                float,
                float,
                float,
                float,
                int,
                int,
                ulong>(accelerator, BuddhabrotKernel);

            int[] peak = new int[1];
            int batch = 0;
            Console.WriteLine($"GPU batch size: {WorkItemsPerBatch * SamplesPerWorkItem:N0} samples");
            Console.WriteLine($"GPU progress check interval: {progressCheckBatchInterval:N0} batches");

            do
            {
                for (int i = 0; i < progressCheckBatchInterval; i++)
                {
                    ulong seed = GetBatchSeed(batch);
                    kernel(
                        WorkItemsPerBatch,
                        exposureBuffer.View,
                        distanceSumsBuffer.View,
                        highestBuffer.View,
                        SamplesPerWorkItem,
                        width,
                        height,
                        realMinF,
                        realRange,
                        imaginaryMinF,
                        imaginaryRange,
                        cutoff,
                        bailout,
                        seed);
                    batch++;
                }

                accelerator.Synchronize();

                highestBuffer.CopyToCPU(peak);
                highestActual = peak[0];
                ReportProgress(highestActual);
            }
            while (highestActual < highestExposureTarget);

            Console.WriteLine($"GPU render complete after {batch:N0} batches.");
            exposure = exposureBuffer.GetAsArray1D();
            float[] distanceSums = distanceSumsBuffer.GetAsArray1D();
            distance = new double[pixelCount];
            canvas = new int[pixelCount];

            _ = Parallel.For(0, pixelCount, index =>
            {
                int hits = exposure[index];
                distance[index] = hits == 0 ? 0 : distanceSums[index] / hits;
            });

            return this;
        }

        private void ReportProgress(int peak)
        {
            int percentComplete = highestExposureTarget <= 0
                ? 100
                : Math.Min(100, (int)((long)peak * 100 / highestExposureTarget));

            while (nextProgressPercent <= percentComplete)
            {
                Console.WriteLine($"{nextProgressPercent}% complete");
                nextProgressPercent++;
            }
        }

        private static bool IsCudaSetupException(Exception ex)
        {
            string typeName = ex.GetType().FullName ?? string.Empty;
            string message = ex.Message ?? string.Empty;
            return ex is DllNotFoundException ||
                   ex is EntryPointNotFoundException ||
                   typeName.Contains("Cuda", StringComparison.OrdinalIgnoreCase) ||
                   message.Contains("CUDA", StringComparison.OrdinalIgnoreCase) ||
                   message.Contains("nvcuda", StringComparison.OrdinalIgnoreCase);
        }

        private static ulong GetBatchSeed(int batch)
        {
            unchecked
            {
                return (ulong)DateTime.UtcNow.Ticks ^ ((ulong)batch + 1UL) * 0x9E3779B97F4A7C15UL;
            }
        }

        private static void BuddhabrotKernel(
            Index1D workerIndex,
            ArrayView<int> exposure,
            ArrayView<float> distanceSums,
            ArrayView<int> highest,
            int samplesPerWorkItem,
            int width,
            int height,
            float realMin,
            float realRange,
            float imaginaryMin,
            float imaginaryRange,
            int cutoff,
            int bailout,
            ulong seed)
        {
            int worker = workerIndex;
            ulong randomState = seed ^ ((ulong)worker + 1UL) * 0xBF58476D1CE4E5B9UL;

            for (int sample = 0; sample < samplesPerWorkItem; sample++)
            {
                float cR = realMin + NextUnit(ref randomState) * realRange;
                float cI = imaginaryMin + NextUnit(ref randomState) * imaginaryRange;

                int escapeIteration = GetEscapeIteration(cR, cI, bailout);
                if (escapeIteration > 0 && IsPrimeOrbit(escapeIteration))
                {
                    PlotTrajectory(
                        cR,
                        cI,
                        exposure,
                        distanceSums,
                        highest,
                        width,
                        height,
                        realMin,
                        realRange,
                        imaginaryMin,
                        imaginaryRange,
                        cutoff,
                        bailout);
                }
            }
        }

        private static int GetEscapeIteration(float cR, float cI, int bailout)
        {
            float zR = 0;
            float zI = 0;

            for (int iteration = 0; iteration <= bailout; iteration++)
            {
                float nextR = zR * zR - zI * zI + cI;
                float nextI = 2.0f * zR * zI + cR;
                zR = nextR;
                zI = nextI;

                if (zR * zR + zI * zI > 4.0f)
                {
                    return iteration + 1;
                }
            }

            return 0;
        }

        private static bool IsPrimeOrbit(int iterations)
        {
            if (iterations < 2 || iterations > MaxPrimeOrbit)
            {
                return false;
            }

            for (int divisor = 2; divisor * divisor <= iterations; divisor++)
            {
                if (iterations % divisor == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static void PlotTrajectory(
            float cR,
            float cI,
            ArrayView<int> exposure,
            ArrayView<float> distanceSums,
            ArrayView<int> highest,
            int width,
            int height,
            float realMin,
            float realRange,
            float imaginaryMin,
            float imaginaryRange,
            int cutoff,
            int bailout)
        {
            float zR = 0;
            float zI = 0;
            float totalDistance = 0;
            int localHighest = 0;

            for (int iteration = 0; iteration <= bailout; iteration++)
            {
                float previousR = zR;
                float previousI = zI;
                float nextR = zR * zR - zI * zI + cI;
                float nextI = 2.0f * zR * zI + cR;
                zR = nextR;
                zI = nextI;

                float deltaR = previousR - zR;
                float deltaI = previousI - zI;
                totalDistance += XMath.Sqrt(deltaR * deltaR + deltaI * deltaI);

                if (iteration >= cutoff)
                {
                    int x = (int)((zI - realMin) / realRange * width);
                    int y = (int)((zR - imaginaryMin) / imaginaryRange * height);

                    if (x >= 0 && y >= 0 && x < width && y < height)
                    {
                        int index = x + y * width;
                        int hitCount = Atomic.Add(ref exposure[index], 1) + 1;
                        float logDistance = totalDistance > 0 ? XMath.Log(totalDistance) : 0;
                        Atomic.Add(ref distanceSums[index], logDistance);
                        if (hitCount > localHighest)
                        {
                            localHighest = hitCount;
                        }
                    }
                }

                if (zR * zR + zI * zI > 4.0f)
                {
                    if (localHighest > 0)
                    {
                        Atomic.Max(ref highest[0], localHighest);
                    }

                    return;
                }
            }

            if (localHighest > 0)
            {
                Atomic.Max(ref highest[0], localHighest);
            }
        }

        private static float NextUnit(ref ulong state)
        {
            unchecked
            {
                state += 0x9E3779B97F4A7C15UL;
                ulong value = state;
                value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
                value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
                value ^= value >> 31;
                return (float)((value >> 40) * 5.9604644775390625E-8);
            }
        }

        private static string TryGetNvidiaGpuName()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    Arguments = "--query-gpu=name --format=csv,noheader",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return string.Empty;
                    }

                    if (!process.WaitForExit(3000))
                    {
                        process.Kill();
                        return string.Empty;
                    }

                    string output = process.StandardOutput.ReadToEnd().Trim();
                    return process.ExitCode == 0 ? output : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
