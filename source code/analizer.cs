using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Amazon.S3.IO;
//using Accord.MachineLearning;
//using Accord.Math;
//using Accord.Math.Decompositions;
//using Accord.Statistics;
//using Accord.Statistics.Analysis;

namespace DataSetsSparsity
{
    class analizer
    {
        private string analysisFolderName;
        private List<List<double>> MainGrid;
        private DB db;
        private recordConfig rc;
        public static double[][][] resultsByTree;
        public static double[] resultsByForest;
        public static double MidPoint;

        public analizer(string analysisFolderName, List<List<double>> MainGrid, DB db, recordConfig rc)
        {
            this.analysisFolderName = analysisFolderName;
            this.MainGrid = MainGrid;
            this.db = db;
            this.rc = rc;
        }

        public void analize(List<int> trainingArr, List<int> testingArr, int[][] boundingBox)
        {
           #region RF tree 

            int tmp_N_rows = Convert.ToInt32(trainingArr.Count * rc.rfBaggingPercent);
            List<int>[] trainingArrRF_indecesList = new List<int>[rc.rfNum];

            if (Form1.runRf)
            {
                //create RF
                List<GeoWave>[] RFdecTreeArr = new List<GeoWave>[rc.rfNum];
                Form1.applyFor(0, rc.rfNum, i =>
                {
                    List<int> trainingArrRF;
                    if (Form1.u_config.BaggingWithRepCB == "1")
                        trainingArrRF = BaggingBreiman(trainingArr, i);
                    else
                        trainingArrRF = Bagging(trainingArr, rc.rfBaggingPercent, i);

                    trainingArrRF_indecesList[i] = trainingArrRF;
                    bool[] Dim2Take = getDim2Take(rc, i);
                    decicionTree decTreeRF = new decicionTree(rc, db, Dim2Take);
                    RFdecTreeArr[i] = decTreeRF.getdecicionTree(trainingArrRF, boundingBox, i);
                });


                if (Form1.u_config.saveTressCB == "1")
                {
                    if (!System.IO.Directory.Exists(analysisFolderName + "\\archive")) 
                        System.IO.Directory.CreateDirectory(analysisFolderName + "\\archive");
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                    {
                        Form1.printWaveletsProperties(RFdecTreeArr[i], analysisFolderName + "\\archive\\waveletsPropertiesTree_" + i.ToString() + ".txt");
                        //Form1.printConstWavelets2File(RFdecTreeArr[i], analysisFolderName + "\\archive\\RFdecTreeArr_" + i.ToString() + "_tree.txt");//dbg
                    }
                }


                //PREPARE FOR TESTING
                if (rc.NormLPTypeInEstimation == 0 || Form1.u_config.useClassificationCB == "1")
                {
                    rc.NormLPTypeInEstimation = 0;
                    setMidpoint(db.validation_label);
                }
                    

                //set norms
                List<List<double>> norms = new List<List<double>>();
                for (int i = 0; i < RFdecTreeArr.Count(); i++)
                {
                    List<double> tmp = new List<double>();
                    norms.Add(tmp);
                    for (int j = 0; j < RFdecTreeArr[i].Count; j++)
                        norms[i].Add(RFdecTreeArr[i][j].norm);
                }

                //SORT each tree BY ID 
                Parallel.For(0, RFdecTreeArr.Count(), i =>
                {
                    RFdecTreeArr[i] = RFdecTreeArr[i].OrderBy(o => o.ID).ToList();
                });

                //IF TEST FULL RF  
                if (Form1.u_config.estimateFullRFCB == "1")
                {
                    double[] tmperrorRyByForest = new double[RFdecTreeArr.Count()];
                    tmperrorRyByForest = testDecisionTreeManyRFnew(testingArr, db.validation_dt, db.validation_label, RFdecTreeArr, 0.0, rc.NormLPTypeInEstimation);
                    List<double> tmpNwaveletsInRF = new List<double>();
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                        tmpNwaveletsInRF.Add(RFdecTreeArr[i].Count());

                    Form1.printList(tmperrorRyByForest.ToList(), analysisFolderName + "\\errorByForest.txt");
                    Form1.printList(tmpNwaveletsInRF, analysisFolderName + "\\NwaveletsInRF.txt");
                }

                //IF TEST RF WITH THRESHOLD ON WAVELETS NORM
                if (Form1.u_config.thresholdWaveletsCB == "1")
                {
                    double[] tmperrorRyByForest = new double[RFdecTreeArr.Count()];
                    tmperrorRyByForest = testDecisionTreeManyRFnew(testingArr, db.validation_dt, db.validation_label, RFdecTreeArr, double.Parse(Form1.u_config.thresholdWaveletsTB), rc.NormLPTypeInEstimation);
                    List<double> tmpNwaveletsInRF = new List<double>();
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                        tmpNwaveletsInRF.Add(RFdecTreeArr[i].Count());

                    Form1.printList(tmperrorRyByForest.ToList(), analysisFolderName + "\\errorByForestWithThreshold" + Form1.u_config.thresholdWaveletsTB + ".txt");
                    Form1.printList(tmpNwaveletsInRF, analysisFolderName + "\\NwaveletsInRFWithThreshold" + Form1.u_config.thresholdWaveletsTB + ".txt");
                }

                //PRUNEFOREST
                if (Form1.runRFPrunning)
                {
                    List<int> order2Remove = reorderTreesByContribution(RFdecTreeArr.ToList(), "AVG", trainingArr, db.training_dt, db.training_label);
                    List<GeoWave>[] tmpRFdecTreeArrAVG = new List<GeoWave>[RFdecTreeArr.Count()];
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                        tmpRFdecTreeArrAVG[i] = RFdecTreeArr[order2Remove[order2Remove.Count() - 1 - i]];
                    order2Remove.Clear();
                    order2Remove = reorderTreesByContribution(RFdecTreeArr.ToList(), "MIN", trainingArr, db.training_dt, db.training_label);
                    List<GeoWave>[] tmpRFdecTreeArrMIN = new List<GeoWave>[RFdecTreeArr.Count()];
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                        tmpRFdecTreeArrMIN[i] = RFdecTreeArr[order2Remove[order2Remove.Count() - 1 - i]];

                    double[] errorRyByForest = new double[RFdecTreeArr.Count()];
                    List<double> NwaveletsInRF = new List<double>();
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                        NwaveletsInRF.Add(RFdecTreeArr[i].Count());

                    //PRUNING AVG                    
                    errorRyByForest = testDecisionTreeManyRFnew(testingArr, db.validation_dt, db.validation_label, tmpRFdecTreeArrAVG, 0.0, rc.NormLPTypeInEstimation);
                    NwaveletsInRF.Clear();
                    for (int i = 0; i < tmpRFdecTreeArrAVG.Count(); i++)
                        NwaveletsInRF.Add(tmpRFdecTreeArrAVG[i].Count());
                    Form1.printList(errorRyByForest.ToList(), analysisFolderName + "\\errorByForestAVG.txt");
                    Form1.printList(NwaveletsInRF, analysisFolderName + "\\NwaveletsInRFAVG.txt");

                    //PRUNING MIN                    
                    errorRyByForest = testDecisionTreeManyRFnew(testingArr, db.validation_dt, db.validation_label, tmpRFdecTreeArrMIN, 0.0, rc.NormLPTypeInEstimation);
                    NwaveletsInRF.Clear();
                    for (int i = 0; i < tmpRFdecTreeArrMIN.Count(); i++)
                        NwaveletsInRF.Add(tmpRFdecTreeArrMIN[i].Count());
                    Form1.printList(errorRyByForest.ToList(), analysisFolderName + "\\errorByForestMIN.txt");
                    Form1.printList(NwaveletsInRF, analysisFolderName + "\\NwaveletsInRFMIN.txt");
                }

                //WAVELETS ANALYSIS:
                if (Form1.u_config.estimateRFwaveletsCB == "1")
                {
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                    {
                        List<GeoWave>[] tmpRFdecTreeArr = new List<GeoWave>[i + 1];
                        Array.Copy(RFdecTreeArr, 0, tmpRFdecTreeArr, 0, i + 1);
                        resultsByTree = new double[tmpRFdecTreeArr.Count()][][];//[N trees][testingArr]
                        double[] normsOfTrees = new double[tmpRFdecTreeArr.Count()];//[N trees][testingArr]
                        Parallel.For(0, tmpRFdecTreeArr.Count(), k =>
                        {
                            resultsByTree[k] = new double[testingArr.Count()][];
                            for (int j = 0; j < testingArr.Count(); j++)
                                resultsByTree[k][j] = new double[db.validation_label[0].Count()];
                            normsOfTrees[k] = -1;// ITS SET ON THE FLY
                        });
                        List<double> errorRyByWavelets = new List<double>();
                        List<double> NwaveletsInWaveletByWavelet = new List<double>();
                        testWaveletsOneByOne(tmpRFdecTreeArr, testingArr, db.validation_dt, db.validation_label, ref NwaveletsInWaveletByWavelet, ref errorRyByWavelets);
                        //PRINT
                        Form1.printList(errorRyByWavelets, analysisFolderName + "\\errorByWaveletsCompressed" + i.ToString() + ".txt");
                        Form1.printList(NwaveletsInWaveletByWavelet, analysisFolderName + "\\NwaveletsInWaveletByWaveletCompressed" + i.ToString() + ".txt");
                    }
                }
                
                if (Form1.u_config.estimateRF4SmoothnessAnalysis == "1")
                {
                    //TEST ON TRAINING 
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                    {
                        List<double> errorRyByWaveletsOfOne = new List<double>();
                        List<double> NwaveletsInWaveletByWaveletOfOne = new List<double>();
                        for (int j = 0; j < norms[i].Count; j++)
                        {
                            j = skipIndex(j);
                            if (j >= norms[i].Count)
                                continue;

                            double tmpErr = testDecisionTree(trainingArrRF_indecesList[i], db.training_dt, db.training_label, RFdecTreeArr[i], norms[i][j], rc.NormLPTypeInEstimation, false);
                            errorRyByWaveletsOfOne.Add(tmpErr);
                            NwaveletsInWaveletByWaveletOfOne.Add(j);
                        }
                        //PRINT
                        Form1.printList(errorRyByWaveletsOfOne, analysisFolderName + "\\errorByWaveletsTraining" + i.ToString() + ".txt");
                        Form1.printList(NwaveletsInWaveletByWaveletOfOne, analysisFolderName + "\\NwaveletsInWaveletByWaveletTraining" + i.ToString() + ".txt");
                    }


                    //TEST ON testing (for thresholding) 
                    for (int i = 0; i < RFdecTreeArr.Count(); i++)
                    {
                        List<double> errorRyByWaveletsOfOne = new List<double>();
                        List<double> NwaveletsInWaveletByWaveletOfOne = new List<double>();
                        for (int j = 0; j < norms[i].Count; j++)
                        {
                            j = skipIndex(j);
                            if (j >= norms[i].Count)
                                continue;

                            double tmpErr = testDecisionTree(testingArr, db.testing_dt, db.testing_label, RFdecTreeArr[i], norms[i][j], rc.NormLPTypeInEstimation, false);
                            errorRyByWaveletsOfOne.Add(tmpErr);
                            NwaveletsInWaveletByWaveletOfOne.Add(j);
                        }
                        //PRINT
                        Form1.printList(errorRyByWaveletsOfOne, analysisFolderName + "\\errorByWaveletsThresholding" + i.ToString() + ".txt");
                        Form1.printList(NwaveletsInWaveletByWaveletOfOne, analysisFolderName + "\\NwaveletsInWaveletByWaveletThresholding" + i.ToString() + ".txt");
                    }                
                }
            }

            #endregion
        }

