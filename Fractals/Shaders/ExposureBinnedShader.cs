using System;
using System.Collections;
using System.Collections.Generic;

namespace Sandbox.Fractals
{
    public partial class Shader
    {
        public Shader ExposureBinned()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
            highestActual = Helper.GetMinMax(exposure).Item2;
            HashSet<int> set = new HashSet<int>();
            Hashtable table = new Hashtable();

            for (int i = 0; i < exposure.Length; i++)
            {
                set.Add(exposure[i]);
            }

            int[] sortedArray = new int[set.Count];
            set.CopyTo(sortedArray);
            Array.Sort(sortedArray);

            for (int i = 0; i < sortedArray.Length; i++)
            {
                table.Add(sortedArray[i], (double)i);
            }

            for (int i = 0; i < exposure.Length; i++)
            {
                double x = (double)table[exposure[i]];
                int y = (int)((x / table.Count) * 256);

                if (y < 0 || y > 255) { throw new Exception(y + " is too dim or too bright!"); }
                canvas[i] = PackColor(y, y, y);
            }

            return this;
        }
    }
}
