using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface ICampaignManager
    {
        [OperationContract]
        Task<OperationResponse> CreateOrUpdateCampaign(CampaignGrpcModel request);
        
        [OperationContract]
        Task<GetAllCampaignsResponse> GetAllCampaigns();
        
        [OperationContract]
        ParamsResponse GetAllParams();

        [OperationContract]
        Task<GetContextsByClientResponse> GetContextsByClient(GetContextsByClientRequest request);
    }
}