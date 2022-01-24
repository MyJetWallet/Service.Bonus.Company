using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.NoSql;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignClientContextCacheManager
    {
        private readonly IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> _writer;

        public CampaignClientContextCacheManager(IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> writer)
        {
            _writer = writer;
        }

        public async Task UpdateContext(List<CampaignClientContext> contexts)
        {
            await _writer.BulkInsertOrReplaceAsync(contexts.Select(CampaignClientContextNoSqlEntity.Create));
            await _writer.CleanAndKeepMaxPartitions(10000);
        }
        
        public async Task UpdateContext(List<Campaign> campaigns)
        {
            await _writer.BulkInsertOrReplaceAsync(campaigns.SelectMany(t => t.CampaignClientContexts).Select(CampaignClientContextNoSqlEntity.Create));
            await _writer.CleanAndKeepMaxPartitions(10000);
        }

        public async Task<List<CampaignClientContext>> GetActiveContextsByClient(List<string> activeCampaigns, string clientId)
        {
            var entities = await _writer.GetAsync(CampaignClientContextNoSqlEntity.GeneratePartitionKey(clientId));
            var campaignClientContextNoSqlEntities = entities.ToList();
            if (campaignClientContextNoSqlEntities.Any() && activeCampaigns.Any())
            {
                return campaignClientContextNoSqlEntities.Select(t => t.Context).Where(context=>activeCampaigns.Contains(context.CampaignId)).ToList();
            }

            return null;
        }
    }
}