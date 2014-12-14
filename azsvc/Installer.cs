using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Linq;


namespace AzSvc
{
    [RunInstaller ( true )]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;
        public Installer ( )
        {
            InitializeComponent ( );
            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller ( );
            serviceInstaller = new ServiceInstaller ( );

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            serviceInstaller.StartType = ServiceStartMode.Manual;

            // ServiceName must equal those on ServiceBase derived classes.
            serviceInstaller.ServiceName = "Azure Ping Service";

            // Add installers to collection. Order is not important.
            Installers.Add ( serviceInstaller );
            Installers.Add ( processInstaller );
        }
    }
}
