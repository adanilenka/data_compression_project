using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArithmeticCoding
{
    public class ProbabilityInterval
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public double Probability { get; set; }
        public int IntervalMin { get; set; }
        public int IntervalMax { get; set; }
        public string IntervalMinBin { get; set; }
        public string IntervalMaxBin { get; set; }
    }
}
