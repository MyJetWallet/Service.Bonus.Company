using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface IClientContextService
    {
        [OperationContract]
        Task<GetContextsResponse> GetContextsByClient(GetContextsByClientRequest request);
        
        [OperationContract]
        Task<GetContextsResponse> GetActiveContextsByClient(GetContextsByClientRequest request);
    }
}