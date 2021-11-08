using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class CampaignGrpcModel
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string Name { get; set; }
        [DataMember(Order = 3)] public DateTime FromDateTime { get; set; }
        [DataMember(Order = 4)] public DateTime ToDateTime { get; set; }
        [DataMember(Order = 5)] public bool IsEnabled { get; set; }
        [DataMember(Order = 6)] public CampaignStatus Status { get; set; }
        [DataMember(Order = 7)] public string BannerId { get; set; }
        [DataMember(Order = 8)] public List<AccessCriteriaGrpcModel> CriteriaList { get; set; }
        [DataMember(Order = 9)] public List<ConditionGrpcModel> Conditions { get; set; }
    }

    [DataContract]
    public class AccessCriteriaGrpcModel
    {
        [DataMember(Order = 1)] public CriteriaType CriteriaType;
        [DataMember(Order = 2)] public Dictionary<string, string> Parameters { get; set; }
    }
    
    [DataContract]
    public class ConditionGrpcModel
    {
        [DataMember(Order = 1)] public string CampaignId { get; set; }
        [DataMember(Order = 2)] public ConditionType Type { get; set; }
        [DataMember(Order = 3)] public Dictionary<string, string> Parameters { get; set; }
        [DataMember(Order = 4)] public List<RewardGrpcModel> Rewards { get; set; }
        [DataMember(Order = 5)] public ConditionStatus Status { get; set; }
    }
    
    [DataContract]
    public class RewardGrpcModel
    {
        [DataMember(Order = 1)] public string RewardId { get; set; }
        [DataMember(Order = 2)] public RewardType Type { get; set; }
        [DataMember(Order = 3)] public Dictionary<string, string> Parameters { get; set; }
    }


    public static class CampaignExtensions
    {
        public static RewardGrpcModel ToGrpcModel(this RewardBase rewardBase)
        {
            return new RewardGrpcModel
            {
                RewardId = rewardBase.RewardId,
                Type = rewardBase.Type,
                Parameters = rewardBase.Parameters
            };
        }
        
        public static ConditionGrpcModel ToGrpcModel(this ConditionBase conditionBase)
        {
            return new ConditionGrpcModel
            {
                CampaignId = conditionBase.CampaignId,
                Type = conditionBase.Type,
                Parameters = conditionBase.Parameters,
                Rewards = conditionBase.Rewards.Select(ToGrpcModel).ToList(),
                Status = conditionBase.Status
            };
        }
        
        public static AccessCriteriaGrpcModel ToGrpcModel(this AccessCriteriaBase criteria)
        {
            return new AccessCriteriaGrpcModel
            {
                CriteriaType = criteria.CriteriaType,
                Parameters = criteria.Parameters
            };
        }
        public static CampaignGrpcModel ToGrpcModel(this Campaign campaign)
        {
            return new CampaignGrpcModel
            {
                Id = campaign.Id,
                Name = campaign.Name,
                FromDateTime = campaign.FromDateTime,
                ToDateTime = campaign.ToDateTime,
                IsEnabled = campaign.IsEnabled,
                Status = campaign.Status,
                BannerId = campaign.BannerId,
                CriteriaList = campaign.CriteriaList?.Select(ToGrpcModel).ToList() ?? new (),
                Conditions = campaign.Conditions?.Select(ToGrpcModel).ToList() ?? new ()
            };
        }
    }
}