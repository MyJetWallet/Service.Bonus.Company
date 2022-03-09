using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models;

[DataContract]
public class RemoveConditionRequest
{
    [DataMember(Order = 1)] public string ConditionId { get; set; }
}