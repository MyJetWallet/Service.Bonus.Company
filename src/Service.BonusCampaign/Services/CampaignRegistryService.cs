using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Services
{
    public class CampaignRegistryService : ICampaignRegistry
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignsRegistry _campaignsRegistry;

        public CampaignRegistryService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignsRegistry campaignsRegistry)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignsRegistry = campaignsRegistry;
        }

        public async Task<ActiveCampaignsResponse> GetActiveCampaigns(GetActiveCampaignsRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaigns = await ctx.Campaigns.Include(t => t.CampaignClientContexts).Where(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.CampaignClientContexts.Any(context => context.ClientId == request.ClientId)).ToListAsync();

            var campaignsIds = campaigns.Select(campaign => campaign.Id).ToList();

            await _campaignsRegistry.AddCampaigns(request.ClientId, campaignsIds);

            return new ActiveCampaignsResponse()
            {
                Campaigns = campaignsIds
            };
        }
    }
}