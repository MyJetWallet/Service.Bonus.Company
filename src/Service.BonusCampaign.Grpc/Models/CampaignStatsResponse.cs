using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.Stats;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class CampaignStatsResponse
    {
        [DataMember(Order = 1)] public List<CampaignStatModel> Campaigns { get; set; }
    }
}