namespace Sandbox
{
    public class Settings
    {
        public string Name { get; set; }
        public double AspectRatio { get; set; }
        public int Height { get; set; }
        public int Highest { get; set; }
        public int HighestTarget { get; set; }
        public int Cutoff { get; set; }
        public int Bailout { get; set; }
        public double MinReal { get; set; }
        public double MaxReal { get; set; }
        public double MinImaginary { get; set; }
        public double MaxImaginary { get; set; }
        public int GpuProgressInterval { get; set; }
        public int Width { get; set; }
        public System.TimeSpan Time { get; set; }
    }
}