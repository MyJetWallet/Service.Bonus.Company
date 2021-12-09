using Autofac;
using MyJetWallet.DynamicLinkGenerator.Services;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;
using Service.MessageTemplates.Client;

// ReSharper disable UnusedMember.Global

namespace Service.BonusCampaign.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBonusCampaignClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BonusCampaignClientFactory(grpcServiceUrl, null, null, null, null);

            builder.RegisterInstance(factory.GetCampaignManager()).As<ICampaignManager>().SingleInstance();
            builder.RegisterInstance(factory.GetClientContextService()).As<IClientContextService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignStatService()).As<ICampaignStatService>().SingleInstance();
        }
        
        public static void RegisterBonusCampaignNoSqlClient(this ContainerBuilder builder, string grpcServiceUrl, IMyNoSqlSubscriber myNoSqlSubscriber, ITemplateClient templateClient, IDynamicLinkClient dynamicLinkClient)
        {
            var contextSubs = new MyNoSqlReadRepository<CampaignClientContextNoSqlEntity>(myNoSqlSubscriber, CampaignClientContextNoSqlEntity.TableName);
            var campaignSubs = new MyNoSqlReadRepository<CampaignNoSqlEntity>(myNoSqlSubscriber, CampaignNoSqlEntity.TableName);

            var factory = new BonusCampaignClientFactory(grpcServiceUrl, contextSubs, campaignSubs, templateClient, dynamicLinkClient);

            builder
                .RegisterInstance(contextSubs)
                .As<IMyNoSqlServerDataReader<CampaignClientContextNoSqlEntity>>()
                .SingleInstance();
            
            builder
                .RegisterInstance(campaignSubs)
                .As<IMyNoSqlServerDataReader<CampaignNoSqlEntity>>()
                .SingleInstance();
            
            builder.RegisterInstance(factory.GetCampaignManager()).As<ICampaignManager>().SingleInstance();
            builder.RegisterInstance(factory.GetClientContextService()).As<IClientContextService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignStatService()).As<ICampaignStatService>().SingleInstance();
        }
        
    }
}
