using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Rewards
{
    public class ClientPaymentReward : RewardBase
    {
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { AmountParam, typeof(decimal).ToString() },
            { PaidAsset, typeof(string).ToString() },
        };

        public override Dictionary<string, string> Parameters { get; set; }
        public override string RewardId { get; set; }
        public override string ConditionId { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override RewardType Type { get; set; }
        
        public ClientPaymentReward(Dictionary<string, string> parameters, string rewardId, string conditionId)
        {
            Type = RewardType.ClientPaymentAbsolute;
            ConditionId = conditionId;
            RewardId = rewardId ?? Guid.NewGuid().ToString("N");
            Parameters = parameters;
            LastUpdate = DateTime.UtcNow;
        }
        
        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task ExecuteReward(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher)
        {
            await publisher.PublishAsync(new ExecuteRewardMessage
            {
                ClientId = context.ClientId,
                RewardType = RewardType.ClientPaymentAbsolute.ToString(),
                Asset = Parameters[PaidAsset],
                AmountAbs = decimal.Parse(Parameters[AmountParam]),
                RewardId = RewardId,
                ReferralClientId = !string.IsNullOrEmpty(context.Context.ReferrerClientId) ? context.ClientId : string.Empty,
                ReferrerClientId = !string.IsNullOrEmpty(context.Context.ReferrerClientId) ? context.Context.ReferrerClientId : string.Empty,
            });
            Console.WriteLine($"Executing reward {Type} for user {context.ClientId}");
        }
    }
}