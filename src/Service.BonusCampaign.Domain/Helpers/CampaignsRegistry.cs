using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.NoSql;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignsRegistry
    {
        private readonly IMyNoSqlServerDataWriter<CampaignsRegistryNoSqlEntity> _writer;

        public CampaignsRegistry(IMyNoSqlServerDataWriter<CampaignsRegistryNoSqlEntity> writer)
        {
            _writer = writer;
        }

        public async Task AddCampaigns(string clientId, List<string> campaigns)
        {
            var record = await _writer.GetAsync(CampaignsRegistryNoSqlEntity.GeneratePartitionKey(), CampaignsRegistryNoSqlEntity.GenerateRowKey(clientId)) ?? CampaignsRegistryNoSqlEntity.Create(clientId, campaigns);
            record.ActiveCampaigns.AddRange(campaigns);

            record.ActiveCampaigns = record.ActiveCampaigns.Distinct().ToList();
            await _writer.InsertOrReplaceAsync(record);
        }
        
        public async Task AddCampaign(string clientId,string campaign)
        {
            var record = await _writer.GetAsync(CampaignsRegistryNoSqlEntity.GeneratePartitionKey(), CampaignsRegistryNoSqlEntity.GenerateRowKey(clientId)) ?? CampaignsRegistryNoSqlEntity.Create(clientId, new List<string>(){campaign});
            record.ActiveCampaigns.Add(campaign);
            
            record.ActiveCampaigns = record.ActiveCampaigns.Distinct().ToList();
            await _writer.InsertOrReplaceAsync(record);
        }

        public async Task DeleteCampaign(string clientId, string campaign)
        {
            var record = await _writer.GetAsync(CampaignsRegistryNoSqlEntity.GeneratePartitionKey(), CampaignsRegistryNoSqlEntity.GenerateRowKey(clientId));
            if (record == null)
                return;

            record.ActiveCampaigns.Remove(campaign);
            await _writer.InsertOrReplaceAsync(record);
        }

        public async Task RemoveCampaignForAll(List<string> campaigns)
        {
            var records = (await _writer.GetAsync(CampaignsRegistryNoSqlEntity.GeneratePartitionKey())).ToList();
            foreach (var record in records)
            {
                record.ActiveCampaigns.RemoveAll(campaigns.Contains);
            }
            await _writer.BulkInsertOrReplaceAsync(records);
        }

        public async Task<List<string>> GetActiveCampaignsForClient(string clientId)
        {
            var record = await _writer.GetAsync(CampaignsRegistryNoSqlEntity.GeneratePartitionKey(), CampaignsRegistryNoSqlEntity.GenerateRowKey(clientId));
            return record?.ActiveCampaigns ?? new List<string>();
        }
    }
}