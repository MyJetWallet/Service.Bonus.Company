using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;

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
        public override RewardType Type { get; set; }
        
        public ClientPaymentReward(Dictionary<string, string> parameters, string rewardId)
        {            
            RewardId = rewardId ?? Guid.NewGuid().ToString("N");
            Parameters = parameters;
        }
        
        public override Dictionary<string, string> GetParams() => ParamDictionary;
    }
}