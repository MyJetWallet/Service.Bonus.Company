using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignClientContextRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignClientContextCacheManager _clientContextCache;

        public CampaignClientContextRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignClientContextCacheManager clientContextCache)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _clientContextCache = clientContextCache;
        }
        
        public async Task<List<CampaignClientContext>> GetContextById(string clientId)
        {
            var cached = await _clientContextCache.GetActiveContextsByClient(clientId);
            if (cached.Any())
                return cached;
            
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var query =
                from context in ctx.CampaignClientContexts
                where context.ClientId == clientId
                join campaign in ctx.Campaigns
                    on context.CampaignId equals campaign.Id
                where campaign.Status == CampaignStatus.Active
                select context;

           return await query.Include(t=>t.Conditions).ToListAsync();
        }
        
        public async Task UpsertContext(List<CampaignClientContext> contexts)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(contexts);
            await ctx.UpsertAsync(contexts.SelectMany(t => t.Conditions));
            await _clientContextCache.UpdateContext(contexts);
        }
    }
}