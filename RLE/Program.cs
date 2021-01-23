
using Data_Compression;
using Metrics;
using System;
using System.IO;

namespace RLEEncoding
{
    public class Program
    {
        public static void Main(String[] args)
        {
             String encodedFileName = "Encoded.txt";
             String fileToEncode = "handmade1.txt";
            //String str = "AAAAABBCCCD";
            string[] lines = File.ReadAllLines(fileToEncode);

            var wholeText = "";

            foreach (string line in lines)
            {
                wholeText += line;
            }

            var encoded = RLE.encode(wholeText);
            //Console.WriteLine(encoded);
            using StreamWriter outputFile = new StreamWriter("Encoded.txt");
            outputFile.Write(encoded);
            Console.WriteLine("Compression Ratio: " + CompressionRatio.Calculate(wholeText, encoded).ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(wholeText).ToString());
           Console.WriteLine("Coded BitRate: " + BitRate.Calculate(wholeText,encoded).ToString());
            Console.WriteLine("Saving Percentage: " + SavingPercentage.Calculate
                (wholeText,encoded).ToString()
                                + "%");
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.CalculateFile(
                Path.Combine(Environment.CurrentDirectory,fileToEncode ),
Path.Combine(Environment.CurrentDirectory, encodedFileName)).ToString() + "%");
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile(Path.
                Combine(Environment.CurrentDirectory, fileToEncode),
                Path.Combine(Environment.CurrentDirectory, encodedFileName)).ToString());
        }
    }
}