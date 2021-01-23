using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data_Compression
{
    public class RLE
    {
        // Perform Run Length Encoding (RLE) data compression algorithm
        // on String str
        public static String encode(String str)
        {
            // stores output String
            String encoding = "";
            int count;

            for (int i = 0; i < str.Length; i++)
            {
                // count occurrences of character at index i
                count = 1;
                while (i + 1 < str.Length && str[i] == str[i + 1])
                {
                    count++;
                    i++;
                }

                // append current character and its count to the result
                encoding += count.ToString() + str[i];
            }

            return encoding;
        }

       
    }




}
