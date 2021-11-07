using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain
{
    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionType type, Dictionary<string, string> parameters, List<RewardBase> rewards, string campaignId)
        {
            switch (type)
            {
                case ConditionType.KYCCondition:
                    return new KycCondition(campaignId, parameters, rewards);
                case ConditionType.TradeCondition:
                case ConditionType.ReferralCondition:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static Dictionary<string, string> GetParams(ConditionType type)
        {
            switch (type)
            {
                case ConditionType.KYCCondition:
                    return KycCondition.ParamDictionary;
                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}