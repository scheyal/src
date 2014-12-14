namespace AzSvc
{
    partial class AzSvc
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose ( );
            }
            base.Dispose ( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ( )
        {
            this.workerThread = new System.ComponentModel.BackgroundWorker();
            // 
            // workerThread
            // 
            this.workerThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerThread_DoWork);
            // 
            // SvcMgr
            // 
            this.ServiceName = "Service1";

        }

        #endregion

        private System.ComponentModel.BackgroundWorker workerThread;
    }
}
