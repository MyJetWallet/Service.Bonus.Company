using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Conditions;

namespace Service.BonusCampaign.Domain.Models
{
    public class CampaignClientContext
    {
        public string ClientId { get; set; }
        public string CampaignId { get; set; }
        public DateTime ActivationTime { get; set; }
        public CampaignStatus Status { get; set; }
        public Dictionary<string, ConditionStatus> Conditions { get; set; }
    }
}