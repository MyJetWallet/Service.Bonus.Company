using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;

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
        public override RewardType Type { get; set; }
        public override Dictionary<string, string> GetParams() => ParamDictionary;

        public FeeShareReward(Dictionary<string, string> parameters, string rewardId, string conditionId)
        {
            Type = RewardType.FeeShareAssignment;
            ConditionId = conditionId;
            RewardId = rewardId ?? Guid.NewGuid().ToString("N");
            Parameters = parameters;
        }
    }
}