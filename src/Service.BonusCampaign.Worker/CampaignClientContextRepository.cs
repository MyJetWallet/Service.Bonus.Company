using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Worker
{
    public class CampaignClientContextRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public CampaignClientContextRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }
        
        public async Task<List<CampaignClientContext>> GetContextById(string clientId)
        {
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
        
        public async Task<List<CampaignClientContext>> GetContext()
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await ctx.CampaignClientContexts.ToListAsync();
        }

        public async Task UpsertContext(CampaignClientContext context)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(new[] { context });
        }
        
        public async Task UpsertContext(List<CampaignClientContext> contexts)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await ctx.UpsertAsync(contexts);
        }
    }
}