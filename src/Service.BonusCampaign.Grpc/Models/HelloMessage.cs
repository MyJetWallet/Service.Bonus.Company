using System.Runtime.Serialization;
using Service.BonusCampaign.Domain.Models;

namespace Service.BonusCampaign.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}