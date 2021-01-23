using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using Metrics;

namespace HuffmanTest
{
    class Program
    {
        static String encodedFileName = "Encoded.txt";
        static String fileToEncode = "handmade4.txt";
        public static void Main(string[] args)
        {
            // Console.WriteLine("Please enter the string:");

            string[] lines = File.ReadAllLines(fileToEncode);
            var encodedStrings = new List<BitArray>();
            // Build the Huffman tree
            var i = 0;
            var wholeText = "";
            foreach (string line in lines)
            {
                wholeText += line;
            }
            Console.WriteLine(i++);
            HuffmanTree huffmanTree = new HuffmanTree();
            huffmanTree.Build(wholeText);
            BitArray encoded = huffmanTree.Encode(wholeText);
            encodedStrings.Add(encoded);
            Console.Write("Encoded: ");

            Console.WriteLine();
            string decoded = huffmanTree.Decode(encoded);

            foreach (var encodedString in encodedStrings)
            {
                byte[] bytes = new byte
                    [encodedString.Length / 8 + (encodedString.Length % 8 == 0 ? 0 : 1)];
                encodedString.CopyTo(bytes, 0);
                File.WriteAllBytes(encodedFileName, bytes);
            }
            Console.WriteLine("Compression Ratio: " + CompressionRatio.CalculateHuffman(wholeText, encoded).
                ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(wholeText).ToString());
            Console.WriteLine("Coded BitRate: " + BitRate.Calculate(wholeText, encoded).ToString());
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.
                CalculateFile(Path.Combine(Environment.CurrentDirectory, fileToEncode),
                Path.Combine(Environment.CurrentDirectory, encodedFileName)).ToString() + "%");
            Console.WriteLine("Saving Percentage: " + SavingPercentage.
                CalculateHuffmanSavingPercentage(wholeText
              , encoded).ToString() + "%");
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile
                (Path.Combine(Environment.CurrentDirectory, fileToEncode),
               Path.Combine(Environment.CurrentDirectory, encodedFileName)).ToString());
         
        }
    }
}