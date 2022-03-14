using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignClientContextRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignClientContextCacheManager _clientContextCache;
        private readonly ILogger<CampaignClientContextRepository> _logger;
        public CampaignClientContextRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignClientContextCacheManager clientContextCache, ILogger<CampaignClientContextRepository> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _clientContextCache = clientContextCache;
            _logger = logger;
        }

        public async Task<List<CampaignClientContext>> GetContextById(string clientId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var cached = await _clientContextCache.GetContextsByClient(clientId);
                if (cached != null)
                    return cached;
                
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query =
                    from context in ctx.CampaignClientContexts
                    where context.ClientId == clientId
                    join campaign in ctx.Campaigns
                        on context.CampaignId equals campaign.Id
                    where campaign.Status == CampaignStatus.Active
                    select context;

                var ret = await query.Include(t => t.Conditions).ToListAsync();

                await _clientContextCache.UpdateContext(ret);
                return ret;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing GetContextById for client {clientId}. Execution time {time}", clientId, stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("GetContextById ran for {time}", stopwatch.Elapsed);
            }
        }
        
        public async Task UpsertContext(List<CampaignClientContext> contexts)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await _clientContextCache.UpdateContext(contexts);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await ctx.UpsertAsync(contexts);
                await ctx.UpsertAsync(contexts.SelectMany(t => t.Conditions));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing UpsertContext. Execution time {time}", stopwatch.Elapsed);
                throw;
            }
            finally
            {              
                stopwatch.Stop();
                _logger.LogInformation("UpsertContext ran for {time}", stopwatch.Elapsed);
            }
        }

        public async Task CleanCache()
        {
            await _clientContextCache.CleanCache();
        }
    }
}