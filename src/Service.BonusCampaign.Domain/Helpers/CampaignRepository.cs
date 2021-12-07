using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignRepository
    {
        private readonly IMyNoSqlServerDataWriter<CampaignNoSqlEntity> _campaignWriter;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignsRegistry _campaignsRegistry;

        public CampaignRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignsRegistry campaignsRegistry, IMyNoSqlServerDataWriter<CampaignNoSqlEntity> campaignWriter)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignsRegistry = campaignsRegistry;
            _campaignWriter = campaignWriter;
        }

        public async Task<List<Campaign>> GetCampaignsWithoutThisClient(string clientId)
        {
            var entities = await _campaignWriter.GetAsync();
            return entities
                .Select(entity=>entity.Campaign)
                .Where(campaign=>campaign.CampaignClientContexts.All(context => context.ClientId != clientId))
                .ToList();
        }

        public async Task<List<Campaign>> GetCampaigns()
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ThenInclude(t=>t.Rewards)
                .Include(t=>t.CampaignClientContexts)
                .ThenInclude(t=>t.Conditions)
                .ToListAsync();
        }

        public async Task<List<Campaign>> GetActiveCampaigns(string clientId)
        {
            var ids = await _campaignsRegistry.GetActiveCampaignsForClient(clientId);

            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.Include(c=>c.Conditions).ThenInclude(t=>t.Rewards).Where(campaign=>ids.Contains(campaign.Id)).ToListAsync();
        }

        public async Task SetActiveCampaigns(List<Campaign> campaigns)
        {
            await _campaignWriter.BulkInsertOrReplaceAsync(campaigns.Select(CampaignNoSqlEntity.Create));
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(campaigns);
        }
        
        public async Task SetFinishedCampaigns(List<Campaign> campaigns)
        {
            foreach (var campaign in campaigns)
            {
                await _campaignWriter.DeleteAsync(CampaignNoSqlEntity.GeneratePartitionKey(),
                    CampaignNoSqlEntity.GenerateRowKey(campaign.Id));
            }
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(campaigns);
        }
    }
}