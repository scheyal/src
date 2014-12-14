using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzSvc 
{
    /// <summary>
    /// Storage log line
    /// </summary>
    class StoreLogLine : TableEntity
    {
        public string CloudBrand { get; set; }
        public string NetID { get; set; }
        public string Version { get; set; }
        public string NickName { get; set; }
        public string LocalReportTime { get; set; }
        public string Status { get; set; }
        public long Ordinal { get; set; }
        public string Environment { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="row"></param>
        public StoreLogLine ( string partition, string row )
        {
            this.PartitionKey = partition;
            this.RowKey = row;
        }

        public StoreLogLine ( )
        {
        }

        override public string ToString()
        {
            string str = String.Format ( "[{0}, {1}]>> ", PartitionKey, RowKey ); 
            str += String.Format ( "CloudBrand = {0}\nNetID = {1}\nVersion = {2}\nNickName = {3}\nLocalTime = {4}\nStatus = {5}\nOrdinal = {6}\nEnvironment = {7}" ,
                                         CloudBrand, NetID, Version, NickName, LocalReportTime, Status, Ordinal, Environment );
            return str;
        }
    }
}
