using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public abstract class ConditionBase
    {
        public abstract string ConditionId { get; set; }
        public abstract string CampaignId { get; set; }
        public abstract ConditionType Type { get; set;  }
        public abstract Dictionary<string, string> Parameters { get; set; }
        public static List<EventType> EventTypes { get; set; }
        public abstract List<RewardBase> Rewards { get; set; }
        public abstract ConditionStatus Status { get; set; }
        public abstract TimeSpan TimeToComplete { get; set; }

        public abstract Dictionary<string, string> GetParams();
        public abstract Task<bool> Check(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher, string paramsJson, CampaignClientContext campaignContext);
        public abstract Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient);

    }
}