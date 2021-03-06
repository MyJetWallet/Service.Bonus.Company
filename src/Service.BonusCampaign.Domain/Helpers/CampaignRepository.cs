using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Domain.Helpers
{
    public class CampaignRepository
    {
        private readonly IMyNoSqlServerDataWriter<CampaignNoSqlEntity> _campaignWriter;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignClientContextCacheManager _contextCacheManager;
        private readonly ILogger<CampaignRepository> _logger;
        public CampaignRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<CampaignNoSqlEntity> campaignWriter, CampaignClientContextCacheManager contextCacheManager, ILogger<CampaignRepository> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignWriter = campaignWriter;
            _contextCacheManager = contextCacheManager;
            _logger = logger;
        }

        
        public async Task<Campaign> GetCampaign(string campaignId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                
                var ret = await ctx.Campaigns.Where(campaign =>
                        campaign.Id == campaignId)
                    .Include(t => t.CriteriaList)
                    .Include(t => t.Conditions)
                    .Include(t=>t.CampaignClientContexts)
                    .FirstOrDefaultAsync();
                
                return ret;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting GetCampaign for client {campaignId}. Execution time {time}", campaignId,  stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("GetCampaign ran for {time}", stopwatch.Elapsed);
            }
        }
        
        public async Task<List<Campaign>> GetCampaignsWithoutThisClient(string clientId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaigns = await ctx.CampaignClientContexts.Where(context => context.ClientId == clientId)
                    .Select(t => t.CampaignId).Distinct().ToListAsync();

                var ret = await ctx.Campaigns.Where(campaign =>
                    campaign.Status == CampaignStatus.Active &&
                    !campaigns.Contains(campaign.Id))
                    .Include(t => t.CriteriaList)
                    .Include(t => t.Conditions)
                    .ToListAsync();
                
                return ret;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting GetCampaignsWithoutThisClient for client {clientId}. Execution time {time}", clientId,  stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("GetCampaignsWithoutThisClient ran for {time}", stopwatch.Elapsed);
            }
        }

        public async Task<List<Campaign>> GetCampaigns()
        {            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                return await ctx.Campaigns
                    .Include(t => t.CriteriaList)
                    .Include(t => t.Conditions)
                    .ThenInclude(t => t.Rewards)
                    .Include(t => t.CampaignClientContexts)
                    .ThenInclude(t => t.Conditions)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing GetCampaigns. Execution time {time}", stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("GetCampaigns ran for {time}", stopwatch.Elapsed);
            }
        }
        
        public async Task SetActiveCampaigns(List<Campaign> campaigns)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await _campaignWriter.BulkInsertOrReplaceAsync(campaigns.Select(CampaignNoSqlEntity.Create).ToList());
                await _contextCacheManager.UpdateContext(campaigns);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await ctx.UpsertAsync(campaigns);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing SetActiveCampaigns. Execution time {time}", stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("SetActiveCampaigns ran for {time}", stopwatch.Elapsed);
            }
        }
        
        public async Task SetFinishedCampaigns(List<Campaign> campaigns)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                foreach (var campaign in campaigns)
                {
                    await _campaignWriter.DeleteAsync(CampaignNoSqlEntity.GeneratePartitionKey(), CampaignNoSqlEntity.GenerateRowKey(campaign.Id));
                }
                await _contextCacheManager.UpdateContext(campaigns);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await ctx.UpsertAsync(campaigns);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing SetFinishedCampaigns. Execution time {time}", stopwatch.Elapsed);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("SetFinishedCampaigns ran for {time}", stopwatch.Elapsed);
            }
        }
        
        public async Task RefreshCampaign(string campaignId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var campaign = await ctx.Campaigns
                .Where(t=>t.Id == campaignId)
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ThenInclude(t=>t.Rewards)
                .Include(t=>t.CampaignClientContexts)
                .ThenInclude(t=>t.Conditions)
                .FirstOrDefaultAsync();

            await _contextCacheManager.UpdateContext(new List<Campaign>(){campaign});
            await _campaignWriter.InsertOrReplaceAsync(CampaignNoSqlEntity.Create(campaign));
        }
    }
}