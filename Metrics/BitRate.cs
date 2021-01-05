using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Metrics
{
    public static class BitRate
    {
   
        public static float Calculate(string source)
        {
            var binSource = Helpers.ConvertStringToBitString(source);

            return ((float)binSource.Length / (float)source.Length);
        }

        public static float Calculate(string source, string coded)
        {
            var codedSource = Helpers.ConvertStringToBitString(coded);
            return ((float)codedSource.Length / (float)source.Length);
        }


        public static float Calculate(string source, List<int> codedArray)
        {
            var bitArray = new BitArray(codedArray.ToArray());
            return ((float)bitArray.Length / (float)source.Length);
        }

        public static float Calculate(string source, BitArray bitArray)
        {
            return ((float)bitArray.Length / (float)source.Length);
        }

  
    }
}
