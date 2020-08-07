using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;
using System.Text.Json;

namespace SurveyBE.Models
{

    public class SurveyMgr
    {
        private IConfiguration AppConfig;
        private readonly string TableName = "SurveyEntries";
        private string ConnStr;
        private TableStorageAsync<SurveyEntryModel> Table;

        public SurveyMgr(IConfiguration configuration)
        {
            AppConfig = configuration;
            ConnStr = AppConfig["StorageConnectionString"];
            Table = new TableStorageAsync<SurveyEntryModel>(TableName, ConnStr);
        }

        public async Task Create(SurveyEntryModel Entry)
        {
            try
            {
                Entry.PartitionKey = Constants.DefaultPartition;
                Entry.RowKey = Guid.NewGuid().ToString();
                Entry.Q6AnswerJson = JsonSerializer.Serialize(Entry.Q6Answer);
                

                await Table.ConnectAsync();
                await Table.CreateEntityAsync(Entry);

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Survey Entry Create Error: {e.Message}");
                throw e;
            }
        }




    }
}
