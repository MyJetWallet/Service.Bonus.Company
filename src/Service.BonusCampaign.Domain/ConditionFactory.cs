using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain
{
    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionType type, Dictionary<string, string> parameters, List<RewardBase> rewards, string campaignId, string conditionId)
        {
            switch (type)
            {
                case ConditionType.KYCCondition:
                    return new KycCondition(campaignId, parameters, rewards, conditionId);
                case ConditionType.TradeCondition:
                    return new TradeCondition(campaignId, parameters, rewards, conditionId);
                case ConditionType.DepositCondition:
                    return new DepositCondition(campaignId, parameters, rewards, conditionId);
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
                case ConditionType.DepositCondition:
                    return DepositCondition.ParamDictionary;
                case ConditionType.TradeCondition:
                    return TradeCondition.ParamDictionary;
                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}