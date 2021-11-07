using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.BonusCampaign.Grpc;

namespace Service.BonusCampaign.Client
{
    [UsedImplicitly]
    public class BonusCampaignClientFactory: MyGrpcClientFactory
    {
        public BonusCampaignClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public ICampaignManager GetHelloService() => CreateGrpcService<ICampaignManager>();
    }
}
