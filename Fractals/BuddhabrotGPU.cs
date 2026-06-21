using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace Sandbox.Fractals
{
    public class BuddhabrotGPU : Fractal
    {
        public const int DefaultProgressCheckBatchInterval = 8;

        const int WorkItemsPerBatch = 131072;
        const int SamplesPerWorkItem = 8;

        readonly int cutoff, bailout;
        readonly int progressCheckBatchInterval;
        double realMin, realMax, imaginaryMin, imaginaryMax;
        bool initialized;
        int nextProgressPercent;

        public struct BuddhabrotGpuParameters
        {
            public int Width;
            public int Height;
            public float RealMin;
            public float RealRange;
            public float RealScale;
            public float ImaginaryMin;
            public float ImaginaryRange;
            public float ImaginaryScale;
            public int Cutoff;
            public int Bailout;

            public BuddhabrotGpuParameters(
                int width,
                int height,
                float realMin,
                float realRange,
                float realScale,
                float imaginaryMin,
                float imaginaryRange,
                float imaginaryScale,
                int cutoff,
                int bailout)
            {
                Width = width;
                Height = height;
                RealMin = realMin;
                RealRange = realRange;
                RealScale = realScale;
                ImaginaryMin = imaginaryMin;
                ImaginaryRange = imaginaryRange;
                ImaginaryScale = imaginaryScale;
                Cutoff = cutoff;
                Bailout = bailout;
            }
        }

        public BuddhabrotGPU(int _width, int _height, int _cutoff, int _bailout, int _highestExposureTarget, int _progressCheckBatchInterval = DefaultProgressCheckBatchInterval)
        {
            this.name = "Buddhabrot-GPU";
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
                throw new InvalidOperationException("BuddhabrotGPU must be initialized before rendering.");
            }

            int pixelCount = checked(width * height);
            float realMinF = (float)realMin;
            float realRange = (float)(realMax - realMin);
            float realScale = width / realRange;
            float imaginaryMinF = (float)imaginaryMin;
            float imaginaryRange = (float)(imaginaryMax - imaginaryMin);
            float imaginaryScale = height / imaginaryRange;
            if (realRange == 0 || imaginaryRange == 0)
            {
                throw new InvalidOperationException("BuddhabrotGPU needs a non-zero render domain.");
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

                return RenderWithAccelerator(accelerator, pixelCount, realMinF, realRange, realScale, imaginaryMinF, imaginaryRange, imaginaryScale);
            }
            catch (Exception ex) when (IsCudaSetupException(ex))
            {
                string hardwareStatus = string.IsNullOrWhiteSpace(gpuName)
                    ? "No NVIDIA GPU was detected by nvidia-smi."
                    : $"Detected NVIDIA GPU: {gpuName}.";

                throw new NotSupportedException(
                    $"{hardwareStatus} BuddhabrotGPU could not create an ILGPU CUDA accelerator. " +
                    "Install or repair the NVIDIA CUDA driver runtime, then run 'buddhabrot-gpu render' again.",
                    ex);
            }
        }

        private Fractal RenderWithAccelerator(
            Accelerator accelerator,
            int pixelCount,
            float realMinF,
            float realRange,
            float realScale,
            float imaginaryMinF,
            float imaginaryRange,
            float imaginaryScale)
        {
            nextProgressPercent = 0;
            ReportProgress(0);

            using MemoryBuffer1D<int, Stride1D.Dense> exposureBuffer = accelerator.Allocate1D<int>(pixelCount);
            using MemoryBuffer1D<float, Stride1D.Dense> distanceSumsBuffer = accelerator.Allocate1D<float>(pixelCount);
            using MemoryBuffer1D<int, Stride1D.Dense> highestBuffer = accelerator.Allocate1D<int>(1);
            exposureBuffer.MemSetToZero();
            distanceSumsBuffer.MemSetToZero();
            highestBuffer.MemSetToZero();

            BuddhabrotGpuParameters parameters = new BuddhabrotGpuParameters(
                width,
                height,
                realMinF,
                realRange,
                realScale,
                imaginaryMinF,
                imaginaryRange,
                imaginaryScale,
                cutoff,
                bailout);

            var kernel = KernelLoaders.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView<int>,
                ArrayView<float>,
                ArrayView<int>,
                int,
                BuddhabrotGpuParameters,
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
                        parameters,
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
            BuddhabrotGpuParameters parameters,
            ulong seed)
        {
            int worker = workerIndex;
            uint randomState = (uint)(seed ^ ((ulong)worker + 1UL) * 0xBF58476D1CE4E5B9UL);
            if (randomState == 0u)
            {
                randomState = 1u;
            }

            for (int sample = 0; sample < samplesPerWorkItem; sample++)
            {
                float cR = parameters.RealMin + NextUnit(ref randomState) * parameters.RealRange;
                float cI = parameters.ImaginaryMin + NextUnit(ref randomState) * parameters.ImaginaryRange;

                if (Escapes(cR, cI, parameters.Bailout))
                {
                    PlotTrajectory(
                        cR,
                        cI,
                        exposure,
                        distanceSums,
                        highest,
                        parameters);
                }
            }
        }

        private static bool Escapes(float cR, float cI, int bailout)
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
                    return true;
                }
            }

            return false;
        }

        private static void PlotTrajectory(
            float cR,
            float cI,
            ArrayView<int> exposure,
            ArrayView<float> distanceSums,
            ArrayView<int> highest,
            BuddhabrotGpuParameters parameters)
        {
            float zR = 0;
            float zI = 0;
            float totalDistance = 0;
            int localHighest = 0;

            int bufferedIndex0 = -1;
            int bufferedCount0 = 0;
            float bufferedDistance0 = 0;
            int bufferedIndex1 = -1;
            int bufferedCount1 = 0;
            float bufferedDistance1 = 0;
            int bufferedIndex2 = -1;
            int bufferedCount2 = 0;
            float bufferedDistance2 = 0;
            int bufferedIndex3 = -1;
            int bufferedCount3 = 0;
            float bufferedDistance3 = 0;

            for (int iteration = 0; iteration <= parameters.Bailout; iteration++)
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

                if (iteration >= parameters.Cutoff)
                {
                    int x = (int)((zI - parameters.RealMin) * parameters.RealScale);
                    int y = (int)((zR - parameters.ImaginaryMin) * parameters.ImaginaryScale);

                    if (x >= 0 && y >= 0 && x < parameters.Width && y < parameters.Height)
                    {
                        int index = x + y * parameters.Width;
                        float logDistance = totalDistance > 0 ? XMath.Log(totalDistance) : 0;

                        if (index == bufferedIndex0)
                        {
                            bufferedCount0++;
                            bufferedDistance0 += logDistance;
                        }
                        else if (index == bufferedIndex1)
                        {
                            bufferedCount1++;
                            bufferedDistance1 += logDistance;
                        }
                        else if (index == bufferedIndex2)
                        {
                            bufferedCount2++;
                            bufferedDistance2 += logDistance;
                        }
                        else if (index == bufferedIndex3)
                        {
                            bufferedCount3++;
                            bufferedDistance3 += logDistance;
                        }
                        else if (bufferedIndex0 < 0)
                        {
                            bufferedIndex0 = index;
                            bufferedCount0 = 1;
                            bufferedDistance0 = logDistance;
                        }
                        else if (bufferedIndex1 < 0)
                        {
                            bufferedIndex1 = index;
                            bufferedCount1 = 1;
                            bufferedDistance1 = logDistance;
                        }
                        else if (bufferedIndex2 < 0)
                        {
                            bufferedIndex2 = index;
                            bufferedCount2 = 1;
                            bufferedDistance2 = logDistance;
                        }
                        else if (bufferedIndex3 < 0)
                        {
                            bufferedIndex3 = index;
                            bufferedCount3 = 1;
                            bufferedDistance3 = logDistance;
                        }
                        else
                        {
                            int updatedCount = Atomic.Add(ref exposure[bufferedIndex0], bufferedCount0) + bufferedCount0;
                            Atomic.Add(ref distanceSums[bufferedIndex0], bufferedDistance0);
                            if (updatedCount > localHighest)
                            {
                                localHighest = updatedCount;
                            }

                            bufferedIndex0 = index;
                            bufferedCount0 = 1;
                            bufferedDistance0 = logDistance;
                        }
                    }
                }

                if (zR * zR + zI * zI > 4.0f)
                {
                    if (bufferedIndex0 >= 0)
                    {
                        int updatedCount = Atomic.Add(ref exposure[bufferedIndex0], bufferedCount0) + bufferedCount0;
                        Atomic.Add(ref distanceSums[bufferedIndex0], bufferedDistance0);
                        if (updatedCount > localHighest)
                        {
                            localHighest = updatedCount;
                        }
                    }
                    if (bufferedIndex1 >= 0)
                    {
                        int updatedCount = Atomic.Add(ref exposure[bufferedIndex1], bufferedCount1) + bufferedCount1;
                        Atomic.Add(ref distanceSums[bufferedIndex1], bufferedDistance1);
                        if (updatedCount > localHighest)
                        {
                            localHighest = updatedCount;
                        }
                    }
                    if (bufferedIndex2 >= 0)
                    {
                        int updatedCount = Atomic.Add(ref exposure[bufferedIndex2], bufferedCount2) + bufferedCount2;
                        Atomic.Add(ref distanceSums[bufferedIndex2], bufferedDistance2);
                        if (updatedCount > localHighest)
                        {
                            localHighest = updatedCount;
                        }
                    }
                    if (bufferedIndex3 >= 0)
                    {
                        int updatedCount = Atomic.Add(ref exposure[bufferedIndex3], bufferedCount3) + bufferedCount3;
                        Atomic.Add(ref distanceSums[bufferedIndex3], bufferedDistance3);
                        if (updatedCount > localHighest)
                        {
                            localHighest = updatedCount;
                        }
                    }

                    if (localHighest > 0)
                    {
                        Atomic.Max(ref highest[0], localHighest);
                    }

                    return;
                }
            }

            if (bufferedIndex0 >= 0)
            {
                int updatedCount = Atomic.Add(ref exposure[bufferedIndex0], bufferedCount0) + bufferedCount0;
                Atomic.Add(ref distanceSums[bufferedIndex0], bufferedDistance0);
                if (updatedCount > localHighest)
                {
                    localHighest = updatedCount;
                }
            }
            if (bufferedIndex1 >= 0)
            {
                int updatedCount = Atomic.Add(ref exposure[bufferedIndex1], bufferedCount1) + bufferedCount1;
                Atomic.Add(ref distanceSums[bufferedIndex1], bufferedDistance1);
                if (updatedCount > localHighest)
                {
                    localHighest = updatedCount;
                }
            }
            if (bufferedIndex2 >= 0)
            {
                int updatedCount = Atomic.Add(ref exposure[bufferedIndex2], bufferedCount2) + bufferedCount2;
                Atomic.Add(ref distanceSums[bufferedIndex2], bufferedDistance2);
                if (updatedCount > localHighest)
                {
                    localHighest = updatedCount;
                }
            }
            if (bufferedIndex3 >= 0)
            {
                int updatedCount = Atomic.Add(ref exposure[bufferedIndex3], bufferedCount3) + bufferedCount3;
                Atomic.Add(ref distanceSums[bufferedIndex3], bufferedDistance3);
                if (updatedCount > localHighest)
                {
                    localHighest = updatedCount;
                }
            }

            if (localHighest > 0)
            {
                Atomic.Max(ref highest[0], localHighest);
            }
        }

        private static float NextUnit(ref uint state)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return (state & 0x00FFFFFFu) * (1.0f / 16777216f);
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
