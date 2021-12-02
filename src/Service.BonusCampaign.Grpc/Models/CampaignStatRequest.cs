using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class CampaignStatRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string Brand { get; set; }
        [DataMember(Order = 3)] public string Lang { get; set; }
    }
}