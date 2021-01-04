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
            string allText = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Test.txt"), Encoding.Default);

            (List<char> alphabet, List<int> compressed) = Compress(allText);
            string decompressed = Decompress(compressed, alphabet);
            using (FileStream fs = File.Create(Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")))
            {
                fs.Write(alphabet.Select(c => (byte)c).ToArray(), 0, alphabet.Select(c => (byte)c).ToArray().Length);
                byte[] compressedByteArray = new byte[compressed.Count * sizeof(int)];
                Buffer.BlockCopy(compressed.ToArray(), 0, compressedByteArray, 0, compressedByteArray.Length);
                fs.Write(compressedByteArray, 0, compressedByteArray.Length);
            }

            Console.WriteLine("Compression Ratio: " + CompressionRatio.Calculate(allText, compressed).ToString());
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile(Path.Combine(Environment.CurrentDirectory, "Test.txt"),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(allText).ToString());
            Console.WriteLine("Coded BitRate: " + BitRate.Calculate(allText, compressed).ToString());
            Console.WriteLine("Saving Percentage: " + SavingPercentage.Calculate(allText, compressed).ToString());
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.CalculateFile(Path.Combine(Environment.CurrentDirectory, "Test.txt"),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString() + "%");
        }

        public static (List<char>, List<int>) Compress(string uncompressed)
        {
            // build the dictionary
            var alphabet = uncompressed.ToList().Distinct().ToList();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int i = 0;
            foreach (char c in alphabet)
            {
                dictionary.Add((c).ToString(), i);
                i++;
            }


            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return (alphabet, compressed);
        }


        public static string Decompress(List<int> compressed, List<char> alphabet)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int i = 0;
            foreach (char c in alphabet)
            {
                dictionary.Add(i, (c).ToString());
                i++;
            }
            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                // new sequence; add it to the dictionary
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }

    }

}
