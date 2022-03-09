using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models;

[DataContract]
public class RemoveRewardRequest
{
    [DataMember(Order = 1)] public string RewardId { get; set; }
}