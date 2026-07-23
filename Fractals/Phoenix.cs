using System;
using System.Threading.Tasks;

namespace Sandbox.Fractals
{
    class Phoenix : Fractal
    {
        private readonly int bailout;
        private readonly double cReal;
        private readonly double cImaginary;
        private readonly double phoenixReal;
        private readonly double phoenixImaginary;

        public Phoenix(int _width, int _height, int _bailout, int _highestExposureTarget)
        {
            name = "Phoenix";
            width = _width;
            height = _height;
            bailout = _bailout;
            highestExposureTarget = _highestExposureTarget;
            cReal = -0.5;
            cImaginary = 0.0;
            phoenixReal = -0.56667;
            phoenixImaginary = 0.0;
        }

        public override Fractal Render()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    double zReal = domain[x][y][0];
                    double zImaginary = domain[x][y][1];
                    double previousReal = 0.0;
                    double previousImaginary = 0.0;
                    int iterations = 0;
                    double totalDistance = 0.0;

                    while (iterations < bailout && zReal * zReal + zImaginary * zImaginary < 4.0)
                    {
                        // Phoenix Julia iteration:
                        // z_{n+1} = z_n^2 + c + p z_{n-1}.
                        double squaredReal = zReal * zReal - zImaginary * zImaginary;
                        double squaredImaginary = 2.0 * zReal * zImaginary;
                        double nextReal = squaredReal + cReal + phoenixReal * previousReal - phoenixImaginary * previousImaginary;
                        double nextImaginary = squaredImaginary + cImaginary + phoenixReal * previousImaginary + phoenixImaginary * previousReal;

                        double stepReal = nextReal - zReal;
                        double stepImaginary = nextImaginary - zImaginary;
                        totalDistance += Math.Sqrt(stepReal * stepReal + stepImaginary * stepImaginary);

                        previousReal = zReal;
                        previousImaginary = zImaginary;
                        zReal = nextReal;
                        zImaginary = nextImaginary;
                        iterations++;
                    }

                    int index = x + y * width;
                    exposure[index] = iterations;
                    distance[index] = Math.Log(totalDistance + 1.0);
                    if (highestActual < exposure[index])
                    {
                        highestActual = exposure[index];
                    }
                }
            });

            return this;
        }
    }
}
