using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models;

[DataContract]
public class BlockUserRequest
{
    [DataMember(Order = 1)] public string CampaignId { get; set; }
    [DataMember(Order = 2)] public string ClientId { get; set; }
}