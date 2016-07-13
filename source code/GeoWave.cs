using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Math;

namespace DataSetsSparsity
{
    public class GeoWave
    {
        public int parentID, child0, child1, level;
        public double proximity;
        public double norm;
        public double[] MeanValue; //vector with means at each dimention
        public int[][] boubdingBox;
        public List<int> pointsIdArray = new List<int>();//points in regeion (row index of static input data)
        public int ID;
        public recordConfig rc;
        public int dimIndex;//of partition
        public int Maingridindex;//of partition
        public double MaingridValue;//of partition
        public int dimIndexSplitter;//of partition
        public double splitValue;//of partition

        public GeoWave(int dataDim, int labelDim, recordConfig rc)
        {
            this.rc = rc;
            Init(dataDim, labelDim);
        }

        private void Init(int dataDim, int labelDim)
        {
            parentID = -1;
            child0 = -1;
            child1 = -1;
            level = -1;
            norm = -1;
            //approx_solution = new double[dataDim, labelDim];
            boubdingBox = new int[2][];
            boubdingBox[0] = new int[dataDim];
            boubdingBox[1] = new int[dataDim];

            MeanValue = new double[labelDim];
            ID = -1;
            dimIndex = -1;
            Maingridindex = -1;
            MaingridValue = -1;
        }

        public GeoWave(int[][] BOX, int labelDim, recordConfig rc)
        {
            this.rc = rc;
            Init(rc.dim, labelDim);
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < rc.dim; j++)
                    boubdingBox[i][j] = BOX[i][j];
        }

        public double[] calc_MeanValue(double[][] Labels_dt, List<int> indexArr)
        {
            double[] tmpMeanValue = new double[MeanValue.Count()];

            //GO OVER ALL POINTS IN REGION
            foreach (int index in indexArr)
            {
                for (int j = 0; j < Labels_dt[0].Count(); j++)
                    tmpMeanValue[j] += Labels_dt[index][j];
            }

            if (indexArr.Count * Labels_dt[0].Count() > 0)
                tmpMeanValue = tmpMeanValue.Divide(indexArr.Count);

            return tmpMeanValue;
        }
        //calculate sum of distances from mean vector
        public double calc_MeanValueReturnError(double[][] labelsDt, List<int> indexArr)
        {
            //NULLIFY
            double[] tmpMeanValue = calc_MeanValue(labelsDt, indexArr);
            double Error = 0;

            switch (rc.partitionErrType)
            {
                case 2: //L2
                    foreach (int index in indexArr)
                    {
                        for (int j = 0; j < labelsDt[0].Count(); j++)
                            Error += (labelsDt[index][j] - tmpMeanValue[j]) * (labelsDt[index][j] - tmpMeanValue[j]);//insert rc of norm type
                    }
                    return Error;
                case 1: //L1
                    foreach (int index in indexArr)
                    {
                        for (int j = 0; j < labelsDt[0].Count(); j++)
                            Error += Math.Abs(labelsDt[index][j] - tmpMeanValue[j]);
                    }
                    return Error;
                case 0:
                    foreach (int index in indexArr)
                    {
                        double sign = 0;
                        for (int j = 0; j < labelsDt[0].Count(); j++)
                            sign += Math.Abs(labelsDt[index][j] - tmpMeanValue[j]); ;
                        if (sign != 0)
                            Error += 1;
                    }
                    return Error;
                default:
                    {
                        double p = rc.partitionErrType;
                        //GO OVER ALL POINTS IN REGION
                        foreach (int index in indexArr)
                        {
                            for (int j = 0; j < labelsDt[0].Count(); j++)
                                Error += Math.Pow(labelsDt[index][j] - tmpMeanValue[j], p);
                        }
                        return Error;//same as l2 don't do sqrt  
                    }
            }
        }

        public double[] calc_MeanValueReturnError(double[][] labelsDt, List<int> indexArr, ref double[] calcedMeanValue)
        {
            //NULLIFY
            calcedMeanValue = calc_MeanValue(labelsDt, indexArr);
            double[] error = new double[labelsDt[0].Count()];

            switch (rc.partitionErrType)
            {
                case 2://L2
                    foreach (int index in indexArr)
                    {
                        for (int j = 0; j < labelsDt[0].Count(); j++)
                            error[j] += (labelsDt[index][j] - calcedMeanValue[j]) * (labelsDt[index][j] - calcedMeanValue[j]);//insert rc of norm type
                    }
                    return error;
                case 1://L1
                    foreach (int index in indexArr)
                    {
                        for (int j = 0; j < labelsDt[0].Count(); j++)
                            error[j] += Math.Abs(labelsDt[index][j] - calcedMeanValue[j]);
                    }
                    return error;
                case 0:
                    MessageBox.Show("shoud not get here rc.partitionErrType == 0");
                    for (int i = 0; i < indexArr.Count; i++)
                    {
                        double sign = 0;
                    }
                    return error;
                default:
                    {
                        double p = rc.partitionErrType;
                        //GO OVER ALL POINTS IN REGION
                        for (int i = 0; i < indexArr.Count; i++)
                        {
                            for (int j = 0; j < labelsDt[0].Count(); j++)
                                error[j] += Math.Pow(labelsDt[indexArr[i]][j] - calcedMeanValue[j], p);
                        }

                        return error;//same as l2 don't do sqrt  
                    }
            }
        }
        public void computeNormOfConsts(GeoWave parent, double Lp)
        {
            norm = 0;
            //GO OVER ALL POINTS IN THE REGION
            if (Lp == 2)
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += ((MeanValue[j] - parent.MeanValue[j]) * (MeanValue[j] - parent.MeanValue[j]));
                norm = norm * pointsIdArray.Count();
                norm = Math.Sqrt(norm);
            }
            else if (Lp == 1)
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += Math.Abs(MeanValue[j] - parent.MeanValue[j]);
                norm = norm * pointsIdArray.Count();
            }
            else
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += Math.Pow(MeanValue[j] - parent.MeanValue[j], Lp);
                norm = norm * pointsIdArray.Count();
                norm = Math.Pow(norm, 1 / Lp);
            }
        }

        public void computeNormOfConsts(double Lp)
        {
            norm = 0;
            if (Lp == 2)
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += (MeanValue[j] * MeanValue[j]);
                norm *= pointsIdArray.Count();
                norm = Math.Sqrt(norm);
            }
            else if (Lp == 1)
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += Math.Abs(MeanValue[j]);
                norm *= pointsIdArray.Count();
            }
            else
            {
                for (int j = 0; j < MeanValue.Count(); j++)
                    norm += Math.Pow(MeanValue[j], Lp);
                norm *= pointsIdArray.Count();
                norm = Math.Pow(norm, 1 / Lp);
            }


        }
    }
}
