using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EukaryoticGeneFinder
{
    class MyMethods
    {
        public static DataTable BuildFileList(string FileDirectory, DataTable dtInput)
        {
            if (Directory.Exists(FileDirectory))
            {
                DataTable dtOutput = new DataTable();

                dtOutput.Columns.Add(new DataColumn
                {
                    ColumnName = "FILE_NAME"
                });

                DirectoryInfo di = new DirectoryInfo(FileDirectory);
                FileInfo[] rgFiles = di.GetFiles("*");
                foreach (FileInfo fi in rgFiles)
                {
                    DataRow row = dtOutput.NewRow();
                    row["FILE_NAME"] = fi.Name;
                    dtOutput.Rows.Add(row);
                }

                return dtOutput;
            }
            else
            {
                return dtInput;
            }

        }

        //Contains Codon usage table
        public static DataTable buildAminoAcidDataTable(string fileDirectory)
        {
            string[] fileContent = File.ReadAllLines(fileDirectory, Encoding.UTF8);//.Split('\t').ToArray();

            DataTable dtAminoAcid = new DataTable("dtAminoAcid");

            //Creating primary key
            DataColumn primaryColumn;

            DataColumn[] keys = new DataColumn[1];

            primaryColumn = new DataColumn();
            primaryColumn.DataType = System.Type.GetType("System.String");
            primaryColumn.ColumnName = "Code";
            // Add the column to the DataTable.Columns collection.
            dtAminoAcid.Columns.Add(primaryColumn);

            // Add the column to the array.
            keys[0] = primaryColumn;
            dtAminoAcid.PrimaryKey = keys;

            //Adding the rest of the columns            
            dtAminoAcid.Columns.Add("Name", typeof(string));
            dtAminoAcid.Columns.Add("AACodonUsage", typeof(decimal));
            dtAminoAcid.Columns.Add("AAwholeUsage", typeof(decimal));

            int arrayLength = fileContent.Length - 1;
            int arrayWidth = fileContent[0].Split('\t').ToArray().Length;
            decimal[,] dataArray = new decimal[arrayLength, arrayWidth];
            int counter = arrayWidth;
            for (int i = 0; i < arrayLength; i++)
            {
                DataRow currRow;
                currRow = dtAminoAcid.NewRow();
                string[] currArray = fileContent[i + 1].Split('\t').ToArray();
                currRow[0] = currArray[0].ToString();
                currRow[1] = currArray[1].ToString();
                currRow[2] = Convert.ToDecimal(currArray[2]);
                currRow[3] = Convert.ToDecimal(currArray[3]);
                dtAminoAcid.Rows.Add(currRow);
            }

            return dtAminoAcid;
        }
        //Generates a random strand of DNA to compare to
        public static string generateRandomDNA(int minlength, int maxlength)
        {
            string strand = "";

            Random rndLength = new Random();
            Random rndBase = new Random();
            int length = rndLength.Next(minlength, maxlength);
            string[] baseArray = { "A", "T", "C", "G" };
            for (int i = 0; i < length; i++)
            {
                int x = rndBase.Next(0, 4);
                strand = strand + baseArray[x];
            }


            return strand;
        }
        //Calculates Codon usage 
        public static decimal calculateAACodonProbality(DataTable dtAminoAcid, List<Codon> CodonList)
        {
            string name;
            string code;
            decimal predicted;
            decimal actual;
            decimal aminoAcidCount;
            decimal CodonCount;
            decimal AACodonUsage;
            decimal totalAACodonUsage = 0;

            for (int i = 0; i < dtAminoAcid.Rows.Count; i++)
            {
                DataRow currRow = dtAminoAcid.Rows[i];
                code = currRow[0].ToString();
                name = currRow[1].ToString();
                predicted = Convert.ToDecimal(currRow[2]);
                aminoAcidCount = CodonList.Count(item => item.name == name);
                CodonCount = CodonList.Count(item => item.sequence == code);
                if (CodonCount > 0)
                {
                    actual = (CodonCount / aminoAcidCount) * 100;
                    AACodonUsage = Math.Abs(predicted - actual);
                    AACodonUsage = 100 - AACodonUsage;
                    totalAACodonUsage = totalAACodonUsage + AACodonUsage;
                }


            }
            totalAACodonUsage = totalAACodonUsage / dtAminoAcid.Rows.Count;

            return totalAACodonUsage;
        }
        //Calculates amino acid usage
        public static decimal calculateAAwholeProbality(DataTable dtAminoAcid, List<Codon> CodonList)
        {
            string code;
            decimal predicted;
            decimal actual;
            decimal CodonCount;
            decimal AAwholeUsage;
            decimal totalAAwholeUsage = 0;

            for (int i = 0; i < dtAminoAcid.Rows.Count; i++)
            {
                DataRow currRow = dtAminoAcid.Rows[i];
                code = currRow[0].ToString();
                predicted = Convert.ToDecimal(currRow[3]);

                CodonCount = CodonList.Count(item => item.sequence == code);
                if (CodonCount > 0)
                {
                    actual = (CodonCount / CodonList.Count) * 1000;
                    AAwholeUsage = Math.Abs(predicted - actual);
                    AAwholeUsage = 100 - AAwholeUsage;
                    totalAAwholeUsage = totalAAwholeUsage + AAwholeUsage;
                }


            }
            totalAAwholeUsage = totalAAwholeUsage / dtAminoAcid.Rows.Count;

            return totalAAwholeUsage;
        }
        //Compares input strand to see how similar it is to the end of an Intron
        public static string buildIntronFinishSeq(string strand)
        {
            strand = strand.Replace("C", "P");
            strand = strand.Replace("T", "P");
            strand = strand.Remove(8, 1);
            strand = strand.Insert(8, "X");
            return strand;
        }
        //Compares input strand to see how similar to a branch point
        public static string buildBranchPoint(string strand)
        {
            string currLetter;
            int currPosition;

            //0
            currPosition = 0;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "C") | (currLetter == "T"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());

            }
            //1
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if (true == true)
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
            }
            //2
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "C") | (currLetter == "T"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }
            //3
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "C") | (currLetter == "T"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }
            //4
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "A") | (currLetter == "G") | (currLetter == "C"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }
            //5
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "A") | (currLetter == "T"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }
            //6
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "C") | (currLetter == "T"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }
            //7
            currPosition++;
            currLetter = strand.Substring(currPosition);
            if ((currLetter == "T") | (currLetter == "C"))
            {
                strand = strand.Insert(currPosition, currPosition.ToString());
                currPosition++;
            }

            return strand;
        }
        //Creates stats like average Orf length, Codon usage when using a random strand to compare to
        public static Statistics calculateStatistics(NucleotideStrand experimentalStrand, DataTable dtAminoAcid)
        {
            NucleotideStrand strand = new NucleotideStrand();
            strand.sequence = generateRandomDNA(experimentalStrand.length, experimentalStrand.length);

            int OrfCount = 1;
            strand.setLength();
            List<Orf> OrfList = new List<Orf>();

            //Reading Strand searching for Orf
            for (int i = 0; i < (strand.length - 3); i++)
            {

                //Setting variables             
                string srchCode = strand.sequence.Substring(i, 3);
                DataRow srchRow = dtAminoAcid.Rows.Find(srchCode);

                Codon srchCodon = new Codon(srchRow[0].ToString(), srchRow[1].ToString(), Convert.ToDecimal(srchRow[2]));

                //If start Codon is found the strand is translate until stop Codon reached    
                if ("Met" == srchCodon.name)
                {


                    Codon currCodon = srchCodon;
                    int b = i;
                    Orf currOrf = new Orf();
                    currOrf.start = b;

                    Exon currExon = new Exon();
                    currExon.start = b;


                    while ((b < (strand.length - 12)) && (currCodon.name != "Stp"))
                    {


                        string currCode = strand.sequence.Substring(b, 3);
                        DataRow currRow = dtAminoAcid.Rows.Find(currCode);
                        currCodon = new Codon(currRow[0].ToString(), currRow[1].ToString(), Convert.ToDecimal(currRow[2]));


                        currOrf.CodonList.Add(currCodon);
                        currExon.CodonList.Add(currCodon);

                        //Detecting Intron
                        if ("AGGT" == strand.sequence.Substring(b + 1, 4))
                        {
                            //Finish Exon
                            currExon.finish = b;
                            currExon.length = currExon.finish - currExon.start;
                            currOrf.ExonList.Add(currExon);

                            //Starting Intron
                            Intron currIntron = new Intron();
                            currIntron.start = b;

                            //Reading Intron
                            string IntronStr = strand.sequence.Substring(b, 8);
                            IntronStr = MyMethods.buildBranchPoint(IntronStr);



                            while ((b < (strand.length - 8)) && (IntronStr == "01234567"))
                            {
                                IntronStr = strand.sequence.Substring(b, 8);
                                IntronStr = MyMethods.buildBranchPoint(IntronStr);
                                b++;
                            }


                            //Converts CGs at end of strand into pyrimidines for comparison because the end of the strand contains alway contains a chain pyrimidines
                            IntronStr = strand.sequence.Substring(b, 12);
                            IntronStr = MyMethods.buildIntronFinishSeq(IntronStr);
                            while ((b < (strand.length - 12)) && ("PPPPPPPPXPAG" != IntronStr))
                            {
                                IntronStr = strand.sequence.Substring(b, 12);
                                IntronStr = MyMethods.buildIntronFinishSeq(IntronStr);
                                b++;
                            }

                            b = b + 12;

                            //Finishing Intron
                            currIntron.finish = b;
                            currIntron.length = currIntron.finish - currIntron.start;
                            currIntron.sequence = strand.sequence.Substring(currIntron.start, currIntron.length);
                            currOrf.IntronList.Add(currIntron);
                            //Starting new Exon
                            currExon = new Exon();
                            currExon.start = b;



                        }

                        //When stop Codon is found Orf is assessed as to how probable it is that it is a gene
                        if ("Stp" == currCodon.name)
                        {

                            //Finish Exon
                            currExon.finish = b;
                            currExon.length = currExon.finish - currExon.start;
                            currOrf.ExonList.Add(currExon);

                            //Finish Orf
                            currOrf.finish = b;
                            currOrf.length = currOrf.finish - currOrf.start;
                            int bpCount = currOrf.ExonList.Sum(item => item.length);



                            currOrf.setAACodonUsage(dtAminoAcid);
                            currOrf.setAAwholeUsage(dtAminoAcid);
                            currOrf.ExonLength = currOrf.ExonList.Sum(Exon => Exon.length);

                            int currVal = currOrf.finish;
                            bool myCheck = OrfList.Any(Orf => Orf.finish == currOrf.finish);

                            if (myCheck == false)
                            {
                                OrfList.Add(currOrf);
                            }

                            OrfCount++;

                        }


                        b = b + 3;

                    }

                }

            }


            Statistics myStatistics = new Statistics();

            double meanExonLength = (OrfList.Sum(Orf => Orf.ExonLength)) / OrfList.Count;
            double StanDevExonLength = OrfList.Sum(Orf => Math.Pow(Orf.ExonLength - meanExonLength, 2));
            StanDevExonLength = Math.Sqrt((StanDevExonLength) / OrfList.Count);
            myStatistics.meanExonLength = meanExonLength;
            myStatistics.StanDevExonLength = StanDevExonLength;
            myStatistics.rangeExonLength = myStatistics.meanExonLength + myStatistics.StanDevExonLength;

            double meanWholeLength = (OrfList.Sum(Orf => Orf.length)) / OrfList.Count;
            double StanDevWholeLength = OrfList.Sum(Orf => Math.Pow(Orf.length - meanWholeLength, 2));
            StanDevWholeLength = Math.Sqrt((StanDevWholeLength) / OrfList.Count);
            myStatistics.meanLength = meanWholeLength;
            myStatistics.StanDevLength = StanDevWholeLength;
            myStatistics.rangeLength = myStatistics.meanLength + myStatistics.StanDevLength;


            double MeanAACodonUsage = Convert.ToDouble((OrfList.Sum(Orf => Orf.AACodonUsage)) / OrfList.Count);
            double StanDevAACodonUsage = OrfList.Sum(Orf => Math.Pow(Convert.ToDouble(Orf.AACodonUsage) - MeanAACodonUsage, 2));
            StanDevAACodonUsage = Math.Sqrt((StanDevAACodonUsage) / OrfList.Count);
            myStatistics.MeanAACodonUsage = MeanAACodonUsage;
            myStatistics.StanDevAACodonUsage = StanDevAACodonUsage;
            myStatistics.RangeAACodonUsage = myStatistics.MeanAACodonUsage + myStatistics.StanDevAACodonUsage;

            double MeanAAwholeUsage = Convert.ToDouble((OrfList.Sum(Orf => Orf.AAwholeUsage)) / OrfList.Count);
            double StanDevAAwholeUsage = OrfList.Sum(Orf => Math.Pow(Convert.ToDouble(Orf.AAwholeUsage) - MeanAAwholeUsage, 2));
            StanDevAAwholeUsage = Math.Sqrt((StanDevAAwholeUsage) / OrfList.Count);
            myStatistics.MeanAAwholeUsage = MeanAAwholeUsage;
            myStatistics.StanDevAAwholeUsage = StanDevAAwholeUsage;
            myStatistics.RangeAAwholeUsage = myStatistics.MeanAAwholeUsage + myStatistics.StanDevAAwholeUsage;

            return myStatistics;
        }
        //Looks for Orfs
        public static List<Orf> findOrf(NucleotideStrand strand, DataTable dtAminoAcid, Statistics currStat)
        {


            int OrfCount = 1;
            strand.setLength();
            List<Orf> OrfList = new List<Orf>();
            //Reading Strand searching for Orf
            for (int i = 0; i < (strand.length - 3); i++)
            {

                //Setting variables
                //DNA is broken into 3 character strings because that is how the body reads DNA. A 3 segment strand is called Codon            
                string srchCode = strand.sequence.Substring(i, 3);
                DataRow srchRow = dtAminoAcid.Rows.Find(srchCode);

                Codon srchCodon = new Codon(srchRow[0].ToString(), srchRow[1].ToString(), Convert.ToDecimal(srchRow[2]));

                //If start Codon is found the strand is translate until stop Codon reached    
                if ("Met" == srchCodon.name)
                {


                    Codon currCodon = srchCodon;
                    //b just symbols the current position in the strand 
                    int b = i;
                    Orf currOrf = new Orf();
                    currOrf.start = b;

                    Exon currExon = new Exon();
                    currExon.start = b;


                    //Orfs can be be broken down into Exons and Introns, Introns don't follow the same design pattern as Exons
                    //Therefore to properly assess a gene the Exons and intros have to be seperated out
                    while ((b < (strand.length - 12)) && (currCodon.name != "Stp"))
                    {


                        string currCode = strand.sequence.Substring(b, 3);
                        DataRow currRow = dtAminoAcid.Rows.Find(currCode);
                        currCodon = new Codon(currRow[0].ToString(), currRow[1].ToString(), Convert.ToDecimal(currRow[2]));


                        currOrf.CodonList.Add(currCodon);
                        currExon.CodonList.Add(currCodon);

                        //Detecting Intron (AGGT indicates the start of an Intron)
                        if ("AGGT" == strand.sequence.Substring(b + 1, 4))
                        {
                            //Finish Exon
                            currExon.finish = b;
                            currExon.length = currExon.finish - currExon.start;
                            currOrf.ExonList.Add(currExon);

                            //Starting Intron
                            Intron currIntron = new Intron();
                            currIntron.start = b;

                            //Reading Intron
                            //A charactertic of an Intron is that it has a branch point in the middle so before
                            //The Intron has ended the branch must be found
                            string IntronStr = strand.sequence.Substring(b, 8);

                            //buildBranchPoint just put the current Intron string into the format 01234567 if each character meets
                            // The criteria of an Intron
                            IntronStr = MyMethods.buildBranchPoint(IntronStr);
                            while ((b < (strand.length - 8)) && (IntronStr == "01234567"))
                            {
                                IntronStr = strand.sequence.Substring(b, 8);
                                IntronStr = MyMethods.buildBranchPoint(IntronStr);
                                b++;
                            }


                            //Converts CGs at end of strand into pyrimidines for comparison because the end of the strand contains alway contains a chain pyrimidines
                            IntronStr = strand.sequence.Substring(b, 12);
                            IntronStr = MyMethods.buildIntronFinishSeq(IntronStr);
                            while ((b < (strand.length - 12)) && ("PPPPPPPPXPAG" != IntronStr))
                            {
                                IntronStr = strand.sequence.Substring(b, 12);
                                IntronStr = MyMethods.buildIntronFinishSeq(IntronStr);
                                b++;
                            }

                            b = b + 12;

                            //Finishing Intron
                            currIntron.finish = b;
                            currIntron.length = currIntron.finish - currIntron.start;
                            currIntron.sequence = strand.sequence.Substring(currIntron.start, currIntron.length);
                            currOrf.IntronList.Add(currIntron);
                            //Starting new Exon
                            currExon = new Exon();
                            currExon.start = b;



                        }

                        //When stop Codon is found Orf is assessed as to how probable it is that it is a gene
                        if ("Stp" == currCodon.name)
                        {

                            //Finish Exon
                            currExon.finish = b;
                            currExon.length = currExon.finish - currExon.start;
                            currOrf.ExonList.Add(currExon);

                            //Finish Orf
                            currOrf.finish = b;
                            currOrf.length = currOrf.finish - currOrf.start;
                            currOrf.ExonLength = currOrf.ExonList.Sum(Exon => Exon.length);
                            int bpCount = currOrf.ExonList.Sum(item => item.length);

                            //This calculates the Codon usage relative to what would be expected in a living organism
                            currOrf.setAACodonUsage(dtAminoAcid);
                            currOrf.setAAwholeUsage(dtAminoAcid);

                            //if the current found Orf has properties greater than the statistical ranges it will be added to the the list of OrfS
                            if ((currOrf.ExonLength > currStat.rangeExonLength) && (Convert.ToDouble(currOrf.AACodonUsage) > currStat.RangeAACodonUsage) && (Convert.ToDouble(currOrf.AAwholeUsage) > currStat.RangeAAwholeUsage) && (currOrf.length > currStat.rangeLength))
                            {

                                bool myCheck = OrfList.Any(Orf => Orf.finish == currOrf.finish);
                                if (myCheck == false)
                                {
                                    OrfList.Add(currOrf);
                                }


                            }



                            OrfCount++;

                        }


                        b = b + 3;

                    }

                }

            }



            return OrfList;
        }

    }
}
