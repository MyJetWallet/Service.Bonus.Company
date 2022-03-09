using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public abstract class AccessCriteriaBase
    {
        public abstract string CriteriaId { get; set; }
        public abstract string CampaignId { get; set; }
        public abstract CriteriaType CriteriaType { get; set; }
        public abstract DateTime LastUpdate { get; set; }

        public AccessCriteriaBase(Dictionary<string, string> parameters)
        {
        }

        public abstract Dictionary<string, string> Parameters { get; set; }

        public abstract Task<bool> Check(ClientContext context);
        public abstract Dictionary<string, string> GetParams();

    }
}