using System.ServiceProcess;


namespace AzSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main ( )
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new AzSvc() 
			};
            // start the service
            ServiceBase.Run ( ServicesToRun );
        }
    }
}
