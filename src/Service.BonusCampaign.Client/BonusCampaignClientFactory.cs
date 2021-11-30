using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;

namespace Service.BonusCampaign.Client
{
    [UsedImplicitly]
    public class BonusCampaignClientFactory: MyGrpcClientFactory
    {
        private readonly MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> _contextReader;
        private readonly MyNoSqlReadRepository<CampaignsRegistryNoSqlEntity> _campaignsReader;

        public BonusCampaignClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> contextReader, MyNoSqlReadRepository<CampaignsRegistryNoSqlEntity> campaignsReader) : base(grpcServiceUrl)
        {
            _contextReader = contextReader;
            _campaignsReader = campaignsReader;
        }

        public ICampaignManager GetCampaignManager() => CreateGrpcService<ICampaignManager>();

        public ICampaignRegistry GetCampaignRegistry() => _campaignsReader != null
            ? new CampaignRegistryClient(_campaignsReader, CreateGrpcService<ICampaignRegistry>())
            : CreateGrpcService<ICampaignRegistry>();

        public IClientContextService GetClientContextService() => _contextReader != null
            ? new ClientContextClient(_contextReader, CreateGrpcService<IClientContextService>())
            : CreateGrpcService<IClientContextService>();
    }
}
