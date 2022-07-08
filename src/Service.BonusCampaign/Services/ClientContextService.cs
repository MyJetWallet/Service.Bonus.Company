using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
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
        private readonly ILogger<ClientContextService> _logger;
        public ClientContextService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<ClientContextService> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
        }

        public async Task<GetContextsResponse> GetContextsByClient(GetContextsByClientRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = ctx.CampaignClientContexts
                .Where(t => t.ClientId == request.ClientId)
                .Skip(request.Skip);

            if (request.Take != 0)
                contexts = contexts.Take(request.Take);

            return new GetContextsResponse()
            {
                Contexts = await contexts
                    .Include(t => t.Conditions)
                    .Select(t => t.ToGrpcModel())
                    .ToListAsync()
            };
        }

        public async Task<GetContextsResponse> GetActiveContextsByClient(GetContextsByClientRequest request)
        {
            _logger.LogInformation("Got request for GetActiveContextsByClient with request {request}", request.ToJson());
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = ctx.CampaignClientContexts
                .Where(t => t.ClientId == request.ClientId)
                .Where(t => !t.Conditions.Any() || t.Conditions.All(conditions =>
                    conditions.Status != ConditionStatus.Expired && conditions.Status != ConditionStatus.Blocked))
                .Skip(request.Skip);

            if (request.Take != 0)
                contexts = contexts.Take(request.Take);

            return new GetContextsResponse()
            {
                Contexts = await contexts
                    .Include(t => t.Conditions)
                    .Select(t => t.ToGrpcModel()).ToListAsync()
            };
        }


        public async Task<GetContextsResponse> GetContextsByCampaign(GetContextsByCampaignRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = ctx.CampaignClientContexts
                .Where(t => t.CampaignId == request.CampaignId)
                .Skip(request.Skip);
            
            if (request.Take != 0)
                contexts = contexts.Take(request.Take);
            
            return new GetContextsResponse()
            {
                Contexts = await contexts.
                    Include(t => t.Conditions)
                    .Select(t => t.ToGrpcModel())
                    .ToListAsync()
            };
        }
    }
}