using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.GrpcModels;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class GetAllCampaignsResponse
    {
        [DataMember(Order = 1)] public List<CampaignGrpcModel> Campaigns { get; set; }
    }
}