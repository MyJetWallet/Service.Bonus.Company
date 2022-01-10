using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Services
{
    public class CampaignRegistryService : ICampaignRegistry
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public CampaignRegistryService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<ActiveCampaignsResponse> GetActiveCampaigns(GetActiveCampaignsRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaigns = await ctx.Campaigns.Include(t => t.CampaignClientContexts).Where(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.CampaignClientContexts.Any(context => context.ClientId == request.ClientId)).ToListAsync();

            var campaignsIds = campaigns.Select(campaign => campaign.Id).ToList();

            return new ActiveCampaignsResponse()
            {
                Campaigns = campaignsIds
            };
        }
    }
}