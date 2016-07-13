using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Accord.Math;
using System.Threading.Tasks;
using System.Threading;

namespace DataSetsSparsity
{
    public partial class Form1 : Form
    {
        //CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();

            //READ AND SET PROPERTIES
            u_config.readConfig(@"C:\Wavelets decomposition\config.txt");
            setfromConfig();
        }

        //PARAMS
        public static double[][] boundingBox;
        public static List<List<double>> MainGrid;
        public static string MainFolderName; //THE DIR OF THE ROOT FOLDER
        public static string[] seperator = { " ", ";", "/t", "/n", "," };
        static public bool rumPrallel;
        public static bool runRf;
        //public static bool runProoning;
        public static bool runRFPrunning;
        public static userConfig u_config = new userConfig();

        public static void printtable(List<int>[] table, string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);

            string line = "";

            for (int i = 0; i < table.Count(); i++)
            {
                line = "";
                for (int j = 0; j < table[i].Count(); j++)
                {
                    line += table[i][j].ToString() + " ";
                }
                sw.WriteLine(line);
            }

            sw.Close();
        }

        public static void printList(List<double> lst, string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);

            for (int i = 0; i < lst.Count(); i++)
            {
                sw.WriteLine(lst[i]);
            }

            sw.Close();
        }

        static public void printConstWavelets2File(List<GeoWave> decision_GeoWaveArr, string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);
            int dataDim = decision_GeoWaveArr[0].rc.dim;
            int labelDim = decision_GeoWaveArr[0].MeanValue.Count();

            //PRINT METADATA
            sw.WriteLine("dimension," + dataDim);
            sw.WriteLine("labelDimension," + labelDim);
            sw.WriteLine("StartReading");

            string line;
            for (int i = 0; i < decision_GeoWaveArr.Count; i++)
            {
                line = "";

                line = decision_GeoWaveArr[i].ID.ToString() + "; " + decision_GeoWaveArr[i].child0.ToString() + "; " + decision_GeoWaveArr[i].child1.ToString() + "; ";
                for (int j = 0; j < dataDim; j++)
                {
                    line += decision_GeoWaveArr[i].boubdingBox[0][j].ToString() + "; " + decision_GeoWaveArr[i].boubdingBox[1][j].ToString() + "; "
                        + MainGrid[j][decision_GeoWaveArr[i].boubdingBox[0][j]].ToString() + "; " + MainGrid[j][decision_GeoWaveArr[i].boubdingBox[1][j]].ToString() + "; ";
                }
                line += decision_GeoWaveArr[i].level + "; ";

                for (int j = 0; j < labelDim; j++)
                {
                    line += decision_GeoWaveArr[i].MeanValue[j].ToString() + "; ";
                }

                line += decision_GeoWaveArr[i].norm + "; " + decision_GeoWaveArr[i].parentID.ToString() + "; ";

                line += decision_GeoWaveArr[i].dimIndex.ToString() + "; " + decision_GeoWaveArr[i].MaingridValue.ToString() + "; ";//SPLITTED

                line += decision_GeoWaveArr[i].dimIndexSplitter.ToString() + "; " + decision_GeoWaveArr[i].splitValue.ToString();//SPLITTER

                sw.WriteLine(line);
            }
            sw.Close();
        }

        static public void printWaveletsProperties(List<GeoWave> decision_GeoWaveArr, string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false);
            int dataDim = decision_GeoWaveArr[0].rc.dim;
            int labelDim = decision_GeoWaveArr[0].MeanValue.Count();

            sw.WriteLine("norm, level, Npoints, dimSplit, MainGridIndexSplit", "MaingridValue");

            for (int i = 0; i < decision_GeoWaveArr.Count; i++)
            {
                sw.WriteLine(decision_GeoWaveArr[i].norm + ", " + decision_GeoWaveArr[i].level + ", " + decision_GeoWaveArr[i].pointsIdArray.Count() 
                                                         + ", " + decision_GeoWaveArr[i].dimIndex + ", " + decision_GeoWaveArr[i].Maingridindex
                                                         + ", " + decision_GeoWaveArr[i].MaingridValue);
            }

            sw.Close();
        }

        public static bool IsBoxSingular(int[][] Box, int dim)
        {
            for (int i = 0; i < dim; i++)
            {
                if (Box[1][i] < Box[0][i])
                    return true;
            }

            if (Enumerable.SequenceEqual(Box[0], Box[1]))
                return true;

            return false;
        }

        private void Run()
        {
            //SET PARAMETERS
            rumPrallel = rumPrallelCB.Checked;
            //runProoning = runPrunningCB.Checked;
            runRFPrunning = runRFPrunningCB.Checked;
            runRf = runRfCB.Checked;
            string results_path = @ResultsTB.Text;
            string db_path = @DBTB.Text + "\\";
            MainFolderName = results_path;
            if (!System.IO.Directory.Exists(MainFolderName))
                System.IO.Directory.CreateDirectory(MainFolderName);
            
            //READ DATA
            DB db = new DB();
            db.training_dt = db.getDataTable(db_path + "trainingData.txt");
            db.testing_dt = db.getDataTable(db_path + "testingData.txt");
            db.validation_dt = db.getDataTable(db_path + "ValidData.txt");
            db.training_label = db.getDataTable(db_path + "trainingLabel.txt");
            db.testing_label = db.getDataTable(db_path + "testingLabel.txt");
            db.validation_label = db.getDataTable(db_path + "ValidLabel.txt");

            //BOUNDING BOX AND GRID  
            db.DBtraining_GridIndex_dt = new long[db.training_dt.Count()][];
            for (int i = 0; i < db.training_dt.Count(); i++)
                db.DBtraining_GridIndex_dt[i] = new long[db.training_dt[i].Count()];
            
            boundingBox = db.getboundingBox(db.training_dt);
            MainGrid = db.getMainGrid(db.training_dt, boundingBox, ref db.DBtraining_GridIndex_dt);
            
            bool useCrossValidation = croosValidCB.Checked;
            List<recordConfig> recArr = new List<recordConfig>();
            int NCrossValidation = 1;
            if (useCrossValidation && !int.TryParse(croosValidTB.Text, out NCrossValidation))
                MessageBox.Show("Num of Cross validation folders wasn't provided");
            for (int j = 0; j < NCrossValidation; j++)
            {
                recordConfig rc = new recordConfig();
                rc.dim = db.training_dt[0].Count();
                rc.approxThresh = double.Parse(approxThreshTB.Text);
                rc.partitionErrType = int.Parse(partitionTypeTB.Text);
                rc.minWaveSize = int.Parse(minNodeSizeTB.Text);
                rc.rfBaggingPercent = double.Parse(bagginPercentTB.Text); // 0.6;
                rc.rfNum = int.Parse(NrfTB.Text);// k + 1;//10 + k*10;// 100 / (k + 46) * 2;// int.Parse(Math.Pow(10, k + 1).ToString());
                rc.BoundLevel = int.Parse(boundLevelTB.Text);//1024;
                if (NfeaturesrfTB.Text == "all")
                    rc.NDimsinRF = db.training_dt[0].Count();
                else if (NfeaturesrfTB.Text == "sqrt")
                    rc.NDimsinRF = (int)Math.Ceiling((Convert.ToDouble(Math.Sqrt(rc.dim))));
                else if (NfeaturesrfTB.Text == "div")
                    rc.NDimsinRF = (int)Math.Ceiling((Convert.ToDouble(rc.dim / 3)));
                else
                    rc.NDimsinRF = int.Parse(NfeaturesrfTB.Text);
                rc.split_type = int.Parse(splitTypeTB.Text); //0
                rc.NormLPTypeInEstimation = int.Parse(errTypeEstimationTB.Text);
                rc.boundDepthTree = int.Parse(boundDepthTB.Text);//1024;
                rc.CrossValidFold = j;
                recArr.Add(rc);
            }

            //CREATE DIRS
            for (int i = 0; i < recArr.Count; i++)
            {
                if (!System.IO.Directory.Exists(MainFolderName + "\\" + recArr[i].getShortName()))
                {
                    System.IO.Directory.CreateDirectory(MainFolderName + "\\" + recArr[i].getShortName());
                    StreamWriter sw = new StreamWriter(MainFolderName + "\\" + recArr[i].getShortName() + "\\record_properties.txt", false);
                    sw.WriteLine(recArr[i].getFullName());
                    sw.Close();
                    u_config.printConfig(MainFolderName + "\\config.txt", null);
                }

            }

            //SET ID ARRAY LIST
            List<int> trainingID = Enumerable.Range(0, db.training_dt.Count()).ToList();
            List<int> testingID = Enumerable.Range(0, db.testing_dt.Count()).ToList();
            List<List<int>> trainingFoldId = new List<List<int>>();
            List<List<int>> testingFoldId = new List<List<int>>();
            var ran = new Random(2);
            List<int> training_rand = trainingID.OrderBy(x => ran.Next()).ToList().GetRange(0, trainingID.Count);
            if (useCrossValidation)
                createCrossValid(NCrossValidation, training_rand, trainingFoldId, testingFoldId);

            //BOUNDING INTERVALS
            int[][] BB = new int[2][];
            BB[0] = new int[boundingBox[0].Count()];
            BB[1] = new int[boundingBox[0].Count()];
            for (int i = 0; i < boundingBox[0].Count(); i++)
            {
                BB[1][i] = MainGrid[i].Count() - 1;//set last index in each dim
            }

            for (int i = 0; i < recArr.Count; i++)
            {
                analizer Analizer = new analizer(MainFolderName + "\\" + recArr[i].getShortName(), MainGrid, db, recArr[i]);
                if (!croosValidCB.Checked)
                    Analizer.analize(trainingID, testingID, BB);
                else
                    Analizer.analize(trainingFoldId[recArr[i].CrossValidFold], testingFoldId[recArr[i].CrossValidFold], BB);//cross validation
            }        
        }

        private void btnScript_Click(object sender, EventArgs e)
        {
            set2Config();
            u_config.printConfig(@"C:\Wavelets decomposition\config.txt", null);
            Run();
            btnScript.BackColor = Color.Green;
        }

        //THE LARGEST GROUP IS TRAINING
        private void createCrossValid(int Kfolds, List<int> trainingID, List<List<int>> trainingFoldId, List<List<int>> testingFoldId)
        {
            //ADD LISTS
            for (int i = 0; i < Kfolds; i++)
            {
                trainingFoldId.Add(new List<int>());
                testingFoldId.Add(new List<int>());
            }

            int Npoints = trainingID.Count / Kfolds;
            //ADD POINTS ID
            int upper_bound = Npoints;
            int counter = -1;
            for (int i = 0; i < trainingID.Count; i++)
            {
                if (i % Npoints == 0)
                {
                    counter++;//should happen Kfolds times
                }

                for (int j = 0; j < Kfolds; j++)
                {
                    if (j == counter)
                        testingFoldId[j].Add(trainingID[i]);
                    else
                        trainingFoldId[j].Add(trainingID[i]);
                }
            }
        }

        private void setfromConfig()
        {
            if (u_config.croosValidCB == "1")
                croosValidCB.Checked = true;
            if (u_config.thresholdWaveletsCB == "1")
                thresholdWaveletsCB.Checked = true;
            if (u_config.useClassificationCB == "1")
                useClassificationCB.Checked = true;
            if (u_config.runRFPrunningCB == "1")
                runRFPrunningCB.Checked = true;
            if (u_config.runRfCB == "1")
                runRfCB.Checked = true;
            if (u_config.rumPrallelCB == "1")
                rumPrallelCB.Checked = true;
            if (u_config.saveTressCB == "1")
                saveTressCB.Checked = true;
            if (u_config.estimateFullRFCB == "1")
                estimateFullRFCB.Checked = true;
            if (u_config.estimateRF4SmoothnessAnalysis == "1")
                estimateRF4SmoothnessAnalysis.Checked = true;
            if (u_config.estimateRFwaveletsCB == "1")
                estimateRFwaveletsCB.Checked = true;
            if (u_config.BaggingWithRepCB == "1")
                useClassificationCB.Checked = true;
            croosValidTB.Text = u_config.croosValidTB;
            DBTB.Text = u_config.DBTB;
            ResultsTB.Text = u_config.ResultsTB;
            approxThreshTB.Text = u_config.approxThreshTB;
            minNodeSizeTB.Text = u_config.minNodeSizeTB;
            partitionTypeTB.Text = u_config.partitionTypeTB;
            splitTypeTB.Text = u_config.splitTypeTB;
            boundLevelTB.Text = u_config.boundLevelTB;
            errTypeEstimationTB.Text = u_config.errTypeEstimationTB;
            NrfTB.Text = u_config.NrfTB;
            NfeaturesrfTB.Text = u_config.NfeaturesrfTB;
            bagginPercentTB.Text = u_config.bagginPercentTB;
            boundDepthTB.Text = u_config.boundDepthTB;
            thresholdWaveletsTB.Text = u_config.thresholdWaveletsTB;
        }

        private void set2Config()
        {
            u_config.croosValidCB = croosValidCB.Checked ? "1" : "0";
            u_config.croosValidTB = croosValidTB.Text;
            u_config.thresholdWaveletsCB = thresholdWaveletsCB.Checked ? "1" : "0";
            u_config.thresholdWaveletsTB = thresholdWaveletsTB.Text;
            u_config.thresholdWaveletsTB = thresholdWaveletsTB.Text;
            u_config.runRFPrunningCB = runRFPrunningCB.Checked ? "1" : "0";
            u_config.runRfCB = runRfCB.Checked ? "1" : "0";
            u_config.rumPrallelCB = rumPrallelCB.Checked ? "1" : "0";
            u_config.estimateFullRFCB = estimateFullRFCB.Checked ? "1" : "0";
            u_config.DBTB = DBTB.Text;
            u_config.ResultsTB = ResultsTB.Text;
            u_config.approxThreshTB = approxThreshTB.Text;
            u_config.minNodeSizeTB = minNodeSizeTB.Text;
            u_config.partitionTypeTB = partitionTypeTB.Text;
            u_config.splitTypeTB = splitTypeTB.Text;
            u_config.boundLevelTB = boundLevelTB.Text;
            u_config.errTypeEstimationTB = errTypeEstimationTB.Text;
            u_config.NrfTB = NrfTB.Text;
            u_config.NfeaturesrfTB = NfeaturesrfTB.Text;
            u_config.bagginPercentTB = bagginPercentTB.Text;
            u_config.boundDepthTB = boundDepthTB.Text;
            u_config.saveTressCB = saveTressCB.Checked ? "1" : "0";
            u_config.estimateRF4SmoothnessAnalysis = estimateRF4SmoothnessAnalysis.Checked ? "1" : "0";
            u_config.estimateRFwaveletsCB = estimateRFwaveletsCB.Checked ? "1" : "0";
            u_config.BaggingWithRepCB = useClassificationCB.Checked ? "1" : "0";
        }

        public static void applyFor(int begin, int size, Action<int> body)
        {
            if (Form1.rumPrallel) Parallel.For(begin, size, body);
            else regularDelegateFor(begin, size, body);
        }
        
        private static void regularDelegateFor(int begin, int size, Action<int> body)
        {
            for (int i = begin; i < size; i++)
            {
                body.Invoke(i);
            }
        }

    }
}
