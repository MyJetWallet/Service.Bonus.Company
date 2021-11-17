using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public abstract class ConditionBase
    {
        public abstract string ConditionId { get; set; }
        public abstract string CampaignId { get; set; }
        public abstract ConditionType Type { get; set;  }
        public abstract Dictionary<string, string> Parameters { get; set; }
        public static List<EventType> EventTypes { get; set; }
        public abstract List<RewardBase> Rewards { get; set; }
        public abstract ConditionStatus Status { get; set; }
        
        public abstract Dictionary<string, string> GetParams();
        public abstract Task<bool> Check(ContextUpdate context);

    }
}