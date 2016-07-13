using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.IO;

namespace DataSetsSparsity
{
    class decicionTree
    {
        private recordConfig rc;
        private double[][] training_dt;
        private long[][] training_GridIndex_dt;
        private double[][] training_label;
        private bool[] Dime2Take;

        public decicionTree(recordConfig rc, DB db, bool[] Dime2Take)
        {
            this.training_dt = db.training_dt;
            this.training_label = db.training_label;
            this.training_GridIndex_dt = db.DBtraining_GridIndex_dt;
            this.rc = rc;
            this.Dime2Take = Dime2Take;
        }

        public List<GeoWave> getdecicionTree(List<int> trainingArr, int[][] boundingBox, int seed = -1)
        {
            //CREATE DECISION_GEOWAVEARR
            List<GeoWave> decision_GeoWaveArr = new List<GeoWave>();

            //SET ROOT WAVELETE
            GeoWave gwRoot = new GeoWave(rc.dim, training_label[0].Count(), rc);

            //SET REGION POINTS IDS
            gwRoot.pointsIdArray = trainingArr;
            boundingBox.CopyTo(gwRoot.boubdingBox, 0);

            decision_GeoWaveArr.Add(gwRoot);
            DecomposeWaveletsByConsts(decision_GeoWaveArr, seed);

            //SET ID
            for (int i = 0; i < decision_GeoWaveArr.Count; i++)
                decision_GeoWaveArr[i].ID = i;

            //get sorted list
            decision_GeoWaveArr = decision_GeoWaveArr.OrderByDescending(o => o.norm).ToList();

            return decision_GeoWaveArr;
        }

        public void DecomposeWaveletsByConsts(List<GeoWave> GeoWaveArr, int seed = -1)//SHOULD GET LIST WITH ROOT GEOWAVE
        {
            GeoWaveArr[0].MeanValue = GeoWaveArr[0].calc_MeanValue(training_label, GeoWaveArr[0].pointsIdArray);
            GeoWaveArr[0].computeNormOfConsts(rc.split_type);
            GeoWaveArr[0].level = 0;

            if (seed == -1)
                recursiveBSP_WaveletsByConsts(GeoWaveArr, 0);
            else recursiveBSP_WaveletsByConsts(GeoWaveArr, 0, seed);//0 is the root index
        }

        private void recursiveBSP_WaveletsByConsts(List<GeoWave> GeoWaveArr, int GeoWaveID, int seed=0)
        {
            //CALC APPROX_SOLUTION FOR GEO WAVE
            double Error = GeoWaveArr[GeoWaveID].calc_MeanValueReturnError(training_label, GeoWaveArr[GeoWaveID].pointsIdArray);
            if (Error < rc.approxThresh || GeoWaveArr[GeoWaveID].pointsIdArray.Count() <= rc.minWaveSize || rc.boundDepthTree <=  GeoWaveArr[GeoWaveID].level)
                return;

            int dimIndex = -1;
            int Maingridindex = -1;

            bool IsPartitionOK = false;
            if (rc.split_type == 0)
                IsPartitionOK = getBestPartitionResult(ref dimIndex, ref Maingridindex, GeoWaveArr, GeoWaveID, Error, Dime2Take);
            else if (rc.split_type == 1)//rand split
                IsPartitionOK = getRandPartitionResult(ref dimIndex, ref Maingridindex, GeoWaveArr, GeoWaveID, Error, seed);
            else if (rc.split_type == 2)//rand features in each node
            {
                var ran1 = new Random(seed);
                var ran2 = new Random(GeoWaveID);
                int one = ran1.Next(0, int.MaxValue / 10);
                int two = ran2.Next(0, int.MaxValue / 10);
                bool[] Dim2TakeNode = getDim2Take(rc, one + two);
                IsPartitionOK = getBestPartitionResult(ref dimIndex, ref Maingridindex, GeoWaveArr, GeoWaveID, Error, Dim2TakeNode);
            }
            else if (rc.split_type == 3)//Gini split
            {
                IsPartitionOK = GetGiniPartitionResult(ref dimIndex, ref Maingridindex, GeoWaveArr, GeoWaveID, Error, Dime2Take);
            }
            else if (rc.split_type == 4)//Gini split + rand node
            {
                var ran1 = new Random(seed);
                var ran2 = new Random(GeoWaveID);
                int one = ran1.Next(0, int.MaxValue / 10);
                int two = ran2.Next(0, int.MaxValue / 10);
                bool[] Dim2TakeNode = getDim2Take(rc, one + two);
                IsPartitionOK = GetGiniPartitionResult(ref dimIndex, ref Maingridindex, GeoWaveArr, GeoWaveID, Error, Dim2TakeNode);
            }

            if (!IsPartitionOK)
                return;

            GeoWave child0 = new GeoWave(GeoWaveArr[GeoWaveID].boubdingBox, training_label[0].Count(), GeoWaveArr[GeoWaveID].rc);
            GeoWave child1 = new GeoWave(GeoWaveArr[GeoWaveID].boubdingBox, training_label[0].Count(), GeoWaveArr[GeoWaveID].rc);

            //set partition
            child0.boubdingBox[1][dimIndex] = Maingridindex;
            child1.boubdingBox[0][dimIndex] = Maingridindex;

            //DOCUMENT ON CHILDREN
            child0.dimIndex = dimIndex;
            child0.Maingridindex = Maingridindex;
            child1.dimIndex = dimIndex;
            child1.Maingridindex = Maingridindex;
            child0.MaingridValue = Form1.MainGrid[dimIndex][Maingridindex];
            child1.MaingridValue = Form1.MainGrid[dimIndex][Maingridindex];

            //DOCUMENT ON PARENT
            GeoWaveArr[GeoWaveID].dimIndexSplitter = dimIndex;
            GeoWaveArr[GeoWaveID].splitValue = Form1.MainGrid[dimIndex][Maingridindex];

            //calc norm
            //calc mean value

            if (Form1.IsBoxSingular(child0.boubdingBox, rc.dim) || Form1.IsBoxSingular(child1.boubdingBox, rc.dim))
                return;

            //SHOULD I VERIFY THAT THE CHILD IS NOT ITS PARENT ? (IN CASES WHERE CAN'T MODEFY THE PARTITION)

            setChildrensPointsAndMeanValue(ref child0, ref child1, dimIndex, GeoWaveArr[GeoWaveID].pointsIdArray);
            //SET TWO CHILDS
            child0.parentID = child1.parentID = GeoWaveID;
            child0.child0 = child1.child0 = -1;
            child0.child1 = child1.child1 = -1;
            child0.level = child1.level = GeoWaveArr[GeoWaveID].level + 1;

            child0.computeNormOfConsts(GeoWaveArr[GeoWaveID], rc.split_type);
            child1.computeNormOfConsts(GeoWaveArr[GeoWaveID], rc.split_type);
            GeoWaveArr.Add(child0);
            GeoWaveArr.Add(child1);
            GeoWaveArr[GeoWaveID].child0 = GeoWaveArr.Count - 2;
            GeoWaveArr[GeoWaveID].child1 = GeoWaveArr.Count - 1;
            child0.dimIndex = dimIndex;
            child1.dimIndex = dimIndex;

            //RECURSION STEP !!!
            recursiveBSP_WaveletsByConsts(GeoWaveArr, GeoWaveArr[GeoWaveID].child0, seed);
            recursiveBSP_WaveletsByConsts(GeoWaveArr, GeoWaveArr[GeoWaveID].child1, seed);
        }

        private bool getBestPartitionResult(ref int dimIndex, ref int Maingridindex, List<GeoWave> GeoWaveArr, int GeoWaveID, double Error, bool[] Dims2Take)
        {
            double[][] error_dim_partition = new double[2][];//error, Maingridindex
            error_dim_partition[0] = new double[rc.dim];
            error_dim_partition[1] = new double[rc.dim];

            //PARALLEL RUN - SEARCHING BEST PARTITION IN ALL DIMS
            if (Form1.rumPrallel)
            {
                Parallel.For(0, rc.dim, i =>
                {
                    //double[] tmpResult = getBestPartition(i, GeoWaveArr[GeoWaveID]);
                    if (Dims2Take[i])
                    {
                        double[] tmpResult = getBestPartitionLargeDB(i, GeoWaveArr[GeoWaveID]);
                        error_dim_partition[0][i] = tmpResult[0];//error
                        error_dim_partition[1][i] = tmpResult[1];//Maingridindex                    
                    }
                    else
                    {
                        error_dim_partition[0][i] = double.MaxValue;//error
                        error_dim_partition[1][i] = -1;//Maingridindex                    
                    }
                });
            }
            else
            {
                for (int i = 0; i < rc.dim; i++)
                {
                    //double[] tmpResult = getBestPartition(i, GeoWaveArr[GeoWaveID]);
                    if (Dims2Take[i])
                    {
                        double[] tmpResult = getBestPartitionLargeDB(i, GeoWaveArr[GeoWaveID]);
                        error_dim_partition[0][i] = tmpResult[0];//error
                        error_dim_partition[1][i] = tmpResult[1];//Maingridindex                    
                    }
                    else
                    {
                        error_dim_partition[0][i] = double.MaxValue;//error
                        error_dim_partition[1][i] = -1;//Maingridindex                    
                    }
                }
            }

            dimIndex = Enumerable.Range(0, error_dim_partition[0].Count())
                .Aggregate((a, b) => (error_dim_partition[0][a] < error_dim_partition[0][b]) ? a : b);

            if (error_dim_partition[0][dimIndex] >= Error)
                return false;//if best partition doesn't help - return

            Maingridindex = Convert.ToInt32(error_dim_partition[1][dimIndex]);
            return true;
        }

        private double[] getBestPartitionLargeDB(int dimIndex, GeoWave geoWave)
        {
            double[] error_n_point = new double[2];//error index
            if (Form1.MainGrid[dimIndex].Count == 1)//empty feature
            {
                error_n_point[0] = double.MaxValue;
                error_n_point[1] = -1;
                return error_n_point;
            }
            //sort ids (for labels) acording to position at Form1.MainGrid[dimIndex][index]
            List<int> tmpIDs = new List<int>(geoWave.pointsIdArray);
            tmpIDs.Sort(delegate(int c1, int c2) { return training_dt[c1][dimIndex].CompareTo(training_dt[c2][dimIndex]); });

            if (training_dt[tmpIDs[0]][dimIndex] == training_dt[tmpIDs[tmpIDs.Count - 1]][dimIndex])//all values are the same 
            {
                error_n_point[0] = double.MaxValue;
                error_n_point[1] = -1;
                return error_n_point;
            }

            int best_ID = -1;
            double lowest_err = double.MaxValue;
            double[] leftAvg = new double[geoWave.MeanValue.Count()];
            double[] rightAvg = new double[geoWave.MeanValue.Count()];
            double[] leftErr = geoWave.calc_MeanValueReturnError(training_label, geoWave.pointsIdArray, ref leftAvg);//CONTAINES ALL POINTS - AT THE BEGINING
            double[] rightErr = new double[geoWave.MeanValue.Count()];

            double N_points = Convert.ToDouble(tmpIDs.Count);

            double tmp_err;

            for (int i = 0; i < tmpIDs.Count - 1; i++)//we dont calc the last (rightmost) boundary - it equal to the left most
            {
                tmp_err = 0;
                for (int j = 0; j < geoWave.MeanValue.Count(); j++)
                {
                    leftErr[j] = leftErr[j] - (N_points - i) * (training_label[tmpIDs[tmpIDs.Count - i - 1]][j] - leftAvg[j]) * (training_label[tmpIDs[tmpIDs.Count - i - 1]][j] - leftAvg[j]) / (N_points - i - 1);
                    leftAvg[j] = (N_points - i) * leftAvg[j] / (N_points - i - 1) - training_label[tmpIDs[tmpIDs.Count - i - 1]][j] / (N_points - i - 1);
                    rightErr[j] = rightErr[j] + (training_label[tmpIDs[tmpIDs.Count - i - 1]][j] - rightAvg[j]) * (training_label[tmpIDs[tmpIDs.Count - i - 1]][j] - rightAvg[j]) * Convert.ToDouble(i) / Convert.ToDouble(i + 1);
                    rightAvg[j] = rightAvg[j] * Convert.ToDouble(i) / Convert.ToDouble(i + 1) + training_label[tmpIDs[tmpIDs.Count - i - 1]][j] / Convert.ToDouble(i + 1);
                    tmp_err += leftErr[j] + rightErr[j];
                }

                if (lowest_err > tmp_err && training_dt[tmpIDs[tmpIDs.Count - i - 1]][dimIndex] != training_dt[tmpIDs[tmpIDs.Count - i - 2]][dimIndex]
                    && (i + 1) >= rc.minWaveSize && (i + rc.minWaveSize) < tmpIDs.Count)
                {
                    best_ID = tmpIDs[tmpIDs.Count - i - 1];
                    lowest_err = tmp_err;
                }
            }

            if (best_ID == -1)
            {
                error_n_point[0] = double.MaxValue;
                error_n_point[1] = double.MaxValue;
                return error_n_point;
            }

            error_n_point[0] = Math.Max(lowest_err, 0);
            error_n_point[1] = training_GridIndex_dt[best_ID][dimIndex];

            return error_n_point;
        }

        private bool GetGiniPartitionResult(ref int dimIndex, ref int Maingridindex, List<GeoWave> GeoWaveArr, int GeoWaveID, double Error, bool[] Dims2Take)
        {
            double[][] error_dim_partition = new double[2][];//information gain, Maingridindex
            error_dim_partition[0] = new double[rc.dim];
            error_dim_partition[1] = new double[rc.dim];

            //PARALLEL RUN - SEARCHING BEST PARTITION IN ALL DIMS
            if (Form1.rumPrallel)
            {
                Parallel.For(0, rc.dim, i =>
                {
                    if (Dims2Take[i])
                    {
                        double[] tmpResult = getGiniPartitionLargeDB(i, GeoWaveArr[GeoWaveID]);
                        error_dim_partition[0][i] = tmpResult[0];//information gain
                        error_dim_partition[1][i] = tmpResult[1];//Maingridindex                    
                    }
                    else
                    {
                        error_dim_partition[0][i] = double.MinValue;//information gain
                        error_dim_partition[1][i] = -1;//Maingridindex                    
                    }
                });
            }
            else
            {
                for (int i = 0; i < rc.dim; i++)
                {
                    if (Dims2Take[i])
                    {
                        double[] tmpResult = getGiniPartitionLargeDB(i, GeoWaveArr[GeoWaveID]);
                        error_dim_partition[0][i] = tmpResult[0];//information gain
                        error_dim_partition[1][i] = tmpResult[1];//Maingridindex                    
                    }
                    else
                    {
                        error_dim_partition[0][i] = double.MinValue;//information gain
                        error_dim_partition[1][i] = -1;//Maingridindex                    
                    }
                }
            }

            dimIndex = Enumerable.Range(0, error_dim_partition[0].Count())
                .Aggregate((a, b) => (error_dim_partition[0][a] > error_dim_partition[0][b]) ? a : b); //maximal gain (>)

            if (error_dim_partition[0][dimIndex] <= 0)
                return false;//if best partition doesn't help - return

            Maingridindex = Convert.ToInt32(error_dim_partition[1][dimIndex]);
            return true;
        }

        private double[] getGiniPartitionLargeDB(int dimIndex, GeoWave geoWave)
        {
            double[] error_n_point = new double[2];//gain index
            if (Form1.MainGrid[dimIndex].Count == 1)//empty feature
            {
                error_n_point[0] = double.MinValue;//min gain
                error_n_point[1] = -1;
                return error_n_point;
            }
            //sort ids (for labels) acording to position at Form1.MainGrid[dimIndex][index]
            List<int> tmpIDs = new List<int>(geoWave.pointsIdArray);
            tmpIDs.Sort(delegate(int c1, int c2) { return training_dt[c1][dimIndex].CompareTo(training_dt[c2][dimIndex]); });

            if (training_dt[tmpIDs[0]][dimIndex] == training_dt[tmpIDs[tmpIDs.Count - 1]][dimIndex])//all values are the same 
            {
                error_n_point[0] = double.MinValue;//min gain
                error_n_point[1] = -1;
                return error_n_point;
            }

            Dictionary<double, double> leftcategories = new Dictionary<double, double>(); //double as counter to enable devision
            Dictionary<double, double> rightcategories = new Dictionary<double, double>(); //double as counter to enable devision
            for (int i = 0; i < tmpIDs.Count(); i++)
            {
                if (leftcategories.ContainsKey(training_label[tmpIDs[i]][0]))
                    leftcategories[training_label[tmpIDs[i]][0]] += 1;
                else
                    leftcategories.Add(training_label[tmpIDs[i]][0], 1);
            }
            double N_points = Convert.ToDouble(tmpIDs.Count);
            double initialGini = calcGini(leftcategories, N_points);
            double NpointsLeft = N_points;
            double NpointsRight = 0;
            double leftGini = 0;
            double rightGini = 0;
            double gain = 0;
            double bestGain = 0;
            int best_ID = -1;

            for (int i = 0; i < tmpIDs.Count - 1; i++)//we dont calc the last (rightmost) boundary - it equal to the left most
            {
                double rightMostLable = training_label[tmpIDs[tmpIDs.Count - i - 1]][0];

                if (leftcategories[rightMostLable] == 1)
                    leftcategories.Remove(rightMostLable);
                else
                    leftcategories[rightMostLable] -= 1;

                if (rightcategories.ContainsKey(rightMostLable))
                    rightcategories[rightMostLable] += 1;
                else
                    rightcategories.Add(rightMostLable, 1);

                NpointsLeft -= 1;
                NpointsRight += 1;

                leftGini = calcGini(leftcategories, NpointsLeft);
                rightGini = calcGini(rightcategories, NpointsRight);

                gain = (initialGini - leftGini) * (NpointsLeft / N_points) + (initialGini - rightGini) * (NpointsRight / N_points);

                if (gain > bestGain && training_dt[tmpIDs[tmpIDs.Count - i - 1]][dimIndex] != training_dt[tmpIDs[tmpIDs.Count - i - 2]][dimIndex]
                    && (i + 1) >= rc.minWaveSize && (i + rc.minWaveSize) < tmpIDs.Count 
                    )
                {
                    best_ID = tmpIDs[tmpIDs.Count - i - 1];
                    bestGain = gain;
                }
            }

            if (best_ID == -1)
            {
                error_n_point[0] = double.MinValue;//min gain
                error_n_point[1] = -1;
                return error_n_point;
            }

            error_n_point[0] = bestGain;
            error_n_point[1] = training_GridIndex_dt[best_ID][dimIndex];

            return error_n_point;
        }

        private double calcGini(Dictionary<double, double> Totalcategories, double Npoints)
        {
            double gini = 0;
            for (int i = 0; i < Totalcategories.Count; i++)
            {
                gini += (Totalcategories.ElementAt(i).Value / Npoints) * (1 - (Totalcategories.ElementAt(i).Value / Npoints));
            }
            return gini;
        }

        private bool getRandPartitionResult(ref int dimIndex, ref int Maingridindex, List<GeoWave> GeoWaveArr, int GeoWaveID, double Error, int seed=0)
        {
            Random rnd0 = new Random(seed);
            int seedIndex = rnd0.Next(0, Int16.MaxValue/2); 

            Random rnd = new Random(seedIndex + GeoWaveID);

            int counter = 0;
            bool partitionFound= false;

            while(!partitionFound && counter < 20)
            {
                counter++;
                dimIndex = rnd.Next(0, GeoWaveArr[0].rc.dim); // creates a number between 0 and GeoWaveArr[0].rc.dim 
                int partition_ID = GeoWaveArr[GeoWaveID].pointsIdArray[rnd.Next(1, GeoWaveArr[GeoWaveID].pointsIdArray.Count() - 1)];

                Maingridindex = Convert.ToInt32(training_GridIndex_dt[partition_ID][dimIndex]);//this is dangerouse for Maingridindex > 2^32
                
                return true;
            }

            return false;
        }

        private void setChildrensPointsAndMeanValue(ref GeoWave child0, ref GeoWave child1, int dimIndex, List<int> indexArr)
        {
            child0.MeanValue.Multiply(0);
            child1.MeanValue.Multiply(0);

            //GO OVER ALL POINTS IN REGION
            for (int i = 0; i < indexArr.Count; i++)
            {
                if (training_dt[indexArr[i]][dimIndex] < Form1.MainGrid[dimIndex].ElementAt(child0.boubdingBox[1][dimIndex]))
                {
                    for (int j = 0; j < training_label[0].Count(); j++)
                        child0.MeanValue[j] += training_label[indexArr[i]][j];
                    child0.pointsIdArray.Add(indexArr[i]);
                }
                else
                {
                    for (int j = 0; j < training_label[0].Count(); j++)
                        child1.MeanValue[j] += training_label[indexArr[i]][j];
                    child1.pointsIdArray.Add(indexArr[i]);
                }
            }
            if(child0.pointsIdArray.Count > 0)
                child0.MeanValue = child0.MeanValue.Divide(Convert.ToDouble(child0.pointsIdArray.Count));
            if (child1.pointsIdArray.Count > 0)
                child1.MeanValue = child1.MeanValue.Divide(Convert.ToDouble(child1.pointsIdArray.Count));
        }

        private bool[] getDim2Take(recordConfig rc, int Seed)
        {
            bool[] Dim2Take = new bool[rc.dim];

            var ran = new Random(Seed);
            for (int i = 0; i < rc.NDimsinRF; i++)
            {
                //Dim2Take[dimArr[i]] = true;
                int index = ran.Next(0, rc.dim);
                if (Dim2Take[index] == true)
                    i--;
                else
                    Dim2Take[index] = true;
            }
            return Dim2Take;
        }
    }
}
