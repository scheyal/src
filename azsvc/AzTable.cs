using System;
using AzSvc.Properties;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Net;
using System.Net.NetworkInformation;
using log4net;

namespace AzSvc
{
    /// <summary>
    /// Azure Table
    /// </summary>
    class AzTable
    {
        CloudTable _PingTable;


        // constructor
        public AzTable ( )
        {
            _PingTable = null;
        }

        /// <summary>
        /// Connect to Azure & create table
        /// </summary>
        /// <param name="force">refresh table handle</param>
        public void Connect ( bool force )
        {
            if ( _PingTable != null && !force )
            {
                return;
            }

            // Retrieve the storage account from the connection string.
            StorageCredentials creds = new StorageCredentials ( Settings.Default.StorageAccount, Settings.Default.StorageKey );
            CloudStorageAccount storageAccount = new CloudStorageAccount ( creds, true );  
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient ( );

            // Create the table if it doesn't exist.
            _PingTable = tableClient.GetTableReference ( "PingTable" );
            _PingTable.CreateIfNotExists ( );
        }

        /// <summary>
        /// Write this monitor status to table
        /// </summary>
        public void Write ( )
        {
            if(!Settings.Default.Debug)
            {
                Connect ( false );
            }

            // Create a new customer entity.
            //  Environment.MachineName, DateTime.Now, _CheckCount
            StoreLogLine logLine = new StoreLogLine ( GetPartitionId(), DateTime.Now.ToUniversalTime().ToString("o") );
            logLine.Version = Settings.Default.Version;
            logLine.CloudBrand = Settings.Default.CloudBrand;
            logLine.NickName = Settings.Default.NickName;
            logLine.LocalReportTime = DateTime.Now.ToString ( "G" );
            logLine.Status = "OK";
            logLine.Ordinal = AzSvc.Counter;
            logLine.NetID = GetNetworkInfo ( ); 
            logLine.Environment = GetEnvironmentInfo ( ) ;

            AzSvc.Logger.Debug( "--- --- ---\n" + logLine.ToString() );

            if( !Settings.Default.Debug )
            {
                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert ( logLine );

                // Execute the insert operation.
                _PingTable.Execute ( insertOperation );
            }
        }

        /// <summary>
        /// returns basic network Identifier (host, ip)
        /// </summary>
        /// <returns></returns>
        public string GetNetworkInfo()
        {
            string localIP = "";
            bool gotIP = false; 
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry ( hostName ); 
            foreach (IPAddress ip in host.AddressList)
            {
                if ( ip.AddressFamily.ToString ( ) == "InterNetwork" || ip.AddressFamily.ToString ( ) == "InterNetworkV6" )
                {
                    gotIP = true;
                    localIP += ip.ToString() + " ";
                }
            }
            if ( gotIP )
            {
                localIP.TrimEnd ( new char[] { ' ' } );
            }
            else
            {
                localIP = "Unknown";
            }
            return localIP;
        }

        /// <summary>
        /// Get FQDN of this machine
        /// </summary>
        /// <returns></returns>
        public static string GetFQDN ( )
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties ( ).DomainName;
            string hostName = Dns.GetHostName ( );

            if ( !hostName.EndsWith ( domainName ) )  // if hostname does not already include domain name
            {
                hostName += "." + domainName;   // add the domain name part
            }

            return hostName;                    // return the fully qualified name
        }

        /// <summary>
        /// Get basic environment
        /// </summary>
        /// <returns></returns>
        public string GetEnvironmentInfo ( )
        {
            string env;

            env = String.Format ( "[OS:{0}, Procs:{1}, WorkSet:{2}]",
                                    Environment.OSVersion.ToString ( ),
                                    Environment.ProcessorCount,
                                    Environment.WorkingSet.ToString ( ) ); 
            return env;
        }

        /// <summary>
        /// Get partition id based on svc provider and dns fqdn
        /// </summary>
        /// <returns></returns>
        public string GetPartitionId()
        {
            return String.Format ( "{0}:{1}",
                                    Settings.Default.CloudBrand,
                                    GetFQDN () ); 
        }
    }
}
