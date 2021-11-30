using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class ActiveCampaignsResponse
    {
        [DataMember(Order = 1)] public List<string> Campaigns { get; set; }
    }
}