using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Client
{
    public class CampaignRegistryClient : ICampaignRegistry
    {
        private readonly ICampaignRegistry _campaignRegistry;
        private readonly MyNoSqlReadRepository<CampaignsRegistryNoSqlEntity> _reader;

        public CampaignRegistryClient(MyNoSqlReadRepository<CampaignsRegistryNoSqlEntity> reader, ICampaignRegistry campaignRegistry)
        {
            _reader = reader;
            _campaignRegistry = campaignRegistry;
        }

        public async Task<ActiveCampaignsResponse> GetActiveCampaigns(GetActiveCampaignsRequest request)
        {
            var entity = _reader.Get(CampaignsRegistryNoSqlEntity.GeneratePartitionKey(),
                CampaignsRegistryNoSqlEntity.GenerateRowKey(request.ClientId));

            if (entity != null)
            {
                return new ActiveCampaignsResponse
                {
                    Campaigns = entity.ActiveCampaigns
                };
            }

            return await _campaignRegistry.GetActiveCampaigns(request);
        }
    }
}