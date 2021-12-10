using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class GetContextsByClientRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public int Skip { get; set; }
        [DataMember(Order = 3)] public int Take { get; set; }
    }
}