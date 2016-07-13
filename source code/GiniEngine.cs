using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataScienceAnalysis
{
    class GiniEngine
    {
        public enum NormType { Gini, CrossEntropy, MostOccuring }
        private readonly double[][] _labels;
        private readonly double[][] _training;
        private readonly long[][] _trainingGridIndex;
        private readonly int _labelsDim;
        private readonly int _dataDim;
        private readonly int _minWaveSize;
        private readonly NormType _mode;

        private const int VALUE = 0;
        private const int SPLIT_INDEX = 1;

        public GiniEngine(recordConfig rc, DB db, NormType mode = NormType.Gini)
        {
            _mode = mode;
            _labels = db.training_label;
            _training = db.training_dt;
            _trainingGridIndex = db.PCAtraining_GridIndex_dt;
            _dataDim = rc.dim;
            _minWaveSize = rc.minWaveSize;
            _labelsDim = _labels[0].Count();
        }

        public double UpdateNodeGini(ref GeoWave node, GeoWave parent = null)
        {
            Dictionary<double, double>[] dicLabelProb = CalculateLabelsAmount(node);
            double result = 0;
            switch (_mode)
            {
                case NormType.Gini:
                    result = CalculateGini(dicLabelProb, ref node, parent);
                    break;
                case NormType.MostOccuring:
                    break;
                case NormType.CrossEntropy:
                    break;

            }

            return result;
        }
        private double CalculateGini(Dictionary<double, double>[] dicLabelCount, ref GeoWave node, GeoWave parent = null)
        {
            var vecGini = new double[_labelsDim];
            double giniNorm = 0;
            for (int dim = 0; dim < _labelsDim; dim++)
            {
                foreach (var dimDic in dicLabelCount[dim])
                {
                    //label probability
                    double labelProb = dimDic.Value / node.pointsIdArray.Count();
                    vecGini[dim] += labelProb * (1 - labelProb);
                }
                //save <label,amount> dictionary array
                node.MgStuff.dicLabelCount[dim] = new Dictionary<double, double>(dicLabelCount[dim]);
            }
            //save gini average value
            node.MgStuff.GiniAvg = vecGini.Sum() / _labelsDim;
            //save gini vector
            Array.Copy(vecGini, node.MgStuff.GiniVector, _labelsDim);
            //save gini norm
            double parentGiniAvg = (parent != null) ? parent.MgStuff.GiniAvg : 0;
            giniNorm = (node.MgStuff.GiniAvg - parentGiniAvg) * (node.MgStuff.GiniAvg - parentGiniAvg) * node.pointsIdArray.Count();
            node.MgStuff.GiniNorm = giniNorm;
            return giniNorm;
        }
        // get node, return array of dictionaries <label, amount>[i]  (i is label dimention)
        public Dictionary<double, double>[] CalculateLabelsAmount(GeoWave node)
        {

            var labelsDic = new Dictionary<double, double>[_labelsDim];
            var nodeLabels = node.pointsIdArray.Select(id => _labels[id]).ToArray();
            //go throw label dimensions
            for (int dim = 0; dim < _labelsDim; dim++)
            {
                labelsDic[dim] = new Dictionary<double, double>();
                //build single dimmention labels array
                var dimIlabels = new double[nodeLabels.Count()];
                for (int ind = 0; ind < nodeLabels.Count(); ind++)
                {
                    dimIlabels[ind] = nodeLabels[ind][dim];
                }
                //count distinct labels at each dimension
                foreach (var label in dimIlabels.Distinct())
                {
                    double curLabel = label;
                    //count equal labels at this dimention
                    int countLabelDimI = dimIlabels.Where(value => Math.Abs(value - curLabel) < Double.Epsilon).Count();
                    labelsDic[dim].Add(label, countLabelDimI);
                }
            }
            return labelsDic;
        }
        //*****************************************************************************************************************************
        //*************************************************** PARTION ******************************************************************
        //*******************************************************************************************************************************

        public bool GetGiniPartitionResult(ref int dimIndex, ref int mainGridIndex, List<GeoWave> geoWaveArr, int geoWaveId, double error, bool[] dims2Take)
        {
            var errorDimPartition = new double[2][];//error, Maingridindex
            errorDimPartition[VALUE] = new double[_dataDim];
            errorDimPartition[SPLIT_INDEX] = new double[_dataDim];
            for (int dim = 0; dim < _dataDim; dim++)
            {
                if (dims2Take[dim])
                {
                    //find best partion at dimension i
                    double[] tmpResult = GetBestPartionAtSingleDim(dim, geoWaveArr[geoWaveId]);
                    errorDimPartition[VALUE][dim] = tmpResult[VALUE];
                    errorDimPartition[SPLIT_INDEX][dim] = tmpResult[SPLIT_INDEX];
                }
                else
                {
                    errorDimPartition[VALUE][dim] = double.MaxValue;//error
                    errorDimPartition[SPLIT_INDEX][dim] = -1;//Maingridindex                    
                }
            }
            //find index with smallest gini average
            dimIndex = Enumerable.Range(0, errorDimPartition[0].Count())
                .Aggregate((a, b) => (errorDimPartition[0][a] < errorDimPartition[0][b]) ? a : b);

            if (errorDimPartition[0][dimIndex] >= error)
                return false;//if best partition doesn't help - return

            mainGridIndex = Convert.ToInt32(errorDimPartition[SPLIT_INDEX][dimIndex]);
            return true;
        }

        private double[] GetBestPartionAtSingleDim(int dim, GeoWave node)
        {
            var errorNPoint = new double[2];//error index
            int bestId = -1;
            if (Form1.MainGrid[dim].Count == 1)//empty feature
            {
                errorNPoint[VALUE] = double.MaxValue;
                errorNPoint[SPLIT_INDEX] = -1;
                return errorNPoint;
            }
            //sort ids (for labels) acording to position at Form1.MainGrid[dimIndex][index] at 'dim' dimention
            var sortedIds = new List<int>(node.pointsIdArray);
            sortedIds.Sort((c1, c2) => _training[c1][dim].CompareTo(_training[c2][dim]));

            if (Math.Abs(_training[sortedIds[0]][dim] - _training[sortedIds[sortedIds.Count - 1]][dim]) < double.Epsilon)//all values are the same 
            {
                errorNPoint[VALUE] = double.MaxValue;
                errorNPoint[SPLIT_INDEX] = -1;
                return errorNPoint;
            }
            var leftDicClone = GiniHelper.CloneLabelAmountDic(node.MgStuff.dicLabelCount); //start with parent data at left
            var rightDic = GiniHelper.CreateEmptyLabelAmountDic(_labelsDim);
            var startLeftLabelsAmount = node.pointsIdArray.Count();
            int bestSplitId = -1;
            var parentSize = startLeftLabelsAmount;
            double giniLowest = node.MgStuff.GiniAvg;

            for (var i = 0; i < sortedIds.Count() - 1; i++)
            {
                var sortedId = sortedIds[startLeftLabelsAmount - i - 1];
                var nextSortedId = sortedIds[startLeftLabelsAmount - i - 2];
                double[] movedLabel = _labels[sortedId];
                var leftSize = parentSize - i - 1;
                var rightSize = i + 1;
                var leftGini = GiniHelper.GetGiniByAction(leftDicClone, movedLabel, GiniHelper.ActionType.Remove, leftSize);
                var rightGini = GiniHelper.GetGiniByAction(rightDic, movedLabel, GiniHelper.ActionType.Insert, rightSize);
                double tempGiniPartion = ((double)leftSize / parentSize) * leftGini + ((double)rightSize / parentSize) * rightGini;
                //in case some points has the same values - we calc the avarage (relevant for splitting) only after all the points (with same values) had moved to the right
                //we don't alow "improving" the same split with two points with the same position (sort is not unique)

                double nowMovedValue = _training[sortedId][dim];
                double nextMovedValue = _training[nextSortedId][dim];
                if (tempGiniPartion < giniLowest && nowMovedValue != nextMovedValue
                    && (i + 1) >= _minWaveSize && (i + _minWaveSize) < sortedIds.Count)
                {
                    giniLowest = tempGiniPartion;
                    bestSplitId = sortedIds[sortedIds.Count() - i - 1];
                }
            }
            if (bestSplitId == -1)
            {
                errorNPoint[VALUE] = double.MaxValue;
                errorNPoint[SPLIT_INDEX] = double.MaxValue;
                return errorNPoint;
            }

            errorNPoint[VALUE] = Math.Max(giniLowest, 0);
            errorNPoint[SPLIT_INDEX] = _trainingGridIndex[bestSplitId][dim];
            return errorNPoint;
        }


    }
}

public static class GiniHelper
{
    public enum ActionType
    {
        Insert,
        Remove
    }
    public static double GetGiniByAction(Dictionary<double, double>[] dicLabelAmount, double[] singleLabel, GiniHelper.ActionType action, int numOfLabels)
    {
        var labelDim = singleLabel.Count();
        UpdateDicByAction(dicLabelAmount, singleLabel, action);
        return GetGiniValueByDictionary(dicLabelAmount, numOfLabels);
    }
    // construct clone dictionary by insert or remove single label
    public static void UpdateDicByAction(Dictionary<double, double>[] dicLabelAmount, double[] singleLabel, GiniHelper.ActionType action)
    {
        int labelDim = dicLabelAmount.Count();
        for (int dim = 0; dim < labelDim; dim++)
        {

            var dimLabel = singleLabel[dim];
            if (dicLabelAmount[dim].ContainsKey(dimLabel))
            {
                switch (action)
                {
                    case ActionType.Insert:
                        dicLabelAmount[dim][dimLabel]++;
                        break;
                    case ActionType.Remove:
                        dicLabelAmount[dim][dimLabel]--;
                        break;
                }
            }
            else
            {
                switch (action)
                {
                    case ActionType.Insert:
                        dicLabelAmount[dim][dimLabel] = 1;
                        break;
                    case ActionType.Remove:
                        // label not in a dic, do nothing
                        break;
                }
            }
            //clean from dic labels with zero amount
            foreach (var item in dicLabelAmount[dim].Where(kvp => kvp.Value == 0).ToList())
            {
                dicLabelAmount[dim].Remove(item.Key);
            }
        }
    }
    //get dictionary array <label,amount>[i] and number of labels
    public static double GetGiniValueByDictionary(Dictionary<double, double>[] dicLabelAmount, int numOfLabels)
    {
        int labelDim = dicLabelAmount.Count();
        var vecGini = new double[labelDim];
        for (int dim = 0; dim < labelDim; dim++)
        {
            foreach (var dimDic in dicLabelAmount[dim])
            {
                //label probability
                double labelProb = dimDic.Value / numOfLabels;
                vecGini[dim] += labelProb * (1 - labelProb);
            }
        }
        //save gini average value
        return vecGini.Sum() / labelDim;
    }

    public static Dictionary<double, double>[] CloneLabelAmountDic(Dictionary<double, double>[] origin)
    {
        var labelDim = origin.Count();
        var cloneDic = new Dictionary<double, double>[labelDim];
        for (int i = 0; i < labelDim; i++)
        {
            cloneDic[i] = new Dictionary<double, double>(origin[i]);
        }
        return cloneDic;
    }

    public static Dictionary<double, double>[] CreateEmptyLabelAmountDic(int labelDim)
    {
        var emptyDic = new Dictionary<double, double>[labelDim];
        for (int i = 0; i < labelDim; i++)
        {
            emptyDic[i] = new Dictionary<double, double>();
        }
        return emptyDic;
    }
}