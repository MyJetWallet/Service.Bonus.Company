using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.Stats
{
    [DataContract]
    public class CampaignStatModel
    {
        [DataMember(Order = 1)]public string Title { get; set; }
        [DataMember(Order = 2)]public string Description { get; set; }
        [DataMember(Order = 3)]public TimeSpan TimeToComplete { get; set; }
        [DataMember(Order = 4)]public List<ConditionStatModel> Conditions { get; set; }
        [DataMember(Order = 5)]public string ImageUrl { get; set; }
    }

    [DataContract]
    public class ConditionStatModel
    {
        [DataMember(Order = 1)]public ConditionType Type { get; set; }
        [DataMember(Order = 2)]public Dictionary<string, string> Params { get; set; }
        [DataMember(Order = 3)]public RewardStatModel Reward { get; set; }
    }
    
    [DataContract]
    public class RewardStatModel
    {
        [DataMember(Order = 1)]public decimal Amount { get; set; }
        [DataMember(Order = 2)]public string Asset { get; set; }
    }
    
}