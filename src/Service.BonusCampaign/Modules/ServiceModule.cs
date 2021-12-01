using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.BonusCampaign.Domain;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.NoSql;

namespace Service.BonusCampaign.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<CampaignClientContextNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignClientContextNoSqlEntity.TableName);
            
            builder.RegisterMyNoSqlWriter<CampaignsRegistryNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignsRegistryNoSqlEntity.TableName);
            
            builder.RegisterType<CampaignRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignClientContextRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignsRegistry>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignClientContextCacheManager>().AsSelf().SingleInstance();
        }
    }
}