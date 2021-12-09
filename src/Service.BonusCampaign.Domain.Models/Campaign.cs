using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models
{
    public class Campaign
    {
        public string Id { get; set; }
        public string TitleTemplateId { get; set; }
        public string DescriptionTemplateId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public bool IsEnabled { get; set; }
        public CampaignStatus Status { get; set; }
        public string ImageUrl { get; set; }
        public List<AccessCriteriaBase> CriteriaList { get; set; }
        public List<ConditionBase> Conditions { get; set; }
        public List<CampaignClientContext> CampaignClientContexts { get; set; }
        public string DynamicLink { get; set; }
    }
}