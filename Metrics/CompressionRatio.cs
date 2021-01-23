using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Metrics
{
    public static class CompressionRatio
    {
        public static float CalculateFile(string sourcefileName, string codedfileName)
        {
            long sourceFileLength = new FileInfo(sourcefileName).Length;
            long codedFileLength = new FileInfo(codedfileName).Length;
            return ((float)sourceFileLength  / (float)codedFileLength);
        }

        public static float Calculate(int sourceCount, int codedCount)
        {
            return ((float)sourceCount / (float)codedCount);
        }

        public static float Calculate(string source, string coded)
        {
            var binSource =Helpers.ConvertStringToBitString(source);
            var codedSource = Helpers.ConvertStringToBitString(coded);
            return ((float)binSource.Length / (float)codedSource.Length);
        }

        public static float Calculate(string source, List<int> coded)
        {
            var binSource = Helpers.ConvertStringToBitString(source);
            var bitCodedArray = new BitArray(coded.ToArray());
            return ((float)binSource.Length / (float)bitCodedArray.Length);
        }
        public static float CalculateHuffman(string source, BitArray coded)
        {
            var binSource = Helpers.ConvertStringToBitString(source);
            return ((float)binSource.Length / (float)coded.Length);
        }
      

    }
}
