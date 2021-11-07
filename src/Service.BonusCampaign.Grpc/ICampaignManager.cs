using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface ICampaignManager
    {
        [OperationContract]
        Task<OperationResponse> CreateCampaign(CreateCampaignRequest request);
        
        [OperationContract]
        Task<GetAllCampaignsResponse> GetAllCampaigns();
        
        [OperationContract]
        ParamsResponse GetAllParams();
    }
}