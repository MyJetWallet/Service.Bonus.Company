using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.NoSql;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignClientContextCacheManager
    {
        private readonly IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> _writer;
        private readonly CampaignRepository _campaignRepository;

        public CampaignClientContextCacheManager(IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> writer, CampaignRepository campaignRepository)
        {
            _writer = writer;
            _campaignRepository = campaignRepository;
        }

        public async Task UpdateContext(List<CampaignClientContext> contexts)
        {
            await _writer.BulkInsertOrReplaceAsync(contexts.Select(CampaignClientContextNoSqlEntity.Create));
            await _writer.CleanAndKeepMaxPartitions(10000);
        }

        public async Task<List<CampaignClientContext>> GetActiveContextsByClient(string clientId)
        {
            var entities = await _writer.GetAsync(CampaignClientContextNoSqlEntity.GeneratePartitionKey(clientId));
            var activeCampaigns = await _campaignRepository.GetActiveCampaignsForClient(clientId);
            var activeCampaignsIds = activeCampaigns.Select(t => t.Id).ToList();
            var campaignClientContextNoSqlEntities = entities.ToList();
            
            if (campaignClientContextNoSqlEntities.Any() && activeCampaigns.Any())
            {
                return campaignClientContextNoSqlEntities.Select(t => t.Context).Where(context=>activeCampaignsIds.Contains(context.CampaignId)).ToList();
            }

            return null;
        }
    }
}