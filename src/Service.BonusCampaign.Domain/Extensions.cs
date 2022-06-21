using System;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain
{
    public static class Extensions
    {
        
        public static ConditionType ToConditionType(this EventType type)
        {
            switch (type)
            {
                case EventType.KYCPassed:
                    return ConditionType.KYCCondition;
                case EventType.TradeMade:
                    return ConditionType.TradeCondition;
                case EventType.DepositMade:
                    return ConditionType.DepositCondition;
                case EventType.FiatDepositMade:
                    return ConditionType.FiatDepositCondition;
                case EventType.WithdrawalMade:
                    return ConditionType.WithdrawalCondition;
                case EventType.ClientRegistered:
                    return ConditionType.ReferralCondition;
                case EventType.ClientLoggedIn:
                case EventType.ReferrerAdded:
                default:
                    return ConditionType.None;
            }
        }
    }
}