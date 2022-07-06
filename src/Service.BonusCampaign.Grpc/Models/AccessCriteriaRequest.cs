using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class AccessCriteriaRequest
    {
        [DataMember(Order = 1)] public CriteriaType Type { get; set; }
        [DataMember(Order = 2)] public Dictionary<string, string> Parameters { get; set; }

    }
}