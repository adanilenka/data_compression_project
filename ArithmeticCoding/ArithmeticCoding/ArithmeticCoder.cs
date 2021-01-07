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
            int buffCount =8;
            string lowStr = new string('0', buffCount);
            string highStr = new string('1', buffCount);
            var probabilityIntervals = GetProbabilityIntervals(source);
            //probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            //probabilityIntervals['1'] = new ProbabilityInterval()
            //{
            //    IntervalMin = 0,
            //    IntervalMax = 40,
            //    Symbol = '1'
            //};
            //probabilityIntervals['2'] = new ProbabilityInterval()
            //{
            //    IntervalMin = 40,
            //    IntervalMax = 41,
            //    Symbol = '2'
            //};
            //probabilityIntervals['3'] = new ProbabilityInterval()
            //{
            //    IntervalMin = 41,
            //    IntervalMax = 50,
            //    Symbol = '3'
            //};

            StringBuilder codedString = new StringBuilder();
            int Lcounter = 0;
            for (int i = 0; i < source.Length; i++)
            {
                double low = Convert.ToInt32(lowStr, 2);
                double high = Convert.ToInt32(highStr, 2);
                double range = high - low + 1;

                high =Math.Floor(low +  range * (Convert.ToDouble(probabilityIntervals[source[i]].IntervalMax)/(source.Length * Math.Pow(2, buffCount)))) -1;
                low = Math.Floor(low + range * (Convert.ToDouble(probabilityIntervals[source[i]].IntervalMin )/( source.Length *Math.Pow(2, buffCount))));
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
               
            }
            var result = codedString.ToString();
            result += new string('0', buffCount - (result.Length % buffCount));
            if (Lcounter > 0)
            {
                result += new string('1', Lcounter) + new string('0',buffCount-Lcounter); 
            }
            return (result, probabilityIntervals);
        }

        public static string Decoding(string coded, Dictionary<char, ProbabilityInterval> probabilityIntervals, int sourceLength)
        {
            int buffCount = 8;
            string lowStr = new string('0', buffCount);
            string highStr = new string('1', buffCount);
            StringBuilder decodedString = new StringBuilder();
            string encodedValueStr = coded.Substring(0, buffCount);
            string temp = coded;
            double low = Convert.ToInt32(lowStr, 2);
            double high = Convert.ToInt32(highStr, 2);
            int pointer = 0;
            while (pointer<=coded.Length-buffCount )
            {
                low = Convert.ToInt32(lowStr, 2);
                high = Convert.ToInt32(highStr, 2);
                double encodedValue = Convert.ToInt32(encodedValueStr, 2);
                double range = high - low + 1;
                double encoded = Convert.ToInt32((encodedValue - low) / (range) * sourceLength);
                var x = probabilityIntervals.Where(pi => pi.Value.IntervalMin <= encoded && encoded < pi.Value.IntervalMax).FirstOrDefault() ;
                decodedString.Append(x.Key);
                high = Math.Floor(low + range * (Convert.ToDouble(x.Value.IntervalMax) / (sourceLength * Math.Pow(2, buffCount))) - 1);
                low = Math.Floor(low + range * (Convert.ToDouble(x.Value.IntervalMin) / (sourceLength * Math.Pow(2, buffCount))));
                highStr = Convert.ToString(Convert.ToInt32(high), 2).PadLeft(buffCount, '0');
                lowStr = Convert.ToString(Convert.ToInt32(low), 2).PadLeft(buffCount, '0');
                while (((lowStr[0] == highStr[0]) || ((lowStr[1] == '1') && (highStr[1] == '0'))))
                {
                    if (lowStr[0] == highStr[0])
                    {

                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                        pointer++;
                        encodedValueStr = coded.Substring(pointer, buffCount);
                    }
                    else if ((lowStr[1] == '1') && (highStr[1] == '0'))
                    {
                        lowStr = lowStr.Substring(1, lowStr.Length - 1) + "0";
                        lowStr = (lowStr[0] == '1' ? "0" : "1") + lowStr.Substring(1, lowStr.Length - 1);
                        highStr = highStr.Substring(1, highStr.Length - 1) + "1";
                        highStr = (highStr[0] == '1' ? "0" : "1") + highStr.Substring(1, lowStr.Length - 1);
                        pointer++;
                        encodedValueStr = coded.Substring(pointer, buffCount);
                        encodedValueStr = (encodedValueStr[0] == '1' ? "0" : "1") + encodedValueStr.Substring(1, encodedValueStr.Length - 1);
                    }
                  
                }


            }
            return decodedString.ToString();
        }


        public static Dictionary<char, ProbabilityInterval> GetProbabilityIntervals(string source)
        {
            int buffCount = 8;
            var frequencies = new Dictionary<char, int>();
            foreach (var s in source)
            {
                if (!frequencies.ContainsKey(s)) frequencies[s] = 1;
                else frequencies[s]++;
            }
            var probabilityIntervals = new Dictionary<char, ProbabilityInterval>();
            //frequencies = frequencies.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            //double totalSymbolCount = Convert.ToDouble(source.Length) / Math.Pow(2, 14);
            
            double tempValue = 0;
            foreach (var f in frequencies)
            {
                probabilityIntervals.Add(f.Key, new ProbabilityInterval()
                {
                    Symbol = f.Key,
                    Frequency = f.Value,
                    Probability =(double)f.Value /source.Length,
                    IntervalMin = Convert.ToInt32(Math.Round(tempValue*Math.Pow(2,buffCount))),
                    IntervalMax = Convert.ToInt32((Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2,buffCount)))),
                    IntervalMinBin = Convert.ToString(Convert.ToInt32(Math.Round(tempValue * Math.Pow(2,buffCount))), 2).PadLeft(buffCount, '0'),
                    IntervalMaxBin = Convert.ToString(Convert.ToInt32(Math.Round((tempValue + (double)f.Value / source.Length) * Math.Pow(2,buffCount))-1), 2).PadLeft(buffCount, '0'),
                }) ;
                tempValue = tempValue + (double)f.Value / source.Length;
            }
            return probabilityIntervals;
        }

    }
}
