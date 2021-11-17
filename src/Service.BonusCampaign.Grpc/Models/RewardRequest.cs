using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class RewardRequest
    {
        [DataMember(Order = 1)] public RewardType Type { get; set; }
        [DataMember(Order = 2)] public Dictionary<string, string> Parameters { get; set; }
    }
}