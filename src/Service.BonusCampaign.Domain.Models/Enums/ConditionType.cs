namespace Service.BonusCampaign.Domain.Models.Enums
{
    public enum ConditionType
    {
        KYCCondition,
        TradeCondition,
        ReferralCondition,
        DepositCondition,
        WithdrawalCondition, 
        ConditionsCondition,
        FiatDepositCondition,
        None = -1,
    }
}