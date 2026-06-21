using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    public class Fractal : Shader
    {
        //internal int[] exposure, canvas;
        internal double aspectRatio;
        internal double[][][] domain;
        internal String name = "Fractal";
        internal String savePath = @"C:\Fractals";
        internal DateTime t0 = DateTime.Now;
        internal TimeSpan TimeStamp { get => DateTime.Now - t0; }
        const int DataFileChunkElements = 1_048_576;

        public Fractal() { }

        public Fractal Init(double x0, double x1, double y0, double y1)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            this.highestActual = 0;
            this.domain = new double[width][][];
            //The domain is a 3D jagged array that must be initialized
            for (int x = 0; x < width; x++)
            {
                this.domain[x] = new double[height][];
                for (int y = 0; y < height; y++)
                {
                    this.domain[x][y] = new double[2];
                }
            }
            this.exposure = new int[width * height];
            this.distance = new double[width * height];
            this.canvas = new int[width * height];
            this.aspectRatio = (double)width / (double)height;
            //Define the domain with a nested for-loop
            _ = Parallel.For(0, width, x =>
            {
                //double Ax = 0;
                double Bx = width;
                double Cx = x0 * aspectRatio;
                double Dx = x1 * aspectRatio;
                //Map the real plane to the imaginary plane
                double mx = x / (Bx) * (Dx - Cx) + Cx;
                for (int y = 0; y < height; y++)
                {
                    //double Ay = 0;
                    double By = height;
                    double Cy = y0;
                    double Dy = y1;
                    //Map the real plane to the imaginary plane
                    double my = y / (By) * (Dy - Cy) + Cy;
                    domain[x][y][0] = mx;
                    domain[x][y][1] = my;
                }
            });

            return this;
        }

        public Fractal InitPoint(double r0, double i0, double W, double H)
        {

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            this.highestActual = 0;
            this.domain = new double[width][][];
            //The domain is a 3D jagged array that must be initialized
            for (int x = 0; x < width; x++)
            {
                this.domain[x] = new double[height][];
                for (int y = 0; y < height; y++)
                {
                    this.domain[x][y] = new double[2];
                }
            }
            this.exposure = new int[width * height];
            this.distance = new double[width * height];
            this.canvas = new int[width * height];
            this.aspectRatio = (double)width / (double)height;
            //Map r to x
            double Ax = 0;
            double Bx = width;
            double Cx = (r0 - W) * this.aspectRatio;
            double Dx = (r0 - W) * this.aspectRatio;
            //Map i to y 
            double Ay = 0;
            double By = height;
            double Cy = i0;
            double Dy = i0;
            for (int x = 0; x < width; x++)
            {
                //Map r to x
                double r = ((x - Ax) / (Bx - Ax) * (Dx - Cx) + Cx);
                for (int y = 0; y < height; y++)
                {
                    //Map i to y
                    double i = ((y - Ay) / (By - Ay) * (Dy - Cy) + Cy);
                    domain[x][y][0] = r;
                    domain[x][y][1] = i;
                }
            }

            return this;
        }

        public virtual Fractal Render() { return this; }

        private string FractalDirectory
        {
            get
            {
                string directoryName = name;
                foreach (char invalidChar in Path.GetInvalidFileNameChars())
                {
                    directoryName = directoryName.Replace(invalidChar, '_');
                }

                return Path.Combine(savePath, directoryName);
            }
        }

        private void EnsureSavePathExists()
        {
            Directory.CreateDirectory(FractalDirectory);
        }

        private string GetFractalPath(string fileName)
        {
            return Path.Combine(FractalDirectory, fileName);
        }

        private bool TryGetLoadPath(string fileName, out string path)
        {
            string fractalPath = GetFractalPath(fileName);
            if (File.Exists(fractalPath))
            {
                path = fractalPath;
                return true;
            }

            string legacyPath = Path.Combine(savePath, fileName);
            if (File.Exists(legacyPath))
            {
                Console.WriteLine($"Using legacy save path: {legacyPath}");
                path = legacyPath;
                return true;
            }

            path = fractalPath;
            return false;
        }

        private string GetLoadPath(string fileName)
        {
            TryGetLoadPath(fileName, out string path);
            return path;
        }


        public Fractal TransformExposure()
        {
            _ = Parallel.For(0, exposure.Length, i =>
            {
                double X = exposure[i];
                int Y = (int)Math.Log(X, 255);
                exposure[i] = Y;
            });
            return this;
        }

        public Fractal ThresholdExposure(int minExposure)
        {
            if (minExposure <= 0 || exposure == null || exposure.Length == 0)
            {
                return this;
            }

            _ = Parallel.For(0, exposure.Length, i =>
            {
                if (exposure[i] < minExposure)
                {
                    exposure[i] = 0;
                }
            });

            return this;
        }

        public Fractal Draw()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string image_path = GetFractalPath(name + ".png");
            Console.WriteLine(image_path);
            EnsureSavePathExists();
            StreamingPngWriter.SaveArgbCanvas(image_path, canvas, width, height);
            return this;
        }
        public Fractal SaveImage(String s)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string image_path = GetFractalPath(s + ".png");
            Console.WriteLine(image_path);
            EnsureSavePathExists();
            StreamingPngWriter.SaveArgbCanvas(image_path, canvas, width, height);
            return this;
        }
        public Fractal SaveExposure(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetFractalPath(name);
            EnsureSavePathExists();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (int offset = 0; offset < exposure.Length; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, exposure.Length - offset);
                    stream.Write(MemoryMarshal.AsBytes(exposure.AsSpan(offset, count)));
                }
            }
            return this;
        }
        public Fractal LoadExposure(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetLoadPath(name);
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length % sizeof(int) != 0)
            {
                throw new InvalidDataException($"Exposure data length is not divisible by {sizeof(int)} bytes: {path}");
            }

            long fileElementCount = fileInfo.Length / sizeof(int);
            if (fileElementCount > int.MaxValue)
            {
                throw new InvalidDataException($"Exposure data is too large to fit in a single .NET array: {path}");
            }

            int elementCount = checked((int)fileElementCount);
            exposure = new int[elementCount];
            canvas = new int[elementCount];
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int offset = 0; offset < exposure.Length; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, exposure.Length - offset);
                    stream.ReadExactly(MemoryMarshal.AsBytes(exposure.AsSpan(offset, count)));
                }
            }
            return this;
        }
        public Fractal SaveDistance(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetFractalPath(name);
            EnsureSavePathExists();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (int offset = 0; offset < distance.Length; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, distance.Length - offset);
                    stream.Write(MemoryMarshal.AsBytes(distance.AsSpan(offset, count)));
                }
            }
            return this;
        }
        public Fractal LoadDistance(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetLoadPath(name);
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length % sizeof(double) != 0)
            {
                throw new InvalidDataException($"Distance data length is not divisible by {sizeof(double)} bytes: {path}");
            }

            long fileElementCount = fileInfo.Length / sizeof(double);
            if (fileElementCount > int.MaxValue)
            {
                throw new InvalidDataException($"Distance data is too large to fit in a single .NET array: {path}");
            }

            int elementCount = checked((int)fileElementCount);
            distance = new double[elementCount];
            canvas = new int[elementCount];
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int offset = 0; offset < distance.Length; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, distance.Length - offset);
                    stream.ReadExactly(MemoryMarshal.AsBytes(distance.AsSpan(offset, count)));
                }
            }
            return this;
        }

        public Fractal SaveFlow(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (flowX == null || flowY == null || flowX.Length == 0)
            {
                return this;
            }

            if (flowX.Length != flowY.Length)
            {
                throw new InvalidDataException("Flow component arrays must have the same length.");
            }

            string path = GetFractalPath(name);
            EnsureSavePathExists();
            float[] chunk = new float[DataFileChunkElements * 2];
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (int offset = 0; offset < flowX.Length; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, flowX.Length - offset);
                    for (int i = 0; i < count; i++)
                    {
                        int sourceIndex = offset + i;
                        int targetIndex = i * 2;
                        chunk[targetIndex] = flowX[sourceIndex];
                        chunk[targetIndex + 1] = flowY[sourceIndex];
                    }

                    stream.Write(MemoryMarshal.AsBytes(chunk.AsSpan(0, count * 2)));
                }
            }

            return this;
        }

        public Fractal TryLoadFlow(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            if (!TryGetLoadPath(name, out string path))
            {
                return this;
            }

            FileInfo fileInfo = new FileInfo(path);
            int bytesPerPixel = sizeof(float) * 2;
            if (fileInfo.Length % bytesPerPixel != 0)
            {
                throw new InvalidDataException($"Flow data length is not divisible by {bytesPerPixel} bytes: {path}");
            }

            long filePixelCount = fileInfo.Length / bytesPerPixel;
            if (filePixelCount > int.MaxValue)
            {
                throw new InvalidDataException($"Flow data is too large to fit in a single .NET array: {path}");
            }

            int pixelCount = checked((int)filePixelCount);
            if (width > 0 && height > 0)
            {
                int expectedPixelCount = checked(width * height);
                if (pixelCount != expectedPixelCount)
                {
                    throw new InvalidDataException($"Flow data has {pixelCount} pixels, but settings expect {expectedPixelCount}: {path}");
                }
            }

            flowX = new float[pixelCount];
            flowY = new float[pixelCount];
            float[] chunk = new float[DataFileChunkElements * 2];
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int offset = 0; offset < pixelCount; offset += DataFileChunkElements)
                {
                    int count = Math.Min(DataFileChunkElements, pixelCount - offset);
                    stream.ReadExactly(MemoryMarshal.AsBytes(chunk.AsSpan(0, count * 2)));
                    for (int i = 0; i < count; i++)
                    {
                        int sourceIndex = i * 2;
                        int targetIndex = offset + i;
                        flowX[targetIndex] = chunk[sourceIndex];
                        flowY[targetIndex] = chunk[sourceIndex + 1];
                    }
                }
            }

            return this;
        }

        public Fractal SaveSettings(String name)
        {
            Settings _data = new Settings()
            {
                Name = this.name,
                AspectRatio = this.aspectRatio,
                Height = this.height,
                Highest = this.highestActual,
                Time = this.TimeStamp,
                Width = this.width
            };

            return SaveSettings(name, _data);
        }

        public Fractal SaveSettings(String name, Settings data)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetFractalPath(name);
            EnsureSavePathExists();
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
            return this;
        }

        public Fractal LoadSettings(String name)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            string path = GetLoadPath(name);
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Settings data = (Settings)serializer.Deserialize(file, typeof(Settings));
                aspectRatio = data.AspectRatio;
                highestActual = data.Highest;
                height = data.Height;
                width = data.Width;
            }
            return this;
        }
    }
}
