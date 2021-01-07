using Metrics;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GolombCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "Test1.txt";
            string compressedFileName = "CompressedFile.txt";
            string decompressedFileName = "DecompressedFile.txt";
            string allText = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, sourceFileName), Encoding.Default);
            int alphabetLength = new string(allText.Distinct().ToArray()).Length;
            string compressed = Compress(allText, 4);

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
            //var compressedFromFileBytes = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, compressedFileName));
            //var binaryCompressedStr = string.Join("", compressedFromFileBytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
            //string decompressed = Decoding(binaryCompressedStr, 4);
            //using (FileStream fs = File.Create(Path.Combine(Environment.CurrentDirectory, decompressedFileName)))
            //{
            //    byte[] bytes = Encoding.UTF8.GetBytes(decompressed);
            //    fs.Write(bytes, 0, bytes.Length);
            //}
            Console.WriteLine("Compression Ratio: " + CompressionRatio.Calculate(allText.Length, bytesArray.Length).ToString());
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(allText).ToString());
            Console.WriteLine("Coded BitRate: " + BitRate.Calculate(allText.Length, compressed.Length).ToString());
            Console.WriteLine("Saving Percentage: " + SavingPercentage.Calculate(allText.Length, bytesArray.Length).ToString());
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString() + "%");
        }

        public static string Compress(string source, int k)
        {
            int M = Convert.ToInt32(Math.Pow(2, k));
            StringBuilder codedSb = new StringBuilder();
            foreach (var s in source)
            {
                int tempS = (int)s;

                int suffix = tempS % M;
                string suffixBin = Convert.ToString(suffix, 2).PadLeft(k, '0');
                string prefix = Convert.ToString(tempS, 2).PadLeft(8, '0').Substring(0, k);
                string prefixUnary = new string('0', Convert.ToInt32(prefix, 2)) + "1";
                codedSb.Append(prefixUnary + suffixBin);
            }
            return codedSb.ToString();
        }

        public static string Decoding(string compressed, int k)
        {
            StringBuilder decodedSb = new StringBuilder();

            int M = Convert.ToInt32(Math.Pow(2, k));
            while (compressed.Length > 0)
            {
                try {
                    int i = 0;
                    while (compressed[i] != '1')
                    {
                        i++;
                    }
                    int Q = i;
                    string suffix = compressed.Substring(i + 1, k);
                    int S = Q * M + Convert.ToInt32(suffix, 2);
                    decodedSb.Append(Convert.ToChar(S));
                    compressed = compressed.Substring(i + 1 + k, compressed.Length - i - k - 1);
                }
                catch
                {
                    Console.WriteLine("Ex");
                    break;
                }     
            }
            return decodedSb.ToString();
        }

        public static string Compress1(string source, int n)
        {
            int k =Convert.ToInt32(Math.Floor(Math.Log2(n)));
            int u = Convert.ToInt32(Math.Pow(2, k+1))-n;
            StringBuilder codedSb = new StringBuilder();
            foreach (var s in source)
            {
                int tempS = (int)s;
                string prefix = Convert.ToString(tempS, 2).PadLeft(8, '0').Substring(0, k);
                string prefixUnary = new string('0', Convert.ToInt32(prefix, 2)) + "1";
                if (tempS < u) {
                    codedSb.Append(prefixUnary+ Binary(tempS, k));
                }
                else
                {
                    codedSb.Append(prefixUnary+Binary(tempS + u, k+1));
                }
            }
            return codedSb.ToString();
        }

        public static string Binary(int x, int k)
        {
            string s = "";
            while (x != 0)
            {
                if (x % 2 == 0)
                {
                    s = "0" + s;
                }
                else
                {
                    s = "1" + s;
                }
                x >>= 1;
            }
            while (s.Length < k) s = "0" + s;
            return s;
        }
        public static string Decoding1(string compressed, int k)
        {
            StringBuilder decodedSb = new StringBuilder();

            int M = Convert.ToInt32(Math.Pow(2, k));
            while (compressed.Length > 0)
            {
                try
                {
                    int i = 0;
                    while (compressed[i] != '1')
                    {
                        i++;
                    }
                    int Q = i;
                    string suffix = compressed.Substring(i + 1, k);
                    int S = Q * M + Convert.ToInt32(suffix, 2);
                    decodedSb.Append(Convert.ToChar(S));
                    compressed = compressed.Substring(i + 1 + k, compressed.Length - i - k - 1);
                }
                catch
                {
                    Console.WriteLine("Ex");
                    break;
                }
            }
            return decodedSb.ToString();
        }


    }
}
