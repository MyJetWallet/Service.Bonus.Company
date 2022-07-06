using System;
using System.Collections.Generic;
using MyJetWallet.DynamicLinkGenerator.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Domain
{
    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionType type, Dictionary<string, string> parameters, List<RewardBase> rewards, string campaignId, string conditionId, TimeSpan timeToComplete, ActionEnum action, int weight, string templateId)
        {
            switch (type)
            {
                case ConditionType.KYCCondition:
                    return new KycCondition(campaignId, parameters, rewards, conditionId, timeToComplete, action, weight, templateId);
                case ConditionType.TradeCondition:
                    return new TradeCondition(campaignId, parameters, rewards, conditionId, timeToComplete, action, weight, templateId);
                case ConditionType.DepositCondition:
                    return new DepositCondition(campaignId, parameters, rewards, conditionId, timeToComplete, action, weight, templateId);
                case ConditionType.FiatDepositCondition:
                    return new FiatDepositCondition(campaignId, parameters, rewards, conditionId, timeToComplete, action, weight, templateId);
                case ConditionType.ConditionsCondition:
                    return new ConditionsCondition(campaignId, parameters, rewards, conditionId, timeToComplete, action, weight, templateId);
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
                case ConditionType.FiatDepositCondition:
                    return FiatDepositCondition.ParamDictionary;
                case ConditionType.TradeCondition:
                    return TradeCondition.ParamDictionary;
                case ConditionType.ConditionsCondition:
                    return ConditionsCondition.ParamDictionary;
                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}