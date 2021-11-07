using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;

namespace Service.BonusCampaign.Domain.Models
{
    public class Campaign
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public bool IsEnabled { get; set; }
        public CampaignStatus Status { get; set; }
        public string BannerId { get; set; }
        public List<AccessCriteriaBase> CriteriaList { get; set; }
        public List<ConditionBase> Conditions { get; set; }
    }
}