using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticCoding
{
    public static class ArithmeticCoder
    {
        public static (string , Dictionary<char, ProbabilityInterval>) Coding(string source)
        {
            //source = "1321";
            int buffCount = 30;
            int n = buffCount - 2;
            string lowStr = new string('0', buffCount);
            string highStr = new string('1', buffCount);
            var probabilityIntervals = GetProbabilityIntervalsAdaptive(source);

            StringBuilder codedString = new StringBuilder();
            int Lcounter = 0;
            //int divNum = Convert.ToInt32(Math.Pow(2, buffCount));
            int divProb = Convert.ToInt32(Math.Pow(2, n));
            for (int i = 0; i < source.Length; i++)
            {
                double low = Convert.ToInt32(lowStr, 2);
                double high = Convert.ToInt32(highStr, 2);
                double range = high - low + 1;
                high =Math.Floor(low +  range * (Convert.ToDouble(probabilityIntervals[source[i]].IntervalMax)/(divProb))) -1;
                low = Math.Floor(low + range * (Convert.ToDouble(probabilityIntervals[source[i]].IntervalMin )/(divProb)));
                highStr = Convert.ToString(Convert.ToInt32(high), 2).PadLeft(buffCount, '0');
                lowStr = Convert.ToString(Convert.ToInt32(low), 2).PadLeft(buffCount, '0');
                while ((lowStr[0] == highStr[0])||((lowStr[1] == '1') && (highStr[1] == '0')))
                {
                    if (lowStr[0] == highStr[0])
                    {
                        codedString.Append(lowStr[0]);
                        if (Lcounter > 0)
                        {
                            codedString.Append(new string(char.Parse((1 - int.Parse(lowStr[0].ToString())).ToString()), Lcounter));
                            Lcounter = 0;
                        }
                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                    }
                    else if((lowStr[1] == '1') && (highStr[1] == '0'))
                    {
                        Lcounter++;
                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        lowStr = (lowStr[0]=='1'?"0":"1") + lowStr.Substring(1, lowStr.Length - 1);
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                        highStr = (highStr[0] == '1' ? "0" : "1") + highStr.Substring(1, highStr.Length - 1);

                    }

                }
                //Console.WriteLine("Current state: ");
                //Console.WriteLine("Iteration: " + i.ToString());
                //Console.WriteLine("Coding symbol: " + source[i]);
                //Console.WriteLine("Lower bound: " + lowStr + "or " + low.ToString());
                //Console.WriteLine("Higher bound: " + highStr + "or " + high.ToString());
                probabilityIntervals = RecalculatedProbabilityIntervals(probabilityIntervals, source[i]);

            }
            var result = codedString.ToString();
            result += lowStr.Substring(1,1);//, buffCount - (result.Length % buffCount));
            if (Lcounter > 0)
            {
                result += new string('1', Lcounter); 
            }
            result += new string('1', buffCount - (result.Length % buffCount));
            return (result, probabilityIntervals);
        }

        public static string Decoding(string coded, Dictionary<char, ProbabilityInterval> probabilityIntervals, int sourceLength)
        {
            int buffCount = 30;
            int n = buffCount - 2;
            string lowStr = new string('0', buffCount);
            string highStr = new string('1', buffCount);
            StringBuilder decodedString = new StringBuilder();
            string encodedValueStr = coded.Substring(0, buffCount);
            string temp = coded;
            double low = Convert.ToInt32(lowStr, 2);
            double high = Convert.ToInt32(highStr, 2);
            int pointer = 0;
            int foundChars = 0;
            //int divNum = Convert.ToInt32(Math.Pow(2, buffCount));
            int divProb = Convert.ToInt32(Math.Pow(2, n));
            while (foundChars < sourceLength )
            {
                low = Convert.ToInt32(lowStr, 2);
                high = Convert.ToInt32(highStr, 2);
                double encodedValue = Convert.ToInt32(encodedValueStr, 2);
                double range = high - low + 1;
                double encoded = Convert.ToInt32((encodedValue - low) / (range) * divProb);
                var x = probabilityIntervals.Where(pi => pi.Value.IntervalMin <= encoded && encoded < pi.Value.IntervalMax).FirstOrDefault() ;
                decodedString.Append(x.Key);
                foundChars++;
                if (x.Value == null)
                {
                    break;
                }
                high = Math.Floor(low + range * (Convert.ToDouble(x.Value.IntervalMax) / (divProb)) - 1);
                low = Math.Floor(low + range * (Convert.ToDouble(x.Value.IntervalMin) / (divProb)));
                highStr = Convert.ToString(Convert.ToInt32(high), 2).PadLeft(buffCount, '0');
                lowStr = Convert.ToString(Convert.ToInt32(low), 2).PadLeft(buffCount, '0');
                while (((lowStr[0] == highStr[0]) || ((lowStr[1] == '1') && (highStr[1] == '0'))))
                {
                    pointer++;
                    if (coded.Substring(pointer).Length < buffCount)
                    {
                        encodedValueStr = coded.Substring(pointer).PadRight(buffCount, '0');
                    }
                    else
                    {
                        encodedValueStr = coded.Substring(pointer, buffCount);
                    }
                    if (lowStr[0] == highStr[0])
                    {
                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                    }
                    else if ((lowStr[1] == '1') && (highStr[1] == '0'))
                    {
                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        lowStr = (lowStr[0] == '1' ? "0" : "1") + lowStr.Substring(1, lowStr.Length - 1);
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                        highStr = (highStr[0] == '1' ? "0" : "1") + highStr.Substring(1, lowStr.Length - 1);
                        encodedValueStr = (encodedValueStr[0] == '1' ? "0" : "1") + encodedValueStr.Substring(1, encodedValueStr.Length - 1);
                    }
                  
                }
                //Console.WriteLine("Current state: ");
                //Console.WriteLine("Found symbol: " + x.Key);
                //Console.WriteLine("Lower bound: " + lowStr + "or " + low.ToString());
                //Console.WriteLine("Higher bound: " + highStr + "or " + high.ToString());

            }
            return decodedString.ToString();
        }

        public static Dictionary<char, ProbabilityInterval> RecalculatedProbabilityIntervals(Dictionary<char, ProbabilityInterval> old, char newSymbol)
        {
            int buffCount = 30;
            int n = buffCount - 2;
            var frequencies = new Dictionary<char, int>();
            int symbCount = 1;
            foreach (var s in old)
            {
                frequencies[s.Key] = s.Value.Frequency;
                symbCount += s.Value.Frequency;
            }
            frequencies[newSymbol]++;
            var probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            frequencies = frequencies.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            double tempValue = 0;
            foreach (var f in frequencies)
            {
                probabilityIntervals.Add(f.Key, new ProbabilityInterval()
                {
                    Symbol = f.Key,
                    Frequency = f.Value,
                    Probability = (double)f.Value / symbCount,
                    IntervalMin = Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))),
                    IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / symbCount) * Math.Pow(2, n)))),

                });
                tempValue = tempValue + (double)f.Value / symbCount;
            }
            return probabilityIntervals;
        }

        public static Dictionary<char, ProbabilityInterval> GetProbabilityIntervalsAdaptive(string source)
        {
            int buffCount = 30;
            int n = buffCount - 2;
            var frequencies = new Dictionary<char, int>();
            foreach (var s in source)
            {
                if (!frequencies.ContainsKey(s)) frequencies[s] = 1;
                //else frequencies[s]++;
            }
            var probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            frequencies = frequencies.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            //double totalSymbolCount = Convert.ToDouble(source.Length) / Math.Pow(2, 14);
            double tempValue = 0;
            foreach (var f in frequencies)
            {
                probabilityIntervals.Add(f.Key, new ProbabilityInterval()
                {
                    Symbol = f.Key,
                    Frequency = f.Value,
                    Probability =(double)f.Value / frequencies.Count,
                    IntervalMin = Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))),
                    IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / frequencies.Count) * Math.Pow(2, n)))),
                    //IntervalMin = Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))),
                    //IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2, n)))),
                    //IntervalMinBin = Convert.ToString(Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))), 2).PadLeft(buffCount, '0'),
                    //IntervalMaxBin = Convert.ToString(Convert.ToInt32(Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2, n))-1), 2).PadLeft(buffCount, '0'),
                }) ;
                tempValue = tempValue + (double)f.Value / frequencies.Count;
            }
            return probabilityIntervals;
        }

        public static Dictionary<char, ProbabilityInterval> GetProbabilityIntervals(string source)
        {
            int buffCount = 30;
            int n = buffCount - 2;
            var frequencies = new Dictionary<char, int>();
            foreach (var s in source)
            {
                if (!frequencies.ContainsKey(s)) frequencies[s] = 1;
                else frequencies[s]++;
            }
            var probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            frequencies = frequencies.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            //double totalSymbolCount = Convert.ToDouble(source.Length) / Math.Pow(2, 14);
            double tempValue = 0;
            foreach (var f in frequencies)
            {
                probabilityIntervals.Add(f.Key, new ProbabilityInterval()
                {
                    Symbol = f.Key,
                    Frequency = f.Value,
                    Probability = (double)f.Value / source.Length,
                    //IntervalMin = Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))),
                    //IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / frequencies.Count) * Math.Pow(2, n)))),
                    IntervalMin = Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))),
                    IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2, n)))),
                    //IntervalMinBin = Convert.ToString(Convert.ToInt32(Math.Round(tempValue * Math.Pow(2, n))), 2).PadLeft(buffCount, '0'),
                    //IntervalMaxBin = Convert.ToString(Convert.ToInt32(Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2, n))-1), 2).PadLeft(buffCount, '0'),
                });
                tempValue = tempValue + (double)f.Value /source.Length;
            }
            return probabilityIntervals;
        }

    }
}
