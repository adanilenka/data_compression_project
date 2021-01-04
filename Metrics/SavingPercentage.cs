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
        public static float Calculate(string sourcefileName, string codedfileName)
        {
            long sourceFileLength = new FileInfo(sourcefileName).Length;
            long codedFileLength = new FileInfo(codedfileName).Length;
            return (((float)sourceFileLength - (float)codedFileLength) / (float)sourceFileLength) * 100;
        }
    }
}
