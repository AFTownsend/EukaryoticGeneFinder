using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EukaryoticGeneFinder
{
    public class Orf : NucleotideStrand
    {
        //Default Constructor
        public Orf()
        {
            AACodonUsage = 0.0M;
            ExonLength = 0;
            ExonList = new List<Exon>();
            IntronList = new List<Intron>();
        }

        //Accessor Function
        public string getWholeSequence()
        {
            string wholeStrand = "";

            for (int i = 0; i < ExonList.Count; i++)
            {
                for (int b = 0; b < ExonList[i].CodonList.Count; b++)
                {
                    wholeStrand = wholeStrand + ExonList[i].CodonList[b].sequence;
                }

                if (0 < IntronList.Count && i < IntronList.Count)
                {
                    for (int b = 0; b < ExonList[i].CodonList.Count; b++)
                    {
                        wholeStrand = wholeStrand + IntronList[i].sequence;
                    }
                }
            }
            return wholeStrand;
        }

        public int ExonLength { get; set; }
        public int numberExons { get; set; }
        public List<Exon> ExonList { get; set; }
        public List<Intron> IntronList { get; set; }

    }
}
