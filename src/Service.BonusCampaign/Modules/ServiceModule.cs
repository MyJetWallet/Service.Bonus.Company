using Autofac;
using MyJetWallet.DynamicLinkGenerator.Ioc;
using MyJetWallet.Sdk.NoSql;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Services;
using Service.MessageTemplates.Client;

namespace Service.BonusCampaign.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var noSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);
            builder.RegisterMyNoSqlWriter<CampaignClientContextNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignClientContextNoSqlEntity.TableName);
            
            builder.RegisterMyNoSqlWriter<CampaignsRegistryNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignsRegistryNoSqlEntity.TableName);
           
            builder.RegisterMyNoSqlWriter<CampaignNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignNoSqlEntity.TableName);
            
            builder.RegisterMessageTemplatesCachedClient(Program.Settings.MessageTemplatesGrpcServiceUrl, noSqlClient);
            
            builder.RegisterType<ClientContextService>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignClientContextRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignClientContextCacheManager>().AsSelf().SingleInstance();
            
            builder.RegisterDynamicLinkClient(noSqlClient);
            
        }
    }
}