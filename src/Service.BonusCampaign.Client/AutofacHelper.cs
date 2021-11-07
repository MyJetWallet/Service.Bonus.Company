using Autofac;
using Service.BonusCampaign.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.BonusCampaign.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBonusCampaignClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BonusCampaignClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<ICampaignManager>().SingleInstance();
        }
    }
}
