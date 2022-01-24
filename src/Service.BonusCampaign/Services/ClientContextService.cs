using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Services
{
    public class ClientContextService : IClientContextService
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public ClientContextService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetContextsByClientResponse> GetContextsByClient(GetContextsByClientRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = ctx.CampaignClientContexts.Where(t => t.ClientId == request.ClientId);

            if (request.Take != 0)
               contexts = contexts.Take(request.Take);

            contexts = contexts.Skip(request.Skip).Include(t => t.Conditions);
            return new GetContextsByClientResponse()
            {
                Contexts = await contexts.Select(t => t.ToGrpcModel()).ToListAsync()
            };
        }

        public async Task<GetContextsByClientResponse> GetActiveContextsByClient(GetContextsByClientRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = ctx.CampaignClientContexts
                .Where(t => t.ClientId == request.ClientId)
                .Where(t => !t.Conditions.Any() || t.Conditions.All(conditions => conditions.Status != ConditionStatus.Expired && conditions.Status != ConditionStatus.Blocked))
                .Skip(request.Skip);
            
            if (request.Take != 0)
                contexts = contexts.Take(request.Take);

            return new GetContextsByClientResponse()
            {
                Contexts = await contexts
                    .Include(t => t.Conditions)
                    .Select(t => t.ToGrpcModel()).
                    ToListAsync()
            };
        }
    }
}