using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Rewards
{
    public class ReferralPaymentReward : RewardBase
    {
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { AmountParam, typeof(decimal).ToString() },
            { PaidAsset, typeof(string).ToString() },
        };

        public override Dictionary<string, string> Parameters { get; set; }
        public override string RewardId { get; set; }
        public override string ConditionId { get; set; }
        public override RewardType Type { get; set; }
        
        public ReferralPaymentReward(Dictionary<string, string> parameters, string rewardId, string conditionId)
        {
            Type = RewardType.ReferrerPaymentAbsolute;
            ConditionId = conditionId;
            RewardId = rewardId ?? Guid.NewGuid().ToString("N");
            Parameters = parameters;
        }
        
        public override Dictionary<string, string> GetParams() => ParamDictionary;
        public override async Task ExecuteReward(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher)
        {
            await publisher.PublishAsync(new ExecuteRewardMessage
            {
                ClientId = context.Context.ReferrerClientId,
                RewardType = RewardType.ReferrerPaymentAbsolute.ToString(),
                Asset = Parameters[PaidAsset],
                AmountAbs = decimal.Parse(Parameters[AmountParam]),
                RewardId = RewardId,
            });
            Console.WriteLine($"Executing reward {Type} for user {context.ClientId}");
        }
    }
}