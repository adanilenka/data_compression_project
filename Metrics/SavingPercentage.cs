using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Metrics
{
    public static class SavingPercentage
    {
        public static float CalculateFile(string sourcefileName, string codedfileName)
        {
            long sourceFileLength = new FileInfo(sourcefileName).Length;
            long codedFileLength = new FileInfo(codedfileName).Length;
            return (((float)sourceFileLength - (float)codedFileLength) / (float)sourceFileLength) * 100;
        }

        public static float Calculate(string source, string coded)
        {
            var binSource = Helpers.ConvertStringToBitString(source);
            var codedSource = Helpers.ConvertStringToBitString(coded);
            return (((float)binSource.Length - (float)codedSource.Length) / (float)binSource.Length) * 100;
        }

        public static float Calculate(int sourceCount, int codedCount)
        {
            return (((float)sourceCount- (float)codedCount) / (float)sourceCount) * 100;
        }

        public static float Calculate(string source, List<int> coded)
        {
            var binSource = Helpers.ConvertStringToBitString(source);
            var bitCodedArray = new BitArray(coded.ToArray());
            return (((float)binSource.Length- (float)bitCodedArray.Length) / (float)binSource.Length) * 100;
        }
    }
}
