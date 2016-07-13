using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSetsSparsity
{

    public class recordConfig
    {
        //CONSTRUCTOR
        public recordConfig(){}

        public int dim;
        public double approxThresh;
        public int partitionErrType;
        public int approxOrder;
        public int rfNum;
        public double rfBaggingPercent;
        public int minWaveSize;
        public int BoundLevel;
        public int NDimsinRF;
        public int split_type;//0 regular l2 partition, 1 rand split, 2 rand feature in each node, 3 gini split, 4 gini split + rand nodes, 5 floody connections 
        public int NormLPTypeInEstimation;//0 classification, 1 L1, 2 L2
        public int CrossValidFold;
        public int boundDepthTree;

        public string getFullName()
        {
            string name =

            "dim_" + dim.ToString()
            + "_appThsh_" + approxThresh.ToString()
            + "_partErrType_" + partitionErrType.ToString()
            + "_appOrdr_" + approxOrder.ToString()
            + "_rfNum_" + rfNum.ToString()
            + "_rfBagPercnt_" + rfBaggingPercent.ToString()
            + "_minWaveSize_" + minWaveSize.ToString()
            + "_BoundLevel_" + BoundLevel.ToString()
            + "_NDimsinRF_" + NDimsinRF.ToString()
            + "_NDimsinRF_" + split_type.ToString()
            + "_NormLPTypeInEstimation_" + NormLPTypeInEstimation.ToString()
            + "_boundDepthTree_" + boundDepthTree.ToString()
            + "_CrossValidFold_" + CrossValidFold.ToString();
            return name;
        }

        public string getShortName()
        {
            string name =

            dim.ToString()
            + "_" + approxThresh.ToString()
            + "_" + partitionErrType.ToString()
            + "_" + approxOrder.ToString()
            + "_" + rfNum.ToString()
            + "_" + rfBaggingPercent.ToString()
            + "_" + minWaveSize.ToString()
            + "_" + BoundLevel.ToString()
            + "_" + NDimsinRF.ToString()
            + "_" + split_type.ToString()
            + "_" + boundDepthTree.ToString()
            + "_" + CrossValidFold.ToString();
            return name;
        }
    }
}
