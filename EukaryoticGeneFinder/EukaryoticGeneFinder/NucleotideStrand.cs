using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EukaryoticGeneFinder
{
    public class NucleotideStrand
    {
        //default constructor
        public NucleotideStrand()
        {
            AACodonUsage = 0.0M;
            AAwholeUsage = 0.0M;
            start = 0;
            finish = 0;
            length = 0;
            sequence = "";
            CodonList = new List<Codon>();

        }
        //overload constructor
        public NucleotideStrand(int Start, int Finish, int Length, decimal aaCodonUsage, decimal aaWholeUsage, string Sequence, List<Codon> CodonList)
        {
            start = Start;
            finish = Finish;
            length = Length;
            AACodonUsage = aaCodonUsage;
            AAwholeUsage = AAwholeUsage;
            sequence = Sequence;
            CodonList = CodonList;
        }

        //mutator function
        public void setLength()
        {
            length = sequence.Length;
        }
        public void setAACodonUsage(DataTable dtAminoAcid)
        {
            AACodonUsage = MyMethods.calculateAACodonProbality(dtAminoAcid, CodonList);
        }
        public void setAAwholeUsage(DataTable dtAminoAcid)
        {
            AAwholeUsage = MyMethods.calculateAAwholeProbality(dtAminoAcid, CodonList);
        }

        //accessor function
        public decimal getAACodonUsage(DataTable dtAminoAcid)
        {
            AACodonUsage = MyMethods.calculateAACodonProbality(dtAminoAcid, CodonList);
            return AACodonUsage;
        }

        public decimal getAAwholeUsage(DataTable dtAminoAcid)
        {
            AAwholeUsage = MyMethods.calculateAAwholeProbality(dtAminoAcid, CodonList);
            return AAwholeUsage;
        }
        public decimal AACodonUsage { get; set; }
        public decimal AAwholeUsage { get; set; }
        public int start { get; set; }
        public int finish { get; set; }
        public int length { get; set; }
        public string sequence { get; set; }
        public List<Codon> CodonList { get; set; }
    }
}
