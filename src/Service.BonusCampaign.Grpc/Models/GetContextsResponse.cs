using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models.GrpcModels;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class GetContextsResponse
    {
        [DataMember(Order = 1)] public List<CampaignClientContextGrpcModel> Contexts { get; set; }
    }
}