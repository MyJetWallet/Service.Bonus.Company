using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.DynamicLinkGenerator.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.Stats
{
    [DataContract]
    public class CampaignStatModel
    {
        [DataMember(Order = 1)]public string Title { get; set; }
        [DataMember(Order = 2)]public string Description { get; set; }
        [DataMember(Order = 3)]public DateTime ExpirationTime { get; set; }
        [DataMember(Order = 4)]public List<ConditionStatModel> Conditions { get; set; }
        [DataMember(Order = 5)]public string ImageUrl { get; set; }
        [DataMember(Order = 6)]public string CampaignId { get; set; }
        [DataMember(Order = 7)]public string DeepLink { get; set; }
        [DataMember(Order = 8)]public int Weight { get; set; }
        [DataMember(Order = 9)]public string DeepLinkWeb { get; set; }
        [DataMember(Order = 10)]public bool ShowReferrerStats { get; set; }
        [DataMember(Order = 11)]public string CampaignType { get; set; }
    }

    [DataContract]
    public class ConditionStatModel
    {
        [DataMember(Order = 1)]public ConditionType Type { get; set; }
        [DataMember(Order = 2)]public Dictionary<string, string> Params { get; set; }
        [DataMember(Order = 3)]public RewardStatModel Reward { get; set; }
        [DataMember(Order = 4)]public string DeepLink { get; set; }
        [DataMember(Order = 5)]public string DeepLinkWeb { get; set; }
        [DataMember(Order = 6)]public int Weight { get; set; }
    }
    
    [DataContract]
    public class RewardStatModel
    {
        [DataMember(Order = 1)]public decimal Amount { get; set; }
        [DataMember(Order = 2)]public string Asset { get; set; }
    }
    
}