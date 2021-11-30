using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Postgres;
using Service.BonusCampaign.Worker.Helpers;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Worker.Jobs
{
    public class CheckerJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignClientContextRepository _contextRepository;
        private readonly CampaignRepository _campaignRepository;
        private readonly IServiceBusPublisher<ExecuteRewardMessage> _publisher;
        private readonly ILogger<CheckerJob> _logger;
        private readonly IConvertIndexPricesClient _pricesClient;
        private readonly CampaignsRegistry _campaignsRegistry;
        public CheckerJob(ISubscriber<ContextUpdate> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignClientContextRepository contextRepository, CampaignRepository campaignRepository, IServiceBusPublisher<ExecuteRewardMessage> publisher, ILogger<CheckerJob> logger, IConvertIndexPricesClient pricesClient, CampaignsRegistry campaignsRegistry)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _contextRepository = contextRepository;
            _campaignRepository = campaignRepository;
            _publisher = publisher;
            _logger = logger;
            _pricesClient = pricesClient;
            _campaignsRegistry = campaignsRegistry;
            subscriber.Subscribe(HandleUpdates);
        }

        private async ValueTask HandleUpdates(ContextUpdate update)
        {
            await HandleCriteriaChecks(update);
            await HandleConditionChecks(update);
        }

        private async Task HandleCriteriaChecks(ContextUpdate update)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var contexts = new List<CampaignClientContext>();

                var campaigns = await _campaignRepository.GetCampaignsWithoutThisClient(update.ClientId);

                foreach (var campaign in campaigns)
                {
                    var results = new List<bool>();
                    foreach (var criteria in campaign.CriteriaList)
                    {
                        results.Add(await criteria.Check(update.Context));
                    }

                    if (results.TrueForAll(t => t))
                    {
                        contexts.Add(new CampaignClientContext
                        {
                            ClientId = update.ClientId,
                            CampaignId = campaign.Id,
                            ActivationTime = DateTime.UtcNow,
                            Conditions = campaign.Conditions.Select(condition => new ClientConditionState
                            {
                                CampaignId = campaign.Id, ClientId = update.ClientId,
                                ConditionId = condition.ConditionId, Type = condition.Type,
                                Status = ConditionStatus.NotMet,
                            }).ToList()
                        });

                        await _campaignsRegistry.AddCampaign(update.ClientId, campaign.Id);
                    }
                }

                await _contextRepository.UpsertContext(contexts);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When checking criteria for update {requestJson}", JsonSerializer.Serialize(update));
                Thread.Sleep(10000);
                throw;
            }
        }

        private async Task HandleConditionChecks(ContextUpdate update)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var contexts = await _contextRepository.GetContextById(update.ClientId);
                var conditionIds = contexts.SelectMany(t => t.Conditions.Select(state => state.ConditionId)).ToList();
                var conditions = ctx.Conditions
                    .Where(condition => conditionIds.Contains(condition.ConditionId)
                                        && condition.Type == update.EventType.ToConditionType())
                    .Include(condition => condition.Rewards)
                    .ToList();

                var afterConditions = ctx.Conditions
                    .Where(condition => conditionIds.Contains(condition.ConditionId)
                                        && condition.Type == ConditionType.ConditionsCondition)
                    .Include(condition => condition.Rewards)
                    .ToList();
                
                foreach (var context in contexts)
                {
                    foreach (var condition in conditions)
                    {                        
                        var conditionState = context.Conditions.FirstOrDefault(t => t.ConditionId == condition.ConditionId);
                        if (conditionState != null)
                        {
                            conditionState.Params =
                                await condition.UpdateConditionStateParams(update, conditionState.Params, _pricesClient);
                            
                            conditionState.Status = await condition.Check(update, _publisher, conditionState.Params, context);
                        }
                    }

                    foreach (var afterCondition in afterConditions)
                    {
                        var conditionState = context.Conditions.FirstOrDefault(t => t.ConditionId == afterCondition.ConditionId);
                        if (conditionState != null)
                        {
                            conditionState.Status = await afterCondition.Check(update, _publisher, conditionState.Params, context);
                        }
                    }
                }

                await _contextRepository.UpsertContext(contexts);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When checking conditions for update {requestJson}",
                    JsonSerializer.Serialize(update));
                Thread.Sleep(10000);
                throw;
            }
        }
    }
}