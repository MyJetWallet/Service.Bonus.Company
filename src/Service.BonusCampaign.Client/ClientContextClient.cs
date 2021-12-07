using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;

namespace Service.BonusCampaign.Client
{
    public class ClientContextClient : IClientContextService
    {

        private readonly IClientContextService _contextService;
        private readonly MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> _reader;

        public ClientContextClient(MyNoSqlReadRepository<CampaignClientContextNoSqlEntity> reader, IClientContextService contextService)
        {
            _reader = reader;
            _contextService = contextService;
        }

        public async Task<GetContextsByClientResponse> GetContextsByClient(GetContextsByClientRequest request)
        {
            var entity = _reader.Get(CampaignClientContextNoSqlEntity.GeneratePartitionKey(request.ClientId));
            if (entity != null)
            {
                return new GetContextsByClientResponse
                {
                    Contexts = entity.Select(t => t.Context.ToGrpcModel()).ToList()
                };
            }

            return await _contextService.GetContextsByClient(request);
        }
    }
}