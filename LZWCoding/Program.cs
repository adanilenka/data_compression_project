using Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LZWCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "test.txt";
            string compressedFileName = "CompressedFile.txt";
            string decompressedFileName = "DecompressedFile.txt";
            string allText = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, sourceFileName), Encoding.Default);

            string compressed = Compress(allText);
            //string decompressed = Decompress(compressed, alphabet);

            int numOfBytes = compressed.Length / 8;
            byte[] bytesArray = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytesArray[i] = Convert.ToByte(compressed.Substring(8 * i, 8), 2);
            }
            using (FileStream fs = File.Create(Path.Combine(Environment.CurrentDirectory, compressedFileName)))
            {
                fs.Write(bytesArray, 0, bytesArray.Length);
            }
            var compressedFromFileBytes= File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, compressedFileName));
            string decompressed = Decompress(compressedFromFileBytes);
            using (FileStream fs = File.Create(Path.Combine(Environment.CurrentDirectory, decompressedFileName)))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(decompressed);
                fs.Write(bytes, 0, bytes.Length);
            }
            Console.WriteLine("Compression Ratio: " + CompressionRatio.Calculate(allText.Length, bytesArray.Length).ToString());
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(allText).ToString());
            Console.WriteLine("Coded BitRate: " + BitRate.Calculate(allText.Length, compressed.Length).ToString());
            Console.WriteLine("Saving Percentage: " + SavingPercentage.Calculate(allText.Length, bytesArray.Length).ToString());
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString() + "%");
        }

        public static string Compress(string uncompressed)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(Encoding.Default.GetString(new byte[1] { Convert.ToByte(i) }), i);
            }

            string w = string.Empty;
            List<int> compressed = new List<int>();
            StringBuilder sb = new StringBuilder();
            foreach (char c in uncompressed)
            {

                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    compressed.Add(dictionary[w]);
                    sb.Append(CompressToBit(dictionary[w], dictionary.Count));
                    if (dictionary.Count < 4096)
                    {
                        dictionary.Add(wc, dictionary.Count);
                    }
                    w = c.ToString();
                }

            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
            {
                compressed.Add(dictionary[w]);
                sb.Append(CompressToBit(dictionary[w], dictionary.Count));
            }
            var result = sb.ToString();
            result += new string( '0', 8 - (result.Length % 8));
            return result;
        }

        public static string CompressToBit(int value, int dictionaryLength)
        {
            var power= (int)Math.Ceiling(Math.Log(dictionaryLength, 2));
            var str = Convert.ToString(value, 2).PadLeft(power, '0');
            return str;
        }

        public static string Decompress(byte[] compressed)
        {
            var binaryStr =string.Join("", compressed.Select(x=> Convert.ToString(x, 2).PadLeft(8, '0')));
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(i, Encoding.Default.GetString(new byte[1] { Convert.ToByte(i) }));
            }
            var temp = binaryStr.Substring(0, 8);
            binaryStr = binaryStr.Substring(8, binaryStr.Length - 8);
            var w = dictionary[Convert.ToInt32(temp, 2)];
            StringBuilder decompressed = new StringBuilder(w);
            int countBitRead = 8;
            while (binaryStr.Length >= countBitRead) 
            {
                if((Math.Ceiling(Math.Log(dictionary.Count, 2)) == Math.Log(dictionary.Count, 2)&&dictionary.Count<4096))
                {
                    countBitRead++;
                }
                temp= binaryStr.Substring(0, countBitRead);
                binaryStr = binaryStr.Substring(countBitRead, binaryStr.Length - countBitRead);
                string entry = null;
                if (dictionary.ContainsKey(Convert.ToInt32(temp, 2))){
                    entry = dictionary[Convert.ToInt32(temp, 2)];
                }
                else
                {
                    entry = w + w[0];
                }

                decompressed.Append(entry);
                if (dictionary.Count < 4096)
                {
                    dictionary[dictionary.Count] = w + entry[0];
                }
                w = entry;

            }

            return decompressed.ToString();
        }
         
      

    }

}
