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
        private readonly CampaignClientContextCacheManager _contextCacheManager;
        public CampaignRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<CampaignNoSqlEntity> campaignWriter, CampaignClientContextCacheManager contextCacheManager)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignWriter = campaignWriter;
            _contextCacheManager = contextCacheManager;
        }

        public async Task<List<Campaign>> GetCampaignsWithoutThisClient(string clientId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.Where(campaign =>
                    campaign.Status == CampaignStatus.Active &&
                    campaign.CampaignClientContexts.All(context => context.ClientId != clientId))
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ThenInclude(t=>t.Rewards)
                .Include(t=>t.CampaignClientContexts)
                .ThenInclude(t=>t.Conditions)
                .ToListAsync();
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

        public async Task<List<Campaign>> GetActiveCampaignsForClient(string clientId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.Include(c=>c.Conditions).ThenInclude(t=>t.Rewards).Where(campaign=>campaign.Status== CampaignStatus.Active && campaign.CampaignClientContexts.Any(t=>t.ClientId == clientId)).ToListAsync();
        }

        public async Task SetActiveCampaigns(List<Campaign> campaigns)
        {
            await _campaignWriter.BulkInsertOrReplaceAsync(campaigns.Select(CampaignNoSqlEntity.Create));
            await _contextCacheManager.UpdateContext(campaigns);
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
            await _contextCacheManager.UpdateContext(campaigns);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(campaigns);
        }
        
        public async Task RefreshCampaign(string campaignId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var campaign = await ctx.Campaigns
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ThenInclude(t=>t.Rewards)
                .Include(t=>t.CampaignClientContexts)
                .ThenInclude(t=>t.Conditions)
                .FirstOrDefaultAsync(t=>t.Id == campaignId);

            await _contextCacheManager.UpdateContext(new List<Campaign>(){campaign});
            await _campaignWriter.InsertOrReplaceAsync(CampaignNoSqlEntity.Create(campaign));
        }
    }
}