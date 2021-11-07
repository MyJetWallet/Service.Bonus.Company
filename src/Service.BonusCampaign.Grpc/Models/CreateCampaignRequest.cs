using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class CreateCampaignRequest
    {
        [DataMember(Order = 1)] public string CampaignName { get; set; }
        [DataMember(Order = 2)] public DateTime FromDateTime { get; set; }
        [DataMember(Order = 3)] public DateTime ToDateTime { get; set; }
        [DataMember(Order = 4)] public string BannerId { get; set; }
        [DataMember(Order = 5)] public List<AccessCriteriaRequest> AccessCriteria { get; set; }
        [DataMember(Order = 6)] public List<ConditionRequest> Conditions { get; set; }
        [DataMember(Order = 7)] public bool IsEnabled { get; set; }
    }
}