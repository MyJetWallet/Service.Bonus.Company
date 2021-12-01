using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.BonusCampaign.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBonusCampaignClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BonusCampaignClientFactory(grpcServiceUrl, null, null);

            builder.RegisterInstance(factory.GetCampaignManager()).As<ICampaignManager>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignRegistry()).As<ICampaignRegistry>().SingleInstance();
            builder.RegisterInstance(factory.GetClientContextService()).As<IClientContextService>().SingleInstance();
            
            builder.RegisterInstance(factory.GetCampaignStatService()).As<ICampaignStatService>().SingleInstance();

        }
        
        public static void RegisterBonusCampaignNoSqlClient(this ContainerBuilder builder, string grpcServiceUrl, IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var contextSubs = new MyNoSqlReadRepository<CampaignClientContextNoSqlEntity>(myNoSqlSubscriber, CampaignClientContextNoSqlEntity.TableName);
            var campaignSubs = new MyNoSqlReadRepository<CampaignsRegistryNoSqlEntity>(myNoSqlSubscriber, CampaignsRegistryNoSqlEntity.TableName);

            var factory = new BonusCampaignClientFactory(grpcServiceUrl, contextSubs, campaignSubs);

            builder
                .RegisterInstance(contextSubs)
                .As<IMyNoSqlServerDataReader<CampaignClientContextNoSqlEntity>>()
                .SingleInstance();
            
            builder
                .RegisterInstance(campaignSubs)
                .As<IMyNoSqlServerDataReader<CampaignsRegistryNoSqlEntity>>()
                .SingleInstance();
            
            builder.RegisterInstance(factory.GetCampaignManager()).As<ICampaignManager>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignRegistry()).As<ICampaignRegistry>().SingleInstance();
            builder.RegisterInstance(factory.GetClientContextService()).As<IClientContextService>().SingleInstance();
            
            builder.RegisterInstance(factory.GetCampaignStatService()).As<ICampaignStatService>().SingleInstance();
        }
        
    }
}
