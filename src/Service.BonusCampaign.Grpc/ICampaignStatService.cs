using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Grpc
{
    [ServiceContract]
    public interface ICampaignStatService
    {
        [OperationContract] Task<CampaignStatsResponse> GetCampaignsStats(CampaignStatRequest request);
    }
}