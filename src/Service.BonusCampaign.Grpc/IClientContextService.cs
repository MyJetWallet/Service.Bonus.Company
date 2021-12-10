using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface IClientContextService
    {
        [OperationContract]
        Task<GetContextsByClientResponse> GetContextsByClient(GetContextsByClientRequest request);
        
        [OperationContract]
        Task<GetContextsByClientResponse> GetActiveContextsByClient(GetContextsByClientRequest request);
    }
}