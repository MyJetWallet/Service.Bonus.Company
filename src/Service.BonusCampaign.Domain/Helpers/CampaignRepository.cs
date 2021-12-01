using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignsRegistry _campaignsRegistry;

        public CampaignRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignsRegistry campaignsRegistry)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignsRegistry = campaignsRegistry;
        }

        public async Task<List<Campaign>> GetCampaignsWithoutThisClient(string clientId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.Where(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.CampaignClientContexts.All(context => context.ClientId != clientId))
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ToListAsync();
        }

        public async Task<List<Campaign>> GetCampaigns()
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.ToListAsync();
        }

        public async Task<List<Campaign>> GetActiveCampaigns(string clientId)
        {
            var ids = await _campaignsRegistry.GetActiveCampaignsForClient(clientId);

            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.Campaigns.Include(c=>c.Conditions).ThenInclude(t=>t.Rewards).Where(campaign=>ids.Contains(campaign.Id)).ToListAsync();
        }
        public async Task UpsertCampaign(Campaign campaign)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(new[] { campaign });
        }
        
        public async Task UpsertCampaign(List<Campaign> campaigns)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(campaigns);
        }
    }
}