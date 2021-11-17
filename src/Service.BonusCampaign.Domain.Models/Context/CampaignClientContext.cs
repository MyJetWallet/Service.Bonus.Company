using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.Context
{
    public class CampaignClientContext
    {
        public string ClientId { get; set; }
        public string CampaignId { get; set; }
        public DateTime ActivationTime { get; set; }
        public CampaignStatus Status { get; set; }
        public List<ClientConditionState> Conditions { get; set; }
        
    }


}