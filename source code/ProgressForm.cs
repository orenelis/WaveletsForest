using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataSetsSparsity
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();

            //SET BACKGROUNDWORKER1 PROPERTIES
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            //backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            //backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(
            //    backgroundWorker1_ProgressChanged);
            //backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            //    backgroundWorker1_RunWorkerCompleted);
        }
         
        //DELEGATE EVENT DOWORK WITH HANDLER
        public delegate void DoWorkEventHandler(ProgressForm sender, DoWorkEventArgs e);
        public event DoWorkEventHandler DoWork;

        //CALL FOR WORK
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //the background worker started
            //let's call the user's event handler
            if (DoWork != null)
                DoWork(this, e);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //the background worker started
            //let's call the user's event handler
            if (DoWork != null)
                DoWork(this, e);
        }

        public void SetProgress(int percent, string status)
        {
            backgroundWorker1.ReportProgress(percent, status);
        }

        public bool CancellationPending
        {
            get { return backgroundWorker1.CancellationPending; }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= progressBar1.Minimum &&
                e.ProgressPercentage <= progressBar1.Maximum)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
            if (e.UserState != null)
                label1.Text = e.UserState.ToString();
        }

        public RunWorkerCompletedEventArgs Result { get; private set; }        
        
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //ShowDialog return value will inform whether the worker finished properly or not
            if (e.Error != null)
                DialogResult = DialogResult.Abort;
            else if (e.Cancelled)
                DialogResult = DialogResult.Cancel;
            else
                DialogResult = DialogResult.OK;
            //close the form
            Close();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //notify the worker we want to cancel
            backgroundWorker1.CancelAsync();
            //disable the cancel button and change the status text
            btnCancel.Enabled = false;
            label1.Text = "Cancelling...";
        }
    }
}
