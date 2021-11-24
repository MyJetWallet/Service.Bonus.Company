﻿using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.BonusCampaign.Worker.Helpers;
using Service.BonusCampaign.Worker.Jobs;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Worker.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var queueName = "Service.Bonus.Campaign-Worker";
            var spotServiceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);
            builder.RegisterMyServiceBusSubscriberSingle<ContextUpdate>(spotServiceBusClient,
                ContextUpdate.TopicName, queueName, TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusPublisher<ExecuteRewardMessage>(spotServiceBusClient,
                ExecuteRewardMessage.TopicName, false);

            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            builder.RegisterConvertIndexPricesClient(myNoSqlClient);
            
            builder.RegisterType<CampaignRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CampaignClientContextRepository>().AsSelf().SingleInstance();
            
            builder.RegisterType<CheckerJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<CampaignCheckerJob>().AsSelf().SingleInstance();

        }
    }
}