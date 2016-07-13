namespace DataSetsSparsity
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        /// <summary>
        /// Clean up any resources being used.
        private System.ComponentModel.IContainer components = null;

        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.useClassificationCB = new System.Windows.Forms.CheckBox();
            this.thresholdWaveletsCB = new System.Windows.Forms.CheckBox();
            this.thresholdWaveletsTB = new System.Windows.Forms.TextBox();
            this.estimateRFwaveletsCB = new System.Windows.Forms.CheckBox();
            this.estimateRF4SmoothnessAnalysis = new System.Windows.Forms.CheckBox();
            this.estimateFullRFCB = new System.Windows.Forms.CheckBox();
            this.saveTressCB = new System.Windows.Forms.CheckBox();
            this.croosValidCB = new System.Windows.Forms.CheckBox();
            this.runRFPrunningCB = new System.Windows.Forms.CheckBox();
            this.croosValidTB = new System.Windows.Forms.TextBox();
            this.runRfCB = new System.Windows.Forms.CheckBox();
            this.rumPrallelCB = new System.Windows.Forms.CheckBox();
            this.btnScript = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DBTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ResultsTB = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.approxThreshTB = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.minNodeSizeTB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.partitionTypeTB = new System.Windows.Forms.TextBox();
            this.splitTypeTB = new System.Windows.Forms.TextBox();
            this.boundLevelTB = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.NrfTB = new System.Windows.Forms.TextBox();
            this.boundDepthTB = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.NfeaturesrfTB = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.bagginPercentTB = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.errTypeEstimationTB = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.useClassificationCB);
            this.groupBox1.Controls.Add(this.thresholdWaveletsCB);
            this.groupBox1.Controls.Add(this.thresholdWaveletsTB);
            this.groupBox1.Controls.Add(this.estimateRFwaveletsCB);
            this.groupBox1.Controls.Add(this.estimateRF4SmoothnessAnalysis);
            this.groupBox1.Controls.Add(this.estimateFullRFCB);
            this.groupBox1.Controls.Add(this.saveTressCB);
            this.groupBox1.Controls.Add(this.croosValidCB);
            this.groupBox1.Controls.Add(this.runRFPrunningCB);
            this.groupBox1.Controls.Add(this.croosValidTB);
            this.groupBox1.Controls.Add(this.runRfCB);
            this.groupBox1.Controls.Add(this.rumPrallelCB);
            this.groupBox1.Location = new System.Drawing.Point(16, 135);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(399, 235);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Script Config";
            // 
            // useClassificationCB
            // 
            this.useClassificationCB.AutoSize = true;
            this.useClassificationCB.Location = new System.Drawing.Point(8, 207);
            this.useClassificationCB.Margin = new System.Windows.Forms.Padding(4);
            this.useClassificationCB.Name = "useClassificationCB";
            this.useClassificationCB.Size = new System.Drawing.Size(273, 21);
            this.useClassificationCB.TabIndex = 66;
            this.useClassificationCB.Text = "use Classification (regression is defult)";
            this.useClassificationCB.UseVisualStyleBackColor = true;
            // 
            // thresholdWaveletsCB
            // 
            this.thresholdWaveletsCB.AutoSize = true;
            this.thresholdWaveletsCB.Location = new System.Drawing.Point(8, 149);
            this.thresholdWaveletsCB.Margin = new System.Windows.Forms.Padding(4);
            this.thresholdWaveletsCB.Name = "thresholdWaveletsCB";
            this.thresholdWaveletsCB.Size = new System.Drawing.Size(151, 21);
            this.thresholdWaveletsCB.TabIndex = 70;
            this.thresholdWaveletsCB.Text = "threshold Wavelets";
            this.thresholdWaveletsCB.UseVisualStyleBackColor = true;
            // 
            // thresholdWaveletsTB
            // 
            this.thresholdWaveletsTB.Location = new System.Drawing.Point(173, 149);
            this.thresholdWaveletsTB.Margin = new System.Windows.Forms.Padding(4);
            this.thresholdWaveletsTB.Name = "thresholdWaveletsTB";
            this.thresholdWaveletsTB.Size = new System.Drawing.Size(79, 22);
            this.thresholdWaveletsTB.TabIndex = 69;
            // 
            // estimateRFwaveletsCB
            // 
            this.estimateRFwaveletsCB.AutoSize = true;
            this.estimateRFwaveletsCB.Location = new System.Drawing.Point(173, 177);
            this.estimateRFwaveletsCB.Margin = new System.Windows.Forms.Padding(4);
            this.estimateRFwaveletsCB.Name = "estimateRFwaveletsCB";
            this.estimateRFwaveletsCB.Size = new System.Drawing.Size(179, 21);
            this.estimateRFwaveletsCB.TabIndex = 64;
            this.estimateRFwaveletsCB.Text = "estimate RF of wavelets";
            this.estimateRFwaveletsCB.UseVisualStyleBackColor = true;
            // 
            // estimateRF4SmoothnessAnalysis
            // 
            this.estimateRF4SmoothnessAnalysis.AutoSize = true;
            this.estimateRF4SmoothnessAnalysis.Location = new System.Drawing.Point(173, 92);
            this.estimateRF4SmoothnessAnalysis.Margin = new System.Windows.Forms.Padding(4);
            this.estimateRF4SmoothnessAnalysis.Name = "estimateRF4SmoothnessAnalysis";
            this.estimateRF4SmoothnessAnalysis.Size = new System.Drawing.Size(165, 21);
            this.estimateRF4SmoothnessAnalysis.TabIndex = 64;
            this.estimateRF4SmoothnessAnalysis.Text = "estimate Smoothness";
            this.estimateRF4SmoothnessAnalysis.UseVisualStyleBackColor = true;
            // 
            // estimateFullRFCB
            // 
            this.estimateFullRFCB.AutoSize = true;
            this.estimateFullRFCB.Location = new System.Drawing.Point(173, 63);
            this.estimateFullRFCB.Margin = new System.Windows.Forms.Padding(4);
            this.estimateFullRFCB.Name = "estimateFullRFCB";
            this.estimateFullRFCB.Size = new System.Drawing.Size(197, 21);
            this.estimateFullRFCB.TabIndex = 63;
            this.estimateFullRFCB.Text = "estimate RF (no Wavelets)";
            this.estimateFullRFCB.UseVisualStyleBackColor = true;
            // 
            // saveTressCB
            // 
            this.saveTressCB.AutoSize = true;
            this.saveTressCB.Location = new System.Drawing.Point(8, 178);
            this.saveTressCB.Margin = new System.Windows.Forms.Padding(4);
            this.saveTressCB.Name = "saveTressCB";
            this.saveTressCB.Size = new System.Drawing.Size(161, 21);
            this.saveTressCB.TabIndex = 61;
            this.saveTressCB.Text = "save trees in archive";
            this.saveTressCB.UseVisualStyleBackColor = true;
            // 
            // croosValidCB
            // 
            this.croosValidCB.AutoSize = true;
            this.croosValidCB.Location = new System.Drawing.Point(8, 120);
            this.croosValidCB.Margin = new System.Windows.Forms.Padding(4);
            this.croosValidCB.Name = "croosValidCB";
            this.croosValidCB.Size = new System.Drawing.Size(159, 21);
            this.croosValidCB.TabIndex = 60;
            this.croosValidCB.Text = "Fold cross validation";
            this.croosValidCB.UseVisualStyleBackColor = true;
            // 
            // runRFPrunningCB
            // 
            this.runRFPrunningCB.AutoSize = true;
            this.runRFPrunningCB.Location = new System.Drawing.Point(8, 63);
            this.runRFPrunningCB.Margin = new System.Windows.Forms.Padding(4);
            this.runRFPrunningCB.Name = "runRFPrunningCB";
            this.runRFPrunningCB.Size = new System.Drawing.Size(134, 21);
            this.runRFPrunningCB.TabIndex = 7;
            this.runRFPrunningCB.Text = "run RF Prunning";
            this.runRFPrunningCB.UseVisualStyleBackColor = true;
            // 
            // croosValidTB
            // 
            this.croosValidTB.Location = new System.Drawing.Point(173, 117);
            this.croosValidTB.Margin = new System.Windows.Forms.Padding(4);
            this.croosValidTB.Name = "croosValidTB";
            this.croosValidTB.Size = new System.Drawing.Size(79, 22);
            this.croosValidTB.TabIndex = 59;
            // 
            // runRfCB
            // 
            this.runRfCB.AutoSize = true;
            this.runRfCB.Location = new System.Drawing.Point(8, 89);
            this.runRfCB.Margin = new System.Windows.Forms.Padding(4);
            this.runRfCB.Name = "runRfCB";
            this.runRfCB.Size = new System.Drawing.Size(65, 21);
            this.runRfCB.TabIndex = 3;
            this.runRfCB.Text = "runRf";
            this.runRfCB.UseVisualStyleBackColor = true;
            // 
            // rumPrallelCB
            // 
            this.rumPrallelCB.AutoSize = true;
            this.rumPrallelCB.Location = new System.Drawing.Point(8, 34);
            this.rumPrallelCB.Margin = new System.Windows.Forms.Padding(4);
            this.rumPrallelCB.Name = "rumPrallelCB";
            this.rumPrallelCB.Size = new System.Drawing.Size(93, 21);
            this.rumPrallelCB.TabIndex = 1;
            this.rumPrallelCB.Text = "rumPrallel";
            this.rumPrallelCB.UseVisualStyleBackColor = true;
            // 
            // btnScript
            // 
            this.btnScript.Location = new System.Drawing.Point(321, 397);
            this.btnScript.Margin = new System.Windows.Forms.Padding(4);
            this.btnScript.Name = "btnScript";
            this.btnScript.Size = new System.Drawing.Size(211, 58);
            this.btnScript.TabIndex = 21;
            this.btnScript.Text = "Run Script !!!";
            this.btnScript.UseVisualStyleBackColor = true;
            this.btnScript.Click += new System.EventHandler(this.btnScript_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.DBTB);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ResultsTB);
            this.groupBox2.Location = new System.Drawing.Point(16, 22);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(399, 97);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "I.O Config";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "DB path";
            // 
            // DBTB
            // 
            this.DBTB.Location = new System.Drawing.Point(121, 26);
            this.DBTB.Margin = new System.Windows.Forms.Padding(4);
            this.DBTB.Name = "DBTB";
            this.DBTB.Size = new System.Drawing.Size(257, 22);
            this.DBTB.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Results path";
            // 
            // ResultsTB
            // 
            this.ResultsTB.Location = new System.Drawing.Point(120, 62);
            this.ResultsTB.Margin = new System.Windows.Forms.Padding(4);
            this.ResultsTB.Name = "ResultsTB";
            this.ResultsTB.Size = new System.Drawing.Size(257, 22);
            this.ResultsTB.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 93);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 17);
            this.label10.TabIndex = 28;
            this.label10.Text = "% RF Bagging";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 124);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(165, 17);
            this.label7.TabIndex = 16;
            this.label7.Text = "Approximation Threshold";
            // 
            // approxThreshTB
            // 
            this.approxThreshTB.Location = new System.Drawing.Point(195, 119);
            this.approxThreshTB.Margin = new System.Windows.Forms.Padding(4);
            this.approxThreshTB.Name = "approxThreshTB";
            this.approxThreshTB.Size = new System.Drawing.Size(63, 22);
            this.approxThreshTB.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 189);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 17);
            this.label8.TabIndex = 20;
            this.label8.Text = "Min Node Size";
            // 
            // minNodeSizeTB
            // 
            this.minNodeSizeTB.Location = new System.Drawing.Point(196, 184);
            this.minNodeSizeTB.Margin = new System.Windows.Forms.Padding(4);
            this.minNodeSizeTB.Name = "minNodeSizeTB";
            this.minNodeSizeTB.Size = new System.Drawing.Size(63, 22);
            this.minNodeSizeTB.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 160);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 17);
            this.label9.TabIndex = 18;
            this.label9.Text = "Partition ErrorType";
            // 
            // partitionTypeTB
            // 
            this.partitionTypeTB.Location = new System.Drawing.Point(196, 151);
            this.partitionTypeTB.Margin = new System.Windows.Forms.Padding(4);
            this.partitionTypeTB.Name = "partitionTypeTB";
            this.partitionTypeTB.Size = new System.Drawing.Size(63, 22);
            this.partitionTypeTB.TabIndex = 17;
            // 
            // splitTypeTB
            // 
            this.splitTypeTB.Location = new System.Drawing.Point(196, 248);
            this.splitTypeTB.Margin = new System.Windows.Forms.Padding(4);
            this.splitTypeTB.Name = "splitTypeTB";
            this.splitTypeTB.Size = new System.Drawing.Size(63, 22);
            this.splitTypeTB.TabIndex = 23;
            // 
            // boundLevelTB
            // 
            this.boundLevelTB.Location = new System.Drawing.Point(196, 217);
            this.boundLevelTB.Margin = new System.Windows.Forms.Padding(4);
            this.boundLevelTB.Name = "boundLevelTB";
            this.boundLevelTB.Size = new System.Drawing.Size(63, 22);
            this.boundLevelTB.TabIndex = 21;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(8, 252);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(66, 17);
            this.label24.TabIndex = 48;
            this.label24.Text = "Split type";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(11, 221);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(180, 17);
            this.label26.TabIndex = 44;
            this.label26.Text = "Bound Level (in estimation)";
            // 
            // groupBox4
            // 
            this.groupBox4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox4.Controls.Add(this.NrfTB);
            this.groupBox4.Controls.Add(this.boundDepthTB);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.NfeaturesrfTB);
            this.groupBox4.Controls.Add(this.approxThreshTB);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.bagginPercentTB);
            this.groupBox4.Controls.Add(this.partitionTypeTB);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.minNodeSizeTB);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label23);
            this.groupBox4.Controls.Add(this.errTypeEstimationTB);
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.boundLevelTB);
            this.groupBox4.Controls.Add(this.splitTypeTB);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Location = new System.Drawing.Point(443, 22);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(290, 348);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Parameters settings";
            // 
            // NrfTB
            // 
            this.NrfTB.Location = new System.Drawing.Point(197, 30);
            this.NrfTB.Margin = new System.Windows.Forms.Padding(4);
            this.NrfTB.Name = "NrfTB";
            this.NrfTB.Size = new System.Drawing.Size(63, 22);
            this.NrfTB.TabIndex = 55;
            // 
            // boundDepthTB
            // 
            this.boundDepthTB.Location = new System.Drawing.Point(197, 312);
            this.boundDepthTB.Margin = new System.Windows.Forms.Padding(4);
            this.boundDepthTB.Name = "boundDepthTB";
            this.boundDepthTB.Size = new System.Drawing.Size(63, 22);
            this.boundDepthTB.TabIndex = 62;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(11, 59);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(96, 17);
            this.label19.TabIndex = 46;
            this.label19.Text = "N features RF";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(7, 316);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(186, 17);
            this.label25.TabIndex = 63;
            this.label25.Text = "Bound depth (in generation)";
            // 
            // NfeaturesrfTB
            // 
            this.NfeaturesrfTB.Location = new System.Drawing.Point(197, 60);
            this.NfeaturesrfTB.Margin = new System.Windows.Forms.Padding(4);
            this.NfeaturesrfTB.Name = "NfeaturesrfTB";
            this.NfeaturesrfTB.Size = new System.Drawing.Size(63, 22);
            this.NfeaturesrfTB.TabIndex = 45;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(11, 33);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(59, 17);
            this.label35.TabIndex = 30;
            this.label35.Text = "RF Num";
            // 
            // bagginPercentTB
            // 
            this.bagginPercentTB.Location = new System.Drawing.Point(195, 89);
            this.bagginPercentTB.Margin = new System.Windows.Forms.Padding(4);
            this.bagginPercentTB.Name = "bagginPercentTB";
            this.bagginPercentTB.Size = new System.Drawing.Size(63, 22);
            this.bagginPercentTB.TabIndex = 29;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(8, 285);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(159, 17);
            this.label23.TabIndex = 50;
            this.label23.Text = "Error Type in estimation";
            // 
            // errTypeEstimationTB
            // 
            this.errTypeEstimationTB.Location = new System.Drawing.Point(196, 281);
            this.errTypeEstimationTB.Margin = new System.Windows.Forms.Padding(4);
            this.errTypeEstimationTB.Name = "errTypeEstimationTB";
            this.errTypeEstimationTB.Size = new System.Drawing.Size(63, 22);
            this.errTypeEstimationTB.TabIndex = 49;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(793, 479);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnScript);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Wavelets decomposition";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnScript;
        private System.Windows.Forms.CheckBox runRFPrunningCB;
        private System.Windows.Forms.CheckBox runRfCB;
        private System.Windows.Forms.CheckBox rumPrallelCB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ResultsTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DBTB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox approxThreshTB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox splitTypeTB;
        private System.Windows.Forms.TextBox boundLevelTB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox minNodeSizeTB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox partitionTypeTB;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox NrfTB;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox NfeaturesrfTB;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox bagginPercentTB;
        private System.Windows.Forms.CheckBox croosValidCB;
        private System.Windows.Forms.TextBox croosValidTB;
        private System.Windows.Forms.CheckBox saveTressCB;
        private System.Windows.Forms.CheckBox estimateFullRFCB;
        private System.Windows.Forms.CheckBox estimateRF4SmoothnessAnalysis;
        private System.Windows.Forms.CheckBox estimateRFwaveletsCB;
        private System.Windows.Forms.CheckBox useClassificationCB;
        private System.Windows.Forms.TextBox boundDepthTB;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox thresholdWaveletsCB;
        private System.Windows.Forms.TextBox thresholdWaveletsTB;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox errTypeEstimationTB;
    }
}

