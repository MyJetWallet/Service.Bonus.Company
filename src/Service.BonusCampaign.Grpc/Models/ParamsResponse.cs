using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class ParamsResponse
    {
        [DataMember(Order = 1)] public List<ParamEntity> CriteriaParams { get; set; }
        [DataMember(Order = 2)] public List<ParamEntity> ConditionParams { get; set; }
        [DataMember(Order = 3)] public List<ParamEntity> RewardParams { get; set; }

    }

    [DataContract]
    public class ParamsDict
    {
        [DataMember (Order = 1)] public string ParamName { get; set; }
        [DataMember (Order = 2)] public string ParamType { get; set; }
    }
    
    [DataContract]
    public class ParamEntity
    {
        [DataMember (Order = 1)] public string Type { get; set; }
        [DataMember (Order = 2)] public List<ParamsDict> Params { get; set; }
    }
}