using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EukaryoticGeneFinder
{
    public class Statistics
    {
        public double MeanAACodonUsage { get; set; }
        public double StanDevAACodonUsage { get; set; }
        public double RangeAACodonUsage { get; set; }
        public double MeanAAwholeUsage { get; set; }
        public double StanDevAAwholeUsage { get; set; }
        public double RangeAAwholeUsage { get; set; }
        public double meanExonLength { get; set; }
        public double StanDevExonLength { get; set; }
        public double rangeExonLength { get; set; }
        public double meanLength { get; set; }
        public double StanDevLength { get; set; }
        public double rangeLength { get; set; }
    }
}
