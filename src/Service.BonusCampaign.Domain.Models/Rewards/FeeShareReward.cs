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
    public class FeeShareReward : RewardBase
    {
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { FeeShareGroup, typeof(string).ToString() },
        };

        public override Dictionary<string, string> Parameters { get; set; }
        public override string RewardId { get; set; }
        public override string ConditionId { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override RewardType Type { get; set; }
        public override Dictionary<string, string> GetParams() => ParamDictionary;

        public override async Task ExecuteReward(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher)
        {
            await publisher.PublishAsync(new ExecuteRewardMessage
            {
                ClientId = context.ClientId,
                RewardType = RewardType.FeeShareAssignment.ToString(),
                FeeShareGroup = Parameters[FeeShareGroup],
                RewardId = RewardId,
            });
            Console.WriteLine($"Executing reward {Type} for user {context.ClientId}");
        }
        
        public FeeShareReward(Dictionary<string, string> parameters, string rewardId, string conditionId)
        {
            Type = RewardType.FeeShareAssignment;
            ConditionId = conditionId;
            RewardId = rewardId ?? Guid.NewGuid().ToString("N");
            Parameters = parameters;
            LastUpdate = DateTime.UtcNow;
        }
    }
}