        private int skipIndex(int i)
        {
            if (i <= 100)
                return i;
            else if (i < 300)
                i += 25;
            else if (i < 1000)
                i += 50;
            else if (i < 5000)
                i += 100;
            else if (i < 10000)
                i += 200;
            else if (i < 100000)
                i += 500;
            else if (i < 200000)
                i += 50000;//1000
            else if (i < 300000)
                i += 100000;//50000
            else
                i += 1000000;//100000
            return i;
        }

        private void testWaveletsOneByOne(List<GeoWave>[] RFdecTreeArr, List<int> testingArr, double[][] dt, double[][] labels, 
                                          ref List<double> NwaveletsInWaveletByWavelet, ref List<double> errorRyByWavelets)
        {
            double[] normsOfTrees = new double[RFdecTreeArr.Count()];//[N trees][testingArr]
            //resultsByTree = new double[RFdecTreeArr.Count()][][];//[N trees][testingArr]
            Parallel.For(0, RFdecTreeArr.Count(), i =>
            {
                //RFdecTreeArr[i] = RFdecTreeArr[i].OrderBy(o => o.ID).ToList();
                //resultsByTree[i] = new double[testingArr.Count()][];
                //for (int j = 0; j < testingArr.Count(); j++)
                //    resultsByTree[i][j] = new double[db.validation_label[0].Count()];
                normsOfTrees[i] = -1;// ITS SET ON THE FLY
            });            
            
            double tmpErr = 0;
            List<double[]> NormMultyArr = new List<double[]>();
            for (int i = 0; i < RFdecTreeArr.Count(); i++)
                for (int j = 0; j < RFdecTreeArr[i].Count; j++)
                {
                    double[] pair = new double[2];
                    pair[0] = RFdecTreeArr[i][j].norm;
                    pair[1] = i;
                    NormMultyArr.Add(pair);
                }

            NormMultyArr = NormMultyArr.OrderByDescending(t => t[0]).ToList();

            int indexeTreeChanged = -1;
            bool newTree = false;

            //SET GLOBAL PARAMETER MODEFIED_LABLES (TO IMPROV EPERFORMANCE)
            resultsByForest = new double[testingArr.Count()];//[N trees][testingArr]
            //resultsByTree = new double[RFdecTreeArr.Count()][][];//[N trees][testingArr]
            double [][] modefied_Lables = new double[testingArr.Count()][];
            for (int i = 0; i < testingArr.Count(); i++)
                modefied_Lables[i] = new double[db.validation_label[0].Count()];

            int NerrorRyByWavelets = NormMultyArr.Count();
            int N_treesInUse = 0;
            for (int i = 0; i < NerrorRyByWavelets; i++)
            {
                if (i > 100 && N_treesInUse == RFdecTreeArr.Count())
                {
                    if (i < 500)
                        i += 25;
                    else if (i < 1000)
                        i += 50;
                    else if (i < 5000)
                        i += 100;
                    else if (i < 10000)
                        i += 200;
                    else if (i < 100000)
                        i += 500;
                    else if (i < 200000)
                        i += 50000;//1000
                    else if (i < 300000)
                        i += 100000;//50000
                    else
                        i += 1000000;//100000

                    if (i >= NerrorRyByWavelets)
                        continue;
                    preparenormsOfTrees(ref normsOfTrees, i, NormMultyArr);
                    tmpErr = calcResultByTree(testingArr, dt, labels, RFdecTreeArr, normsOfTrees);
                    errorRyByWavelets.Add(tmpErr);
                    NwaveletsInWaveletByWavelet.Add(i + 1);
                }
                else
                {
                    indexeTreeChanged = (int)NormMultyArr[i][1];
                    if (normsOfTrees[indexeTreeChanged] == -1)
                    {
                        newTree = true;
                        N_treesInUse++;
                    }
                    normsOfTrees[indexeTreeChanged] = NormMultyArr[i][0];
                    tmpErr = testerrorRyByWavelets(testingArr, dt, labels, RFdecTreeArr, normsOfTrees, ref N_treesInUse, ref indexeTreeChanged, newTree, rc.NormLPTypeInEstimation);
                    errorRyByWavelets.Add(tmpErr);
                    newTree = false;
                    NwaveletsInWaveletByWavelet.Add(i + 1);
                }
            }
        }

        private void setMidpoint(double[][] Data_Lables)
        {
            double minVal = 0;
            double maxVal = 1;
            for (int i = 0; i < Data_Lables.Count(); i++)
            {
                minVal = (Data_Lables[i][0] < minVal) ? Data_Lables[i][0] : minVal;
                maxVal = (Data_Lables[i][0] > maxVal) ? Data_Lables[i][0] : maxVal;
            }
            MidPoint = 0.5 * (minVal + maxVal);
        }

        private double[] testDecisionTreeManyRFnew(List<int> testingArr, double[][] Data_table, double[][] Data_Lables, List<GeoWave>[] RFdecTreeArr, double NormThreshold, int NormLPTypeInEstimation)
        {
            //GO OVER TESTING DATA AND GET ESTIMATIONS FOR EACH DATA LINE
            double[][][] estimatedLabels = new double[RFdecTreeArr.Count()][][];//num of trees, label index, label values (or value in most cases)
            for (int i = 0; i < RFdecTreeArr.Count(); i++)
            {
                estimatedLabels[i] = new double[testingArr.Count()][];
                for (int j = 0; j < testingArr.Count(); j++)
                    estimatedLabels[i][j] = new double[Data_Lables[0].Count()];
            }

            if (Form1.rumPrallel)
            {
                Parallel.For(0, testingArr.Count(), i =>
                {
                    //test_table_low_dim.Rows[i].ToArray().CopyTo(point,0);
                    double[] point = new double[rc.dim];//Data_table[0].Count()
                    //Data_table.CopyTo(point, i);
                    for (int j = 0; j < rc.dim; j++)//Data_table[0].Count()
                        point[j] = double.Parse(Data_table[testingArr[i]][j].ToString());
                    double[][] tmpLabel = new double[RFdecTreeArr.Count()][];
                    Parallel.For(0, RFdecTreeArr.Count(), j =>
                    {
                        tmpLabel[j] = askTreeMeanVal(point, RFdecTreeArr[j], NormThreshold);
                    });

                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                    {
                        estimatedLabels[0][i][j] = tmpLabel[0][j];
                        for (int k = 1; k < RFdecTreeArr.Count(); k++)
                            estimatedLabels[k][i][j] = (Convert.ToDouble(k) / (Convert.ToDouble(k) + 1)) * estimatedLabels[k - 1][i][j] + (1 / (Convert.ToDouble(k) + 1)) * tmpLabel[k][j];
                    }
                });
            }
            else
            {
                for (int i = 0; i < testingArr.Count(); i++)
                {
                    //test_table_low_dim.Rows[i].ToArray().CopyTo(point,0);
                    double[] point = new double[rc.dim];//Data_table[0].Count()
                    //Data_table.CopyTo(point, i);
                    for (int j = 0; j < rc.dim; j++)//Data_table[0].Count()
                        point[j] = double.Parse(Data_table[testingArr[i]][j].ToString());
                    double[][] tmpLabel = new double[RFdecTreeArr.Count()][];

                    for (int j = 0; j < RFdecTreeArr.Count(); j++)
                    {
                        tmpLabel[j] = askTreeMeanVal(point, RFdecTreeArr[j], NormThreshold);
                    }


                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                    {
                        estimatedLabels[0][i][j] = tmpLabel[0][j];
                        for (int k = 1; k < RFdecTreeArr.Count(); k++)
                            estimatedLabels[k][i][j] = (Convert.ToDouble(k) / (Convert.ToDouble(k) + 1)) * estimatedLabels[k - 1][i][j] + (1 / (Convert.ToDouble(k) + 1)) * tmpLabel[k][j];
                    }
                }
            }

            double[] error = new double[RFdecTreeArr.Count()];
            if (NormLPTypeInEstimation == 2)
            {
                for (int k = 0; k < RFdecTreeArr.Count(); k++)
                {
                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                        for (int i = 0; i < testingArr.Count(); i++)
                            error[k] += (estimatedLabels[k][i][j] - Data_Lables[testingArr[i]][j]) * (estimatedLabels[k][i][j] - Data_Lables[testingArr[i]][j]);
                    error[k] = Math.Sqrt(error[k] / Convert.ToDouble(testingArr.Count()));
                }
            }
            else if (NormLPTypeInEstimation == 1)
            {
                for (int k = 0; k < RFdecTreeArr.Count(); k++)
                {
                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                        for (int i = 0; i < testingArr.Count(); i++)
                            error[k] += Math.Abs(estimatedLabels[k][i][j] - Data_Lables[testingArr[i]][j]);
                }
            }
            else if(NormLPTypeInEstimation == 0)
            {
                int do_nothing = 0;
                for (int k = 0; k < RFdecTreeArr.Count(); k++)
                {
                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                        for (int i = 0; i < testingArr.Count(); i++)
                        {
                            if((estimatedLabels[k][i][j] >= MidPoint) &&  (Data_Lables[testingArr[i]][j] >= MidPoint) ||
                                (estimatedLabels[k][i][j] < MidPoint) &&  (Data_Lables[testingArr[i]][j] < MidPoint))
                                do_nothing++;
                            else
                                error[k]++;
                        }
                    error[k] = (error[k]/ (double)testingArr.Count());
                }                
                
            }
            return error;

        }

        //FROM LESS IMPORTANT TO MORE IMPORTANT
        private List<int> reorderTreesByContribution(List<List<GeoWave>> RFdecTreeArr, string MarginType, List<int> trainingArr, double[][] trainingData, double[][] trainingLabel)
        {
            //TABLE OF ERRORS FOR EACH TREE IN PREDICTION (BASED ON TRAINING)
            double[,] errorTable = new double[RFdecTreeArr.Count, trainingArr.Count];//each column is a prediction of error of tree

            //GET ESTIMATED LABELS
            double[][][] estimatedLabels = getEstimatedLabeles(RFdecTreeArr, trainingArr, trainingData, trainingLabel);
            
            //FIND MIN MAX VALUES TO DETECT REGRESSION OR CLASSIFICATION
            double minVal = 0;
            double maxVal = 1;
            for (int i = 0; i < trainingLabel.Count(); i++)
            {
                minVal = (trainingLabel[i][0] < minVal) ? trainingLabel[i][0] : minVal;
                maxVal = (trainingLabel[i][0] > maxVal) ? trainingLabel[i][0] : maxVal;
            }
            double midPoint = 0.5 * (minVal + maxVal);
            bool regression = (maxVal == 1 && (minVal == -1 || minVal == 0)) ? false : true;

            //ERROR[I] IS THE MARGIN VECTOR of full tree and err for each tree is saved at errorTable

            //ERR[I] IS THE ERROR OF POINT I USING ALL TREES
            double[] error = getAvgErr(trainingArr, trainingLabel, RFdecTreeArr, regression, estimatedLabels, midPoint, ref errorTable);

            //PREPARE INDEX LIST OF REMOVED TREES
            List<int> indexOfRemoved = new List<int>();
            List<int> indexOfTrees = new List<int>();
            for (int i = 0; i < RFdecTreeArr.Count; i++)
                indexOfTrees.Add(i);

            //OPERATED ON ALL TREES UNTILL ONE TREE IS LEFT
            for (int i = 0; i < RFdecTreeArr.Count; i++ )
            {
                //THE INDEX IS FROM THE [0,INDEXOFTREES] RANGE
                int indexTreeWithLowestImpact = getindexTreeWithLowestImpact(indexOfTrees, trainingArr, errorTable, error, MarginType);

                //ADD
                indexOfRemoved.Add(indexOfTrees[indexTreeWithLowestImpact]);

                //UPDATE AVG ERROR
                for (int k = 0; k < error.Count(); k++)
                {
                    error[k] = ((indexOfTrees.Count() * error[k]) - errorTable[indexOfTrees[indexTreeWithLowestImpact], k]) / (indexOfTrees.Count() - 1);
                }

                //REMOVE LESS IMPORTANT TREE
                indexOfTrees.RemoveAt(indexTreeWithLowestImpact);
            }
            return indexOfRemoved;
        }

        private int getindexTreeWithLowestImpact(List<int> indexOfTrees, List<int> trainingArr, double[,] errorTable, double[] error, string MarginType)
        {
            int indexTreeWithLowestImpact = -1;
            double lowImpact = double.MaxValue;
            for (int j = 0; j < indexOfTrees.Count; j++)
            {
                double tmpErr = 0;
                if (MarginType == "AVG")
                {
                    for (int k = 0; k < trainingArr.Count(); k++)
                    {
                        tmpErr += Math.Abs(errorTable[indexOfTrees[j], k] - error[k]);
                    }                
                }
                else if (MarginType == "MIN")
                {
                    tmpErr = double.MaxValue;
                    for (int k = 0; k < trainingArr.Count(); k++)
                    {
                        if (error[k] < tmpErr) 
                            tmpErr = error[k];
                    }  
                }

                if (tmpErr < lowImpact)
                {
                    lowImpact = tmpErr;
                    indexTreeWithLowestImpact = j;
                }
            }
            return indexTreeWithLowestImpact;
        }

        private double[] getAvgErr(List<int> trainingArr, double[][] trainingLabel, List<List<GeoWave>> RFdecTreeArr, bool regression, double[][][] estimatedLabels, double midPoint, ref double[,] errorTable)
        {
            double[] error = new double[trainingArr.Count];//or margin
            for (int i = 0; i < trainingArr.Count(); i++)//go over ALL LABELS
            {
                for (int k = 0; k < RFdecTreeArr.Count(); k++)//GO OVER ALL TREES
                {
                    double err = 0;
                    if (regression)
                    {
                        err = Math.Pow((estimatedLabels[k][i][0] - trainingLabel[trainingArr[i]][0]), 2);
                    }
                    else
                        err = ((estimatedLabels[k][i][0] > midPoint && trainingLabel[trainingArr[i]][0] > midPoint))
                            || (estimatedLabels[k][i][0] < midPoint && trainingLabel[trainingArr[i]][0] < midPoint) ? 1 : -1;

                    errorTable[k, i] = err;

                    error[i] += err;
                }
                error[i] /= RFdecTreeArr.Count();
            }
            return error;
        }

        private double[][][] getEstimatedLabeles(List<List<GeoWave>> RFdecTreeArr, List<int> trainingArr, double[][] trainingData, double[][] trainingLabel)
        {
            //GO OVER TESTING DATA AND GET ESTIMATIONS FOR EACH DATA LINE
            double[][][] estimatedLabels = new double[RFdecTreeArr.Count()][][];//index of tree, label index, label values (or value in most cases)
            for (int i = 0; i < RFdecTreeArr.Count(); i++)
            {
                estimatedLabels[i] = new double[trainingArr.Count()][];
                for (int j = 0; j < trainingArr.Count(); j++)
                    estimatedLabels[i][j] = new double[trainingLabel[0].Count()];
            }

            if (Form1.rumPrallel)
            {
                Parallel.For(0, trainingArr.Count(), i =>
                {
                    double[] point = new double[rc.dim];
                    for (int j = 0; j < rc.dim; j++)
                        point[j] = double.Parse(trainingData[trainingArr[i]][j].ToString());
                    double[][] tmpLabel = new double[RFdecTreeArr.Count()][];
                    Parallel.For(0, RFdecTreeArr.Count(), j =>
                    {
                        tmpLabel[j] = askTreeMeanVal(point, RFdecTreeArr[j], 0);// 0 FOR TAKING WORST CASE
                    });

                    for (int j = 0; j < trainingLabel[0].Count(); j++)
                    {
                        for (int k = 0; k < RFdecTreeArr.Count(); k++)
                            estimatedLabels[k][i][j] = tmpLabel[k][j];
                    }
                });
            }
            else
            {
                for (int i = 0; i < trainingArr.Count(); i++)
                {
                    //test_table_low_dim.Rows[i].ToArray().CopyTo(point,0);
                    double[] point = new double[rc.dim];//Data_table[0].Count()
                    //Data_table.CopyTo(point, i);
                    for (int j = 0; j < rc.dim; j++)//Data_table[0].Count()
                        point[j] = double.Parse(trainingData[trainingArr[i]][j].ToString());
                    double[][] tmpLabel = new double[RFdecTreeArr.Count()][];

                    for (int j = 0; j < RFdecTreeArr.Count(); j++)
                    {
                        tmpLabel[j] = askTreeMeanVal(point, RFdecTreeArr[j], 0);// 0 FOR TAKING WORST CASE
                    }

                    for (int j = 0; j < trainingLabel[0].Count(); j++)
                    {
                        for (int k = 0; k < RFdecTreeArr.Count(); k++)
                            estimatedLabels[k][i][j] = tmpLabel[k][j];
                    }
                }
            }
            return estimatedLabels;
        }

        private double calcResultByTree(List<int> testingArr, double[][] Data_table, double[][] Data_Lables, 
                                             List<GeoWave>[] RFdecTreeArrById, double[] normsOfTrees)
        {
            double[] errArr = new double[testingArr.Count()];         
            
            Parallel.For(0, testingArr.Count(), i =>
            {
                //GO OVER ALL TREES
                double tmpVal = 0;
                for(int j=0; j < RFdecTreeArrById.Count();j++)
                {
                    double[] val= askTreeMeanVal(Data_table[testingArr[i]], RFdecTreeArrById[j], normsOfTrees[j]);
                    tmpVal += val[0];
                }
                tmpVal /= RFdecTreeArrById.Count();

                if(rc.NormLPTypeInEstimation == 0 )
                {
                    if( (((tmpVal >= MidPoint) &&  (Data_Lables[testingArr[i]][0] < MidPoint)) ||
                                ((tmpVal < MidPoint) &&  (Data_Lables[testingArr[i]][0] >= MidPoint))))
                    errArr[i]++;
                }
                else
                    errArr[i] = (tmpVal - Data_Lables[testingArr[i]][0])*(tmpVal- Data_Lables[testingArr[i]][0]);
            });

            double error = 0;
            for(int i=0; i < errArr.Count(); i++)
                error +=errArr[i];
            if(rc.NormLPTypeInEstimation == 0)
                error /=(double)testingArr.Count();
            else
                error = Math.Sqrt(error / Convert.ToDouble(testingArr.Count()));
            return error;   


        }

        private void preparenormsOfTrees(ref double[] normsOfTrees, int index2start, List<double[]> NormMultyArr)
        {
            bool[] wasSet = new bool[normsOfTrees.Count()];
            int ID = -1;
            int totalSet = 0;
            while (totalSet != normsOfTrees.Count())
            {
                ID = (int)NormMultyArr[index2start][1];
                if (wasSet[ID])
                {
                    index2start--;
                    continue;
                }

                normsOfTrees[ID] = NormMultyArr[index2start][0];
                wasSet[ID] = true;
                totalSet++;
                index2start--;
            }
        }

        private double testerrorRyByWavelets(List<int> testingArr, double[][] Data_table, double[][] Data_Lables, 
                                             List<GeoWave>[] RFdecTreeArrById, double[] normsOfTrees, 
                                             ref int N_treesInUse, ref int indexeTreeChanged, bool newTree, int NormLPTypeInEstimation)
        {
            //GO TO THE TREE THAT HAD BEEN CHANGED (ONE MORE WAVELET) AND RE-CALCULATE IT 
            int tmpIndexeTreeChanged = indexeTreeChanged;
            double oldVal = 0;
            double newVal = 0;
            double weightOldGroup = Convert.ToDouble(N_treesInUse - 1) / (Convert.ToDouble(N_treesInUse));
            double weightChangedTree = Convert.ToDouble(1) / (Convert.ToDouble(N_treesInUse) );

            if (Form1.rumPrallel)
            {
                double [][] modefied_Lables = new double[testingArr.Count()][];
                for(int i=0; i < testingArr.Count(); i++)
                    modefied_Lables[i] = new double[1]; 

                Parallel.For(0, testingArr.Count(), i =>
                {
                    modefied_Lables[i] = askTreeMeanVal(Data_table[testingArr[i]], RFdecTreeArrById[tmpIndexeTreeChanged], normsOfTrees[tmpIndexeTreeChanged]);
                    oldVal = resultsByTree[tmpIndexeTreeChanged][i][0];// - GET OLD VAL BEFORE CHANGE (COULD BE ZERO) HERE I DON'T SUPPORT MULTY LABELING - CAN BE EASLY CHANGED
                    resultsByTree[tmpIndexeTreeChanged][i] = modefied_Lables[i];
                    newVal = resultsByTree[tmpIndexeTreeChanged][i][0];
                    ////SET TOTAL
                    //if (newTree)
                    //    resultsByForest[i] = weightChangedTree * (newVal - oldVal) + weightOldGroup * resultsByForest[i];
                    //else
                    //    resultsByForest[i] += weightChangedTree * (newVal - oldVal);
                    int total = 0;
                    double avg = 0;
                    for (int j = 0; j < normsOfTrees.Count(); j++)
                    {
                        if (normsOfTrees[j] == -1)
                            continue;
                        total++;
                        avg += resultsByTree[tmpIndexeTreeChanged][i][0];
                    }
                    resultsByForest[i] = avg / total;
                });
            }
            else
            {
                for (int i = 0; i < testingArr.Count(); i++)
                {
                    double[] val = askTreeMeanVal(Data_table[testingArr[i]], RFdecTreeArrById[tmpIndexeTreeChanged], normsOfTrees[tmpIndexeTreeChanged]);
                    oldVal = resultsByTree[tmpIndexeTreeChanged][i][0];// - GET OLD VAL BEFORE CHANGE (COULD BE ZERO) HERE I DON'T SUPPORT MULTY LABELING - CAN BE EASLY CHANGED
                    resultsByTree[tmpIndexeTreeChanged][i][0] = val[0];
                    ////SET TOTAL
                    //if (newTree)
                    //    resultsByForest[i] = weightChangedTree * (newVal - oldVal) + weightOldGroup * resultsByForest[i];
                    //else
                    //    resultsByForest[i] += weightChangedTree * (newVal - oldVal);
                    int total = 0;
                    double avg = 0;
                    for (int j = 0; j < normsOfTrees.Count(); j++)
                    {
                        if (normsOfTrees[j] == -1)
                            continue;
                        total++;
                        avg += resultsByTree[j][i][0];
                    }
                    resultsByForest[i] = avg / total;
                }            
            }

            double error = 0;
            if (NormLPTypeInEstimation == 0)
            {
                double errArr = 0;
                for (int i = 0; i < testingArr.Count(); i++)
                    if ((((resultsByForest[i] >= MidPoint) && (Data_Lables[testingArr[i]][0] < MidPoint)) || ((resultsByForest[i] < MidPoint) && (Data_Lables[testingArr[i]][0] >= MidPoint))))
                        errArr++;
                error = errArr/ (double)testingArr.Count();
            }
            else
            {
                for (int i = 0; i < testingArr.Count(); i++)
                    error += (resultsByForest[i] - Data_Lables[testingArr[i]][0]) * (resultsByForest[i] - Data_Lables[testingArr[i]][0]);
                error = Math.Sqrt(error / Convert.ToDouble(testingArr.Count()));            
            }

            return error;
        }


        private bool[] getDim2Take(recordConfig rc, int Seed)
        {
            bool[] Dim2Take = new bool[rc.dim];

            var ran = new Random(Seed);
            //List<int> dimArr = Enumerable.Range(0, rc.dim).OrderBy(x => ran.Next()).ToList().GetRange(0, rc.dim);
            //List<int> dimArr = Enumerable.Range(0, rc.dim).OrderBy(x => ran.Next()).ToList().GetRange(0, rc.dim);
            for (int i = 0; i < rc.NDimsinRF; i++)
            {
                int index = ran.Next(0, rc.dim);
                if (Dim2Take[index] == true)
                    i--;
                else
                    Dim2Take[index] = true;
            }

            return Dim2Take;
        }

        private List<int> Bagging(List<int> trainingArr, double percent, int Seed)//percent in [0,1]
        {
            //List<int> baggedArr = new List<int>();
            int N_rows = Convert.ToInt32(trainingArr.Count * percent);
            //int Seed = (int)DateTime.Now.Ticks;
            var ran = new Random(Seed);
//            return Enumerable.Range(0, trainingArr.Count).OrderBy(x => ran.Next()).ToList().GetRange(0, N_rows);
            return trainingArr.OrderBy(x => ran.Next()).ToList().GetRange(0, N_rows);
        }

        private List<int> BaggingBreiman(List<int> trainingArr, int Seed)//percent in [0,1]
        {
            bool[] isSet = new bool[trainingArr.Count];
            List<int> baggedArr = new List<int>();
            var ran = new Random(Seed);
            for (int i = 0; i < trainingArr.Count; i++)
            {
                int j = ran.Next(0, trainingArr.Count);
                if (isSet[j] == false)
                    baggedArr.Add(trainingArr[j]);
                isSet[j] = true;
            }          
            return baggedArr;
        }

        private double testDecisionTree(List<int> testingArr, double[][] Data_table, double[][] Data_Lables, List<GeoWave> Tree_orderedById, double NormThreshold, int NormLPTypeInEstimation, bool Sort)
        {
            if(Sort)
                Tree_orderedById = Tree_orderedById.OrderBy(o => o.ID).ToList();

            //GO OVER TESTING DATA AND GET ESTIMATIONS FOR EACH DATA LINE
            double[][] estimatedLabels = new double[testingArr.Count()][];
            for (int i = 0; i < testingArr.Count(); i++)
                estimatedLabels[i] = new double[Data_Lables[0].Count()];

            if (Form1.rumPrallel)
            {
                Parallel.For(0, testingArr.Count(), i =>
                {
                    estimatedLabels[i] = askTreeMeanVal(Data_table[testingArr[i]], Tree_orderedById, NormThreshold);
                });
            }
            else
            {
                for (int i = 0; i < testingArr.Count(); i++)
                {
                    estimatedLabels[i] = askTreeMeanVal(Data_table[testingArr[i]], Tree_orderedById, NormThreshold);
                }
            }


            double error = 0;
            if (NormLPTypeInEstimation == 2)
            {
                for (int j = 0; j < Data_Lables[0].Count(); j++)
                    for (int i = 0; i < testingArr.Count(); i++)
                    {
                        error += (estimatedLabels[i][j] - Data_Lables[testingArr[i]][j]) * (estimatedLabels[i][j] - Data_Lables[testingArr[i]][j]);
                    }
                error = Math.Sqrt(error / Convert.ToDouble(testingArr.Count()));
            }
            else if (NormLPTypeInEstimation == 1)
            {
                for (int j = 0; j < Data_Lables[0].Count(); j++)
                    for (int i = 0; i < testingArr.Count(); i++)
                    {
                        error += Math.Abs(estimatedLabels[i][j] - Data_Lables[testingArr[i]][j]);
                    }
            }
            else if (NormLPTypeInEstimation == 0)
            {
                int do_nothing = 0;
                for (int j = 0; j < Data_Lables[0].Count(); j++)
                    for (int i = 0; i < testingArr.Count(); i++)
                     {
                         if ((estimatedLabels[i][j] >= MidPoint) && (Data_Lables[testingArr[i]][j] >= MidPoint) ||
                                (estimatedLabels[i][j] < MidPoint) && (Data_Lables[testingArr[i]][j] < MidPoint))
                                do_nothing++;
                            else
                                error++;
                        }
                    error /= (double)testingArr.Count();

            }
            else if (NormLPTypeInEstimation == -1)//max
            {
                List<double> errList = new List<double>();
                double tmp = 0;
                for (int i = 0; i < testingArr.Count(); i++)
                {
                    tmp = 0;
                    for (int j = 0; j < Data_Lables[0].Count(); j++)
                    {
                        tmp += Math.Abs(estimatedLabels[i][j] - Data_Lables[testingArr[i]][j]);
                    }
                    errList.Add(tmp);
                }
                error = errList.Max();
            }
            else if (NormLPTypeInEstimation == 0 && estimatedLabels[0].Count() == 1)//+-1 labels
            {
                for (int i = 0; i < testingArr.Count(); i++)
                {
                    if ((estimatedLabels[i][0] * Data_Lables[testingArr[i]][0]) <= 0)
                        error += 1;
                }
            }
            else if (NormLPTypeInEstimation == -2 && estimatedLabels[0].Count() == 1)//+-1 labels + BER
            {
                double NclassA = 0;
                double NclassB = 0;
                double NMissclassA = 0;
                double NMissclassB = 0;

                for (int i = 0; i < testingArr.Count(); i++)
                {
                    if (Data_Lables[testingArr[i]][0] == 1)
                    {
                        NclassA += 1;
                        if (estimatedLabels[i][0] <= 0)
                            NMissclassA += 1;
                    }
                    if (Data_Lables[testingArr[i]][0] == -1)
                    {
                        NclassB += 1;
                        if (estimatedLabels[i][0] >= 0)
                            NMissclassB += 1;
                    }
                }
                error = 0.5 * ((NMissclassA / NclassA) + (NMissclassB / NclassB));
            }

            return error;

            //printErrorsToFile(Form1.MainFolderName + Form1.dataStruct[5] + "\\misslabeling_results_Dim" + test_table_low_dim.Columns.Count.ToString() + ".txt", l2_error, l1_error, numOfMissLables, test_Lables.Rows.Count);
        }

        private double[] askTreeMeanVal(double[] point, List<GeoWave> Tree_orderedById, double NormThreshold)
        {

            int counter = 0;
            if (!DB.IsPntInsideBox(Tree_orderedById[0].boubdingBox, point, rc.dim))
            {
                DB.ProjectPntInsideBox(Tree_orderedById[0].boubdingBox, ref point);
                counter++;
            }

            double[] zeroMean = new double[Tree_orderedById[0].MeanValue.Count()];
            double[] MeanValue = new double[Tree_orderedById[0].MeanValue.Count()];

            //SET THE ROOT MEAN VAL
            Tree_orderedById[0].MeanValue.CopyTo(MeanValue, 0);

            ////get to leaf 

            int parent_index = 0;
            bool endOfLoop = false;

            while (!endOfLoop)
            {
                if (Tree_orderedById[parent_index].child0 != -1 && DB.IsPntInsideBox(Tree_orderedById[Tree_orderedById[parent_index].child0].boubdingBox, point, rc.dim))
                {
                    if (!Tree_orderedById[Tree_orderedById[parent_index].child0].MeanValue.SequenceEqual(zeroMean) &&
                        NormThreshold <= Tree_orderedById[Tree_orderedById[parent_index].child0].norm) //take the mean value if its not 0 and the wavelete should be taken ( norm size) - or if its the root wavelete
                    {
                        MeanValue[0] += (Tree_orderedById[Tree_orderedById[parent_index].child0].MeanValue[0] - Tree_orderedById[parent_index].MeanValue[0]);
                    }

                    parent_index = Tree_orderedById[parent_index].child0;
                }
                else if (Tree_orderedById[parent_index].child1 != -1 && DB.IsPntInsideBox(Tree_orderedById[Tree_orderedById[parent_index].child1].boubdingBox, point, rc.dim))
                {
                    if (NormThreshold <= Tree_orderedById[Tree_orderedById[parent_index].child1].norm) //take the mean value if its above threshold
                    {
                        MeanValue[0] += (Tree_orderedById[Tree_orderedById[parent_index].child1].MeanValue[0] - Tree_orderedById[parent_index].MeanValue[0]);
                    }

                    parent_index = Tree_orderedById[parent_index].child1;
                }
                else
                    endOfLoop = true;
            }
            return MeanValue;
        }

    }
}
