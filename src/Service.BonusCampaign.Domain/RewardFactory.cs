using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Domain
{
    public static class RewardFactory
    {
        public static RewardBase CreateReward(RewardType type, Dictionary<string, string> parameters, string rewardId)
        {
            switch (type)
            {
                case RewardType.FeeShareAssignment:
                    return new FeeShareReward(parameters, rewardId);
                case RewardType.ClientPaymentAbsolute:
                    return new ClientPaymentReward(parameters, rewardId);
                case RewardType.ReferrerPaymentAbsolute:
                case RewardType.ReferrerPaymentRelative:
                case RewardType.ClientPaymentRelative:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static Dictionary<string, string> GetParams(RewardType type)
        {
            switch (type)
            {
                case RewardType.FeeShareAssignment:
                    return FeeShareReward.ParamDictionary;
                case RewardType.ClientPaymentAbsolute:
                    return ClientPaymentReward.ParamDictionary;
                case RewardType.ReferrerPaymentAbsolute:
                case RewardType.ReferrerPaymentRelative:
                case RewardType.ClientPaymentRelative:
                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}