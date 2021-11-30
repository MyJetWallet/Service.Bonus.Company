using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface ICampaignRegistry
    {
        [OperationContract]
        Task<ActiveCampaignsResponse> GetActiveCampaigns(GetActiveCampaignsRequest request);
    }
}