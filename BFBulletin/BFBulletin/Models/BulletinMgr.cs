using Azure.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFBulletin.Models
{
    public class BulletinMgr
    {
        private IConfiguration AppConfig;
        private readonly string TableName = "Bulletins";
        private string ConnStr;
        public List<Models.Bulletin> AllBulletins { get; set; }
        public Bulletin CurrentBulletin { get; set; }


        public BulletinMgr(IConfiguration configuration)
        {
            AppConfig = configuration;
            ConnStr = AppConfig["StorageConnectionString"];
        }


        public async Task<Models.Bulletin> Retrieve(string partition, string key)
        {
            System.Diagnostics.Trace.TraceInformation(string.Format("Call Bulletin::Retrieve({0}, {1})", partition, key));

            // add model to 
            TableStorageAsync<Models.Bulletin> table;
            table = new TableStorageAsync<Models.Bulletin>(TableName, ConnStr);
            await table.ConnectAsync();
            CurrentBulletin = await table.GetEntityByPartitionKeyAndRowKeyAsync(partition, key);
            return CurrentBulletin;
        }

        public async Task RetrieveAll()
        {

            TableStorageAsync<Models.Bulletin> table = new TableStorageAsync<Models.Bulletin>(TableName, ConnStr);
            await table.ConnectAsync();
            var bulletins = await table.GetAllEntitiesAsync();
            AllBulletins = bulletins?.ToList<Models.Bulletin>();
            AllBulletins.Sort();
            if (AllBulletins.Count == 0)
            {
                CurrentBulletin = new Bulletin();
            }
            else
            {
                CurrentBulletin = AllBulletins[0];
            }
        }

        public async Task<Bulletin> GetBulletin()
        {
            await RetrieveAll();
            return CurrentBulletin;
        }


    }
}
