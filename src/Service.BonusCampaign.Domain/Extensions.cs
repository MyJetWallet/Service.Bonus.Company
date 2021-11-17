using System;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain
{
    public static class Extensions
    {
        public static CriteriaType ToCriteriaType(this EventType type)
        {
            switch (type)
            {
                case EventType.ClientRegistered:
                    return CriteriaType.RegistrationType;
                case EventType.KYCPassed:
                    return CriteriaType.KycType;
                case EventType.ReferrerAdded:
                case EventType.DepositMade:
                case EventType.TradeMade:
                case EventType.WithdrawalMade:
                default:
                   return CriteriaType.None;
            }
        }
        
        public static ConditionType ToConditionType(this EventType type)
        {
            switch (type)
            {
                case EventType.KYCPassed:
                    return ConditionType.KYCCondition;
                case EventType.ReferrerAdded:
                    return ConditionType.ReferralCondition;
                case EventType.TradeMade:
                    return ConditionType.TradeCondition;
                case EventType.DepositMade:
                    return ConditionType.DepositCondition;
                case EventType.WithdrawalMade:
                    return ConditionType.WithdrawalCondition;
                case EventType.ClientRegistered:
                default:
                    return ConditionType.None;
            }
        }
    }
}