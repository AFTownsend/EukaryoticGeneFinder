using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EukaryoticGeneFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dtInputFiles = new DataTable();
        int SelectedIndex = -1;

        public MainWindow()
        {
            InitializeComponent();

            GridView myGridView = new GridView();

            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "INPUT_FILES";

            myGridView.Columns.Add(new GridViewColumn
            {

                Header = "FILE_NAME",
                DisplayMemberBinding = new Binding("FILE_NAME")
            });

            lvInputFiles.View = myGridView;

            dtInputFiles = MyMethods.BuildFileList(tbInputFiles.Text, dtInputFiles);
            lvInputFiles.ItemsSource = dtInputFiles.DefaultView;

        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = lvInputFiles.SelectedIndex;
            if (SelectedIndex >= 0)
            {
                DataRow row = dtInputFiles.Rows[SelectedIndex];
                string InputFileName = tbInputFiles.Text + @"\" + row["FILE_NAME"].ToString();
                string OutputFileDirectory = tbOutputFileDirectory.Text;
                string OutputFileName = OutputFileDirectory + @"\" + tbOutputFileName.Text;
                string ProgramFiles = tbInputFiles.Text;


                if (Directory.Exists(OutputFileDirectory))
                {
                    int start;
                    if (true == int.TryParse(tbStart.Text, out start))
                    {
                        int finish;
                        if (true == int.TryParse(tbFinish.Text, out finish))
                        {
                            if (start > 0 && finish < 100 && start < finish)
                            {
                                pgStatus.Value = 30;
                                lblStatus.Content = "Reading Strand.... This can take a while";
                                RunDetector(InputFileName, OutputFileName, start, finish, ProgramFiles);

                            }
                            else
                            {
                                MessageBox.Show("Start or finish parameter is outside of 0-100 range");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Finish Value is not an integer between 0-100");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Start Value is not an integer between 0-100");
                    }
                }
                else
                {
                    MessageBox.Show("Output File is invalid");
                }



            }



        }
        private void btnBlastSearch_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ensembl.org/Homo_sapiens/Tools/Blast");
        }
        private void lvInputFiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedIndex = lvInputFiles.SelectedIndex;
        }
        private void tbInputFiles_KeyUp(object sender, KeyEventArgs e)
        {
            dtInputFiles = MyMethods.BuildFileList(tbInputFiles.Text, dtInputFiles);
            lvInputFiles.ItemsSource = dtInputFiles.DefaultView;
        }
        private void RunDetector(string inputFileName, string outputFileName, int PercentageStart, int PercentageFinish, string ProgramFiles)
        {

            string fileDirectory = ProgramFiles + @"\" + "AminoAcidProbTable.txt";

            //Getting DNA Strand
            string[] geneStrandArray = File.ReadAllLines(inputFileName);


            //Getting Codon usage values
            DataTable dtAminoAcid = MyMethods.buildAminoAcidDataTable(fileDirectory);

            int start = (geneStrandArray.Length / 100) * PercentageStart;
            int finish = (geneStrandArray.Length / 100) * PercentageFinish;

            //Calculating strand length
            decimal percentageCoverage = Convert.ToDecimal(PercentageFinish - PercentageStart);
            int strandLength = Convert.ToInt32(((Convert.ToDecimal(geneStrandArray.Length) * percentageCoverage) / 100));


            //Reading gene
            List<Orf> OrfList = new List<Orf>();

            int numberOfWindows = strandLength / 1000;

            int lineCount = start;
            int lineLimit = start + 1000;

            //Strands are normally too long to be analysed in one go so are broken down into windows
            Statistics myStatistics = new Statistics();
            for (int windowCount = 0; windowCount < numberOfWindows; windowCount++)
            {

                NucleotideStrand strand = new NucleotideStrand();
                strand.sequence = "";


                while (lineCount < lineLimit && lineCount < geneStrandArray.Length && lineCount < (start + strandLength))
                {


                    strand.sequence = strand.sequence + geneStrandArray[lineCount];
                    lineCount++;
                }
                strand.sequence = strand.sequence.Replace("\r\n", "");
                strand.sequence = strand.sequence.ToUpper();
                strand.length = strand.sequence.Length;

                if (windowCount == 0)
                {
                    MessageBox.Show("Strand approximately " + (strand.length * numberOfWindows).ToString() + "bp long");
                    //Calculating statistical ranges
                    myStatistics = MyMethods.calculateStatistics(strand, dtAminoAcid);
                }


                //Reading strand
                OrfList.AddRange(MyMethods.findOrf(strand, dtAminoAcid, myStatistics));

                //-100 so thee is a slight overlap with windows
                lineCount = lineCount - 100;
                lineLimit = lineLimit + 1000;
            }


            //Ordering the list of OrfS by AACodonUsage and writing the top values to file
            OrfList = OrfList.OrderByDescending(Orf => Orf.AACodonUsage).ToList();

            string foundGenes = "";

            //Building string of found genes and writing to file only want the top 10
            for (int x = 0; (x < OrfList.Count) && (x <= 9); x++)
            {
                foundGenes = foundGenes + "<" + OrfList[x].getWholeSequence() + "> \r\n \r\n";
            }


            using (StreamWriter file = new StreamWriter(outputFileName))
            {
                file.WriteLine(foundGenes);
            }

            pgStatus.Value = 100;
            lblStatus.Content = "Finished!";
            MessageBox.Show("Program Finished file created: " + outputFileName);

        }

    }
}