using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EukaryoticGeneFinder
{
    public class Codon : NucleotideStrand
    {

        //default constructor
        public Codon()
        {
            sequence = "XXX";
            name = "NONE";
            AACodonUsage = 0.0M;

        }
        //overload constructor
        public Codon(string inputSequence, string inputName, decimal inputProb)
        {
            sequence = inputSequence;
            name = inputName;
            AACodonUsage = inputProb;
        }

        public string name { get; set; }


    }
}
