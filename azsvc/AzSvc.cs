using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;

using log4net;
using AzSvc.Properties;

namespace AzSvc
{
    /// <summary>
    /// Service object
    /// </summary>
    public partial class AzSvc : ServiceBase
    {

        #region attributes

        enum SvcStatus
        {
            Unknown,
            Started,
            Paused,
            Running,
            Stopped
        };

        SvcStatus _Status;
        SvcStatus Status 
        {
            get { return _Status;  }
            set 
            {
                Logger.DebugFormat ( "Status Change: {0} --> {1}", _Status, value );
                _Status = value;
            }
        }

        public static ILog Logger;
        public static long Counter = 0;
        int _DelayMS;
        AzTable _AzTable;

        #endregion attributes

        #region construction

        public AzSvc ( )
        {
            // service
            InitializeComponent ( );
            AutoLog = true;
            CanPauseAndContinue = true;
            ServiceName = "Azure Ping Service";
            log4net.Config.XmlConfigurator.Configure ( );
            Logger = LogManager.GetLogger ( "AzSvcLog" );
            Logger.Debug ( "SvcMgr Constructor called" );
            _Status = SvcStatus.Unknown;

            // monitoring
            _DelayMS = Settings.Default.PingDelaySeconds * 1000;
            _AzTable = new AzTable ( );

        }

        #endregion construction

        #region service ops

        protected override void OnStart ( string[] args )
        {
            Logger.Debug ( "Call: SvcMgr.OnStart" );
            if ( Status == SvcStatus.Running )
            {
                Logger.Warn ( "Service is attempted to restarted when already running" );
                return;
            }

            Status = SvcStatus.Started;
            try
            {
            workerThread.RunWorkerAsync(); 
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Cannot start AzPingSvc. Exception: {0}", e.Message); 
            }
        }

        protected override void OnStop ( )
        {
            Logger.Debug ( "Call: SvcMgr.OnStop" );
            Status = SvcStatus.Stopped; 
        }

        protected override void OnPause ( )
        {
            Logger.Debug ( "Call: SvcMgr.OnPause" );
            Status = SvcStatus.Paused;
            base.OnPause ( );
        }

        protected override void OnContinue ( )
        {
            Logger.Debug ( "Call: SvcMgr.OnContinue" );
            Status = SvcStatus.Running;
            base.OnContinue ( );
        }
        #endregion service ops

        #region async worker

        /// <summary>
        /// Service worker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workerThread_DoWork ( object sender, DoWorkEventArgs e )
        {

            try
            {
                // when called at startup set to running, later listen to changes
                Status = SvcStatus.Running;

                while ( Status != SvcStatus.Stopped )
                {
                    if ( Status == SvcStatus.Paused )
                    {
                        // paused, sleep and retry
                        Thread.Sleep ( _DelayMS );
                        continue;
                    }

                    // actual work
                    Monitor ( ); 

                    // sleep and repeat
                    Thread.Sleep ( _DelayMS );
                }
            }
            catch ( Exception ex )
            {
                Logger.DebugFormat ( "DoWork raised exception: {0}", ex.Message );
                Status = SvcStatus.Stopped; 
            }
        }

        /// <summary>
        /// Monitor action cycle
        /// </summary>
        void Monitor ( )
        {
            try
            {
                _AzTable.Write ( );

                if ( ++AzSvc.Counter == Int64.MaxValue - 1 )
                {
                    AzSvc.Counter = 0;
                }
            }
            catch ( Exception e )
            {
                Logger.Error ( "UNHANDLED EXCEPTION: " + e.Message );
            }

        }

        #endregion async worker

    }
}
