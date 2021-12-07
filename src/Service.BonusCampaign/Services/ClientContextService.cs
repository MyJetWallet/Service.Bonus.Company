using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var contexts = await ctx.CampaignClientContexts.Where(t => t.ClientId == request.ClientId)
                .Include(t => t.Conditions).ToListAsync();
            return new GetContextsByClientResponse()
            {
                Contexts = contexts.Select(t => t.ToGrpcModel()).ToList()
            };
        }
    }
}