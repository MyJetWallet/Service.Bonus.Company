using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class GetContextsByClientResponse
    {
        [DataMember(Order = 1)] public List<CampaignClientContextGrpcModel> Contexts { get; set; }
    }
}