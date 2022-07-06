using System;
using System.Threading.Tasks;
using MyJetWallet.DynamicLinkGenerator.Services;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.MessageTemplates.Client;

namespace Service.BonusCampaign.Services
{
    public class CampaignStatService : ICampaignStatService
    {
        private readonly IClientContextService _contextClient;
        private readonly IMyNoSqlServerDataReader<CampaignNoSqlEntity> _campaignReader;
        private readonly ITemplateClient _templateClient;
        private readonly IDynamicLinkClient _dynamicLinkClient;
        public CampaignStatService(IClientContextService contextClient, ITemplateClient templateClient, IMyNoSqlServerDataReader<CampaignNoSqlEntity> campaignReader, IDynamicLinkClient dynamicLinkClient)
        {
            _contextClient = contextClient;
            _templateClient = templateClient;
            _campaignReader = campaignReader;
            _dynamicLinkClient = dynamicLinkClient;
        }

        public async Task<CampaignStatsResponse> GetCampaignsStats(CampaignStatRequest request)
        {
            throw new NotImplementedException();
        }
    }
}