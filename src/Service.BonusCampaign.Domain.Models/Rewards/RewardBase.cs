using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Rewards
{
    public abstract class RewardBase
    {
        public const string AmountParam = "amount";
        public const string PaidAsset = "asset";
        public const string ClientId = "clientId";
        public const string ReferrerId = "referrerId";
        public const string FeeShareGroup = "feeShareGroup";

        public abstract Dictionary<string, string> Parameters { get; set; }
        public abstract string RewardId { get; set; }
        public abstract string ConditionId { get; set; }
        public abstract DateTime LastUpdate { get; set; }
        public abstract RewardType Type { get; set; }
        public abstract Dictionary<string, string> GetParams();
        public abstract Task ExecuteReward(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher);
    }
}