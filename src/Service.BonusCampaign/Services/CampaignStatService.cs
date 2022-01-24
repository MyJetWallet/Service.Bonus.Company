using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.DynamicLinkGenerator.Models;
using MyJetWallet.DynamicLinkGenerator.Services;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Context.ParamsModels;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusCampaign.Domain.Models.Stats;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;
using Service.DynamicLinkGenerator.Domain.Models.Enums;
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