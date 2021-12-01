using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class CampaignStatRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
    }
}