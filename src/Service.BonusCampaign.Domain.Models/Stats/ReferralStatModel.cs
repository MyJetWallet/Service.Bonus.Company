using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.Stats
{
    [DataContract]
    public class ReferralStatModel
    {
        public string CampaignName { get; set; }
        public List<ConditionStatModel> Conditions { get; set; }
    }

    public class ConditionStatModel
    {
        public ConditionType Type { get; set; }
        public TimeSpan RemainingType { get; set; }
        public KycStatModel KycStatModel { get; set; }
        public DepositStatModel DepositStatModel { get; set; }
        public TradeStatModel TradeStatModel { get; set; }
        public ConditionsConditionStatModel ConditionsConditionStatModel { get; set; }
        
        public RewardStatModel Reward { get; set; }
    }
    
    public class KycStatModel
    {
        public bool CurrentStatus { get; set; }
        public bool RequiredStatus { get; set; }
    }
    public class DepositStatModel
    {
        public bool CurrentStatus { get; set; }
        public bool RequiredStatus { get; set; }
    }
    public class TradeStatModel
    {
        public decimal CurrentAmount { get; set; }
        public decimal RequiredAmount { get; set; }
    }
    
    public class ConditionsConditionStatModel
    {
        public int CurrentConditionsCount { get; set; }
        public int RequiredConditionsCount { get; set; }
    }

    public class RewardStatModel
    {
        public string Asset { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
    
}