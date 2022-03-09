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
        Task<GetContextsResponse> GetContextsByClient(GetContextsByClientRequest request);
        
        [OperationContract]
        Task<GetContextsResponse> GetContextsByCampaign(GetContextsByCampaignRequest request);
        
        [OperationContract]
        Task<OperationResponse> BlockUserInCampaign(BlockUserRequest request);
        
        [OperationContract]
        Task<OperationResponse> UnblockUserInCampaign(BlockUserRequest request);
        
        [OperationContract]
        Task<OperationResponse> RemoveReward(RemoveRewardRequest request);
        
        [OperationContract]
        Task<OperationResponse> RemoveCondition(RemoveConditionRequest request);
    }
}