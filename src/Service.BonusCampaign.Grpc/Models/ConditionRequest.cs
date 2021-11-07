using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class ConditionRequest
    {
        [DataMember(Order = 1)] public ConditionType Type { get; set; }
        [DataMember(Order = 2)] public Dictionary<string, string> Parameters { get; set; }
        [DataMember(Order = 3)] public List<RewardRequest> Rewards { get; set; }
    }
}