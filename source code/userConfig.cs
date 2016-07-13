using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Threading;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.IO;

namespace DataSetsSparsity
{
    public class userConfig
    {
        public void readConfig(string txtfile)
        { 
            if(!File.Exists(txtfile))
                return;
            StreamReader sr = new StreamReader(File.OpenRead(txtfile));

            string[] values = { "" };
            string line = "";

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                //values = line.Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                values = line.Split("=".ToArray(), StringSplitOptions.None);

                if (values[0] == "croosValidCB")
                    croosValidCB = values[1];
                else if (values[0] == "croosValidTB")
                    croosValidTB = values[1];
                else if (values[0] == "runRFPrunningCB")
                    runRFPrunningCB = values[1];
                else if (values[0] == "runRfCB")
                    runRfCB = values[1];
                else if (values[0] == "rumPrallelCB")
                    rumPrallelCB = values[1];
                else if (values[0] == "DBTB")
                    DBTB = values[1];
                else if (values[0] == "ResultsTB")
                    ResultsTB = values[1];
                else if (values[0] == "approxThreshTB")
                    approxThreshTB = values[1];
                else if (values[0] == "minNodeSizeTB")
                    minNodeSizeTB = values[1];
                else if (values[0] == "partitionTypeTB")
                    partitionTypeTB = values[1];
                else if (values[0] == "splitTypeTB")
                    splitTypeTB = values[1];
                else if (values[0] == "boundLevelTB")
                    boundLevelTB = values[1];
                else if (values[0] == "NrfTB")
                    NrfTB = values[1];
                else if (values[0] == "NfeaturesrfTB")
                    NfeaturesrfTB = values[1];
                else if (values[0] == "bagginPercentTB")
                    bagginPercentTB = values[1];
                else if (values[0] == "saveTressCB")
                    saveTressCB = values[1];
                else if (values[0] == "estimateFullRFCB")
                    estimateFullRFCB = values[1];
                else if (values[0] == "estimateRF4SmoothnessAnalysis")
                    estimateRF4SmoothnessAnalysis = values[1];
                else if (values[0] == "estimateRFwaveletsCB")
                    estimateRFwaveletsCB = values[1];
                else if (values[0] == "BaggingWithRepCB")
                    BaggingWithRepCB = values[1];
                else if (values[0] == "errTypeEstimationTB")
                    errTypeEstimationTB = values[1];
                else if (values[0] == "boundDepthTB")
                    boundDepthTB = values[1];
                else if (values[0] == "thresholdWaveletsCB")
                    thresholdWaveletsCB = values[1];
                else if (values[0] == "thresholdWaveletsTB")
                    thresholdWaveletsTB = values[1];
                else if (values[0] == "useClassificationCB")
                    useClassificationCB = values[1];

            }
            sr.Close();
        }
        public void printConfig(string fileName, S3FileInfo outFile)
        { 
            StreamWriter sw;
            if(outFile == null)
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                sw = new StreamWriter(fileName, false);
            }
            else
                sw = new StreamWriter(outFile.OpenWrite());
            
            sw.WriteLine("croosValidCB" + "=" + croosValidCB);
            sw.WriteLine("croosValidTB" + "=" + croosValidTB);
            sw.WriteLine("runRFPrunningCB" + "=" + runRFPrunningCB);
            sw.WriteLine("runRfCB" + "=" + runRfCB);
            sw.WriteLine("rumPrallelCB" + "=" + rumPrallelCB);
            sw.WriteLine("DBTB" + "=" + DBTB);
            sw.WriteLine("ResultsTB" + "=" + ResultsTB);
            sw.WriteLine("approxThreshTB" + "=" + approxThreshTB);
            sw.WriteLine("minNodeSizeTB" + "=" + minNodeSizeTB);
            sw.WriteLine("partitionTypeTB" + "=" + partitionTypeTB);
            sw.WriteLine("splitTypeTB" + "=" + splitTypeTB);
            sw.WriteLine("boundLevelTB" + "=" + boundLevelTB);
            sw.WriteLine("errTypeEstimationTB" + "=" + errTypeEstimationTB);
            sw.WriteLine("trainingPercentTB" + "=" + trainingPercentTB);
            sw.WriteLine("NrfTB" + "=" + NrfTB);
            sw.WriteLine("NfeaturesrfTB" + "=" + NfeaturesrfTB);
            sw.WriteLine("bagginPercentTB" + "=" + bagginPercentTB);
            sw.WriteLine("saveTressCB" + "=" + saveTressCB);
            sw.WriteLine("runOneTreeCB" + "=" + runOneTreeCB);
            sw.WriteLine("estimateFullRFCB" + "=" + estimateFullRFCB);   
            sw.WriteLine("estimateRF4SmoothnessAnalysis" + "=" + estimateRF4SmoothnessAnalysis);
            sw.WriteLine("estimateRFwaveletsCB" + "=" + estimateRFwaveletsCB);
            sw.WriteLine("BaggingWithRepCB" + "=" + BaggingWithRepCB); 
            sw.WriteLine("boundDepthTB" + "=" + boundDepthTB);
            sw.WriteLine("thresholdWaveletsCB" + "=" + thresholdWaveletsCB);
            sw.WriteLine("thresholdWaveletsTB" + "=" + thresholdWaveletsTB); 
            sw.WriteLine("useClassificationCB" + "=" + useClassificationCB);

            sw.Close();
        }

        public string croosValidCB;
        public string croosValidTB;
        public string runRFPrunningCB;
        public string runRfCB;
        public string rumPrallelCB;
        public string DBTB;
        public string ResultsTB;
        public string approxThreshTB;
        public string minNodeSizeTB;
        public string partitionTypeTB;
        public string splitTypeTB;
        public string boundLevelTB;
        public string errTypeEstimationTB;
        public string trainingPercentTB;
        public string NrfTB;
        public string NfeaturesrfTB;
        public string bagginPercentTB;
        public string saveTressCB;
        public string runOneTreeCB;
        public string estimateFullRFCB;
        public string estimateRF4SmoothnessAnalysis;
        public string estimateRFwaveletsCB;
        public string BaggingWithRepCB;
        public string boundDepthTB;   
        public string thresholdWaveletsCB;
        public string thresholdWaveletsTB;
        public string useClassificationCB; 
     }
 }
 
 
 