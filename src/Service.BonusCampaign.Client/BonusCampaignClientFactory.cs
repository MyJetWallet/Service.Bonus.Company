﻿using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;
using Service.MessageTemplates.Client;

namespace Service.BonusCampaign.Client
{
    [UsedImplicitly]
    public class BonusCampaignClientFactory: MyGrpcClientFactory
    {
        private readonly MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> _contextReader;
        private readonly MyNoSqlReadRepository<CampaignNoSqlEntity> _campaignsReader;
        private readonly ITemplateClient _templateClient;

        public BonusCampaignClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> contextReader, MyNoSqlReadRepository<CampaignNoSqlEntity> campaignsReader, ITemplateClient templateClient) : base(grpcServiceUrl)
        {
            _contextReader = contextReader;
            _campaignsReader = campaignsReader;
            _templateClient = templateClient;
        }

        public ICampaignManager GetCampaignManager() => CreateGrpcService<ICampaignManager>();
        
        public IClientContextService GetClientContextService() => _contextReader != null
            ? new ClientContextClient(_contextReader, CreateGrpcService<IClientContextService>())
            : CreateGrpcService<IClientContextService>();
        
        public ICampaignStatService GetCampaignStatService() => (_contextReader != null && _templateClient != null)
            ? new CampaignStatClient(GetClientContextService(), _templateClient, _campaignsReader)
            :CreateGrpcService<ICampaignStatService>();


    }
}
