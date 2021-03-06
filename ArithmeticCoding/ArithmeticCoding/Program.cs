﻿using Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "test1.txt";
            string compressedFileName = "CompressedFile.txt";
            string decompressedFileName = "DecompressedFile.txt";
            string source = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, sourceFileName), Encoding.Default);
            (string compressed, Dictionary<char, ProbabilityInterval> probabilityIntervals) =  ArithmeticCoder.Coding(source);
            //string decompressed =  ArithmeticCoder.Decoding(compressed, probabilityIntervals, source.Length);

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
            Console.WriteLine("Compression Ratio: " + CompressionRatio.Calculate(source.Length, bytesArray.Length).ToString());
            Console.WriteLine("Compression Ratio (File): " + CompressionRatio.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString());
            Console.WriteLine("Source BitRate: " + BitRate.Calculate(source).ToString());
            Console.WriteLine("Coded BitRate: " + BitRate.Calculate(source.Length, compressed.Length).ToString());
            Console.WriteLine("Saving Percentage: " + SavingPercentage.Calculate(source.Length, bytesArray.Length).ToString());
            Console.WriteLine("Saving Percentage (File): " + SavingPercentage.CalculateFile(Path.Combine(Environment.CurrentDirectory, sourceFileName),
                Path.Combine(Environment.CurrentDirectory, "CompressedFile.txt")).ToString() + "%");
        }

        public static (string, Dictionary<char, ProbabilityInterval>) Coding(string source)
        {
            int buffCount = 5;
            string lowStr = new string('0', buffCount);
            string highStr = new string('9', buffCount);
            var probabilityIntervals = GetProbabilityIntervals(source);
            string codedString = "";
            int Lcounter = 0;
            for (int i = 0; i < source.Length; i++)
            {
                double range = (double.Parse(highStr) - double.Parse(lowStr) + 1);
                string tempLowStr = lowStr;
                lowStr = (double.Parse(tempLowStr) + range * probabilityIntervals[source[i]].IntervalMin).ToString();
                highStr = (double.Parse(tempLowStr) + range * probabilityIntervals[source[i]].IntervalMax - 1).ToString();
                if (lowStr.Length < buffCount)
                {
                    lowStr += new string('0', buffCount - lowStr.Length);
                }
                if (highStr.Length < buffCount)
                {
                    highStr += new string('9', buffCount - highStr.Length);
                }

                while (lowStr[0] == highStr[0])
                {
                    codedString += lowStr[0];
                    lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                    highStr = highStr.Substring(1, highStr.Length - 1) + "9";
                    Lcounter--;

                }
                if ((lowStr[0] != highStr[0]) && (lowStr[1] == '0') && (highStr[1] == '1'))
                {
                    lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                    highStr = highStr.Substring(1, highStr.Length - 1) + "9";
                    Lcounter++;
                }
            }
            codedString += string.Concat(lowStr.Where(s => s != '0'));
            return (codedString, probabilityIntervals);
        }

        public static string Decoding(string compressed, Dictionary<char, ProbabilityInterval> probabilityIntervals)
        {
            int buffCount = 5;
            string lowStr = new string('0', buffCount);
            string highStr = new string('9', buffCount);
            string tempCompressed = compressed;
            string code = tempCompressed.Substring(0, buffCount);
            tempCompressed = tempCompressed.Substring(buffCount, tempCompressed.Length-buffCount);
            int pointer = 1;
            string source = "";
            for (int i = 0; i < compressed.Length; i++)
            {
                int range = (int.Parse(highStr) - int.Parse(lowStr) + 1);
                float localCode = (float.Parse(code) - int.Parse(lowStr)) / range;
                var sourceSymbol = probabilityIntervals.Where(x => (x.Value.IntervalMin <= localCode) && (x.Value.IntervalMax > localCode)).FirstOrDefault();
                source += sourceSymbol.Value.Symbol;
                string tempLowStr = lowStr;
                lowStr = (int.Parse(tempLowStr) + range * sourceSymbol.Value.IntervalMin).ToString();
                highStr = (int.Parse(tempLowStr) + range * sourceSymbol.Value.IntervalMax - 1).ToString();
                
                if (lowStr.Length < buffCount)
                {
                    lowStr += new string('0', buffCount - lowStr.Length);
                }
                if (highStr.Length < buffCount)
                {
                    highStr += new string('9', buffCount - highStr.Length);
                }
                while (lowStr[0] == highStr[0])
                {
                    pointer++;
                    code = code.Substring(1, code.Length - 1);
                    if (tempCompressed.Length != 0)
                    {
                        code += tempCompressed.Substring(0, 1);
                        tempCompressed = tempCompressed.Substring(1, tempCompressed.Length - 1);
                    }
                    else
                    {
                        code += 0;
                    }
                    lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                    highStr = highStr.Substring(1, highStr.Length - 1) + "9";
                }
                if ((lowStr[0] != highStr[0]) && (lowStr[1] == '0') && (highStr[1] == '1'))
                {
                    lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                    highStr = highStr.Substring(1, highStr.Length - 1) + "9";
                }
            }

            return source;
        }

        public static string Decoding(string coded)
        {
  
            return "";
        }
        public static Dictionary<char, ProbabilityInterval> GetProbabilityIntervals(string source)
        {

            var frequencies = new Dictionary<char, int>();
            foreach (var s in source)
            {
                if (!frequencies.ContainsKey(s)) frequencies[s] = 1;
                else frequencies[s]++;
            }
            var probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            frequencies = frequencies.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            float tempValue = 0;
            foreach (var f in frequencies)
            {
                probabilityIntervals.Add(f.Key, new ProbabilityInterval()
                {
                    Symbol = f.Key,
                    Frequency = f.Value,
                    Probability = ((float)f.Value) / source.Length,
                    //IntervalMin = tempValue,
                    //IntervalMax = tempValue + ((float)f.Value) / source.Length
                });
                tempValue = tempValue + ((float)f.Value) / source.Length;
            }
            return probabilityIntervals;
        }


    }

}
