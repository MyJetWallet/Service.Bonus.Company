using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.DynamicLinkGenerator.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.GrpcModels
{
    [DataContract]
    public class CampaignGrpcModel
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string TitleTemplateId { get; set; }
        [DataMember(Order = 3)] public DateTime FromDateTime { get; set; }
        [DataMember(Order = 4)] public DateTime ToDateTime { get; set; }
        [DataMember(Order = 5)] public bool IsEnabled { get; set; }
        [DataMember(Order = 6)] public CampaignStatus Status { get; set; }
        [DataMember(Order = 7)] public string ImageUrl { get; set; }
        [DataMember(Order = 8)] public List<AccessCriteriaGrpcModel> CriteriaList { get; set; }
        [DataMember(Order = 9)] public List<ConditionGrpcModel> Conditions { get; set; }
        // [DataMember(Order = 10)] public List<CampaignClientContextGrpcModel> Contexts { get; set; }
        [DataMember(Order = 11)] public string DescriptionTemplateId { get; set; }
        [DataMember(Order = 12)] public string SerializedRequest { get; set; }
        [DataMember(Order = 13)] public ActionEnum Action { get; set; }
        [DataMember(Order = 14)] public string Name { get; set; }
        [DataMember(Order = 15)] public int Weight { get; set; }
        [DataMember(Order = 16)] public bool ShowReferrerStats { get; set; }
    }

    [DataContract]
    public class AccessCriteriaGrpcModel
    {
        [DataMember(Order = 1)] public CriteriaType CriteriaType;
        [DataMember(Order = 2)] public Dictionary<string, string> Parameters { get; set; }
        [DataMember(Order = 3)] public string CriteriaId { get; set; }

    }
    
    [DataContract]
    public class ConditionGrpcModel
    {
        [DataMember(Order = 1)] public string CampaignId { get; set; }
        [DataMember(Order = 2)] public ConditionType Type { get; set; }
        [DataMember(Order = 3)] public Dictionary<string, string> Parameters { get; set; }
        [DataMember(Order = 4)] public List<RewardGrpcModel> Rewards { get; set; }
        [DataMember(Order = 5)] public ConditionStatus Status { get; set; }
        [DataMember(Order = 6)] public string ConditionId { get; set; }
        [DataMember(Order = 7)] public TimeSpan TimeToComplete { get; set; }
        [DataMember(Order = 8)] public ActionEnum Action { get; set; }
    }
    
    [DataContract]
    public class RewardGrpcModel
    {
        [DataMember(Order = 1)] public string RewardId { get; set; }
        [DataMember(Order = 2)] public RewardType Type { get; set; }
        [DataMember(Order = 3)] public Dictionary<string, string> Parameters { get; set; }
        [DataMember(Order = 4)] public string ConditionId { get; set; }

    }
    
    [DataContract]
    public class CampaignClientContextGrpcModel
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string CampaignId { get; set; }
        [DataMember(Order = 3)] public DateTime ActivationTime { get; set; }
        [DataMember(Order = 4)] public List<ConditionStateGrpcModel> Conditions { get; set; }
    }

    [DataContract]
    public class ConditionStateGrpcModel
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string CampaignId { get; set; }
        [DataMember(Order = 3)] public string ConditionId { get; set; }
        [DataMember(Order = 4)] public ConditionType Type { get; set; }
        [DataMember(Order = 5)] public ConditionStatus Status { get; set; }
        [DataMember(Order = 6)] public DateTime ExpirationTime { get; set; }
        [DataMember(Order = 7)] public string Params { get; set; }

    }

    public static class CampaignExtensions
    {
        public static ConditionStateGrpcModel ToGrpcModel(this ClientConditionState conditionState)
        {
            return new ConditionStateGrpcModel
            {
                ClientId = conditionState.ClientId,
                CampaignId = conditionState.CampaignId,
                ConditionId = conditionState.ConditionId,
                Type = conditionState.Type,
                Status = conditionState.Status,
                ExpirationTime = conditionState.ExpirationTime,
                Params = conditionState.Params
            };
        }

        public static CampaignClientContextGrpcModel ToGrpcModel(this CampaignClientContext context)
        { 
            return new CampaignClientContextGrpcModel
            {
                ClientId = context.ClientId,
                CampaignId = context.CampaignId,
                ActivationTime = context.ActivationTime,
                Conditions = context.Conditions?.Select(t=>t.ToGrpcModel()).ToList() ?? new ()
            };
        }

        public static RewardGrpcModel ToGrpcModel(this RewardBase rewardBase)
        {
            return new RewardGrpcModel
            {
                RewardId = rewardBase.RewardId,
                Type = rewardBase.Type,
                Parameters = rewardBase.Parameters,
                ConditionId = rewardBase.ConditionId
            };
        }
        
        public static ConditionGrpcModel ToGrpcModel(this ConditionBase conditionBase)
        {
            return new ConditionGrpcModel
            {
                ConditionId = conditionBase.ConditionId,
                CampaignId = conditionBase.CampaignId,
                Type = conditionBase.Type,
                Parameters = conditionBase.Parameters,
                Rewards = conditionBase.Rewards.Select(ToGrpcModel).ToList(),
                Status = conditionBase.Status,
                TimeToComplete = conditionBase.TimeToComplete,
                Action = conditionBase.Action
            };
        }
        
        public static AccessCriteriaGrpcModel ToGrpcModel(this AccessCriteriaBase criteria)
        {
            return new AccessCriteriaGrpcModel
            {
                CriteriaId = criteria.CriteriaId,
                CriteriaType = criteria.CriteriaType,
                Parameters = criteria.Parameters
            };
        }
        public static CampaignGrpcModel ToGrpcModel(this Campaign campaign)
        {
            return new CampaignGrpcModel
            {
                Id = campaign.Id,
                TitleTemplateId = campaign.TitleTemplateId,
                DescriptionTemplateId = campaign.DescriptionTemplateId,
                FromDateTime = campaign.FromDateTime,
                ToDateTime = campaign.ToDateTime,
                IsEnabled = campaign.IsEnabled,
                Status = campaign.Status,
                ImageUrl = campaign.ImageUrl,
                CriteriaList = campaign.CriteriaList?.Select(ToGrpcModel).ToList() ?? new (),
                Conditions = campaign.Conditions?.Select(ToGrpcModel).ToList() ?? new (),
                //Contexts = campaign.CampaignClientContexts?.Select(ToGrpcModel).ToList() ?? new (),
                SerializedRequest = campaign.SerializedRequest,
                Action = campaign.Action,
                Name = campaign.Name,
                Weight = campaign.Weight,
                ShowReferrerStats = campaign.ShowReferrerStats
            };
        }
        
    }
}