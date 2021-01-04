using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Metrics
{
    public static class Helpers
    {
        public static string ConvertStringToBitString(string str)
        {
            var byteArray = Encoding.UTF8.GetBytes(str);
            string binStr = string.Join("", byteArray.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray());
            return binStr;
        }

    }
}
