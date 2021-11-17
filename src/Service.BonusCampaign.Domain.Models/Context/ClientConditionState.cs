using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain.Models.Context
{
    public class ClientConditionState
    {
        public string CampaignId { get; set; }
        public string ClientId { get; set; }
        public string ConditionId { get; set; }
        public ConditionType Type { get; set; }
        public ConditionStatus Status { get; set; }
        public string Params { get; set; }
    }
}