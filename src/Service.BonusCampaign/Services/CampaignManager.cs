using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusCampaign.Domain;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;
using Service.BonusCampaign.Settings;

namespace Service.BonusCampaign.Services
{
    public class CampaignManager: ICampaignManager
    {
        private readonly ILogger<CampaignManager> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly CampaignRepository _campaignRepository;
        private readonly CampaignClientContextRepository _contextRepository;
        private readonly ClientContextService _contextService;

        public CampaignManager(ILogger<CampaignManager> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, CampaignRepository campaignRepository, CampaignClientContextRepository contextRepository, ClientContextService contextService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _campaignRepository = campaignRepository;
            _contextRepository = contextRepository;
            _contextService = contextService;
        }

        public async Task<OperationResponse> CreateOrUpdateCampaign(CampaignGrpcModel request)
        {
            var campaignId = request.Id ?? Guid.NewGuid().ToString("N");
            var criteriaList = new List<AccessCriteriaBase>();
            var conditions = new List<ConditionBase>();
            try
            {
                request.CriteriaList ??= new ();
                request.Conditions ??= new ();

                criteriaList.AddRange(request.CriteriaList.Select(criteriaRequest => AccessCriteriaFactory.CreateCriteria(criteriaRequest.CriteriaType, criteriaRequest.Parameters, criteriaRequest.CriteriaId, campaignId)));

                if(request.Conditions.Any())
                    conditions.AddRange(
                    from conditionRequest 
                        in request.Conditions 
                    let rewards = conditionRequest.Rewards?.Select(rewardRequest => RewardFactory.CreateReward(rewardRequest.Type, rewardRequest.Parameters, rewardRequest.RewardId, conditionRequest.ConditionId)).ToList() ?? new List<RewardBase>()
                    select ConditionFactory.CreateCondition(conditionRequest.Type, conditionRequest.Parameters, rewards, campaignId, conditionRequest.ConditionId, conditionRequest.TimeToComplete, conditionRequest.Action, conditionRequest.Weight));

                var campaign = new Campaign
                {
                    Id = campaignId,
                    TitleTemplateId = request.TitleTemplateId,
                    DescriptionTemplateId = request.DescriptionTemplateId,
                    FromDateTime = request.FromDateTime,
                    ToDateTime = request.ToDateTime,
                    IsEnabled = request.IsEnabled,
                    Status = request.Status,
                    ImageUrl = request.ImageUrl,
                    CriteriaList = criteriaList,
                    Conditions = conditions,
                    SerializedRequest = request.SerializedRequest,
                    Action = request.Action,
                    Name = request.Name,
                    Weight = request.Weight,
                    ShowReferrerStats = request.ShowReferrerStats,
                    CampaignType = string.IsNullOrWhiteSpace(request.CampaignType) ? CampaignTypes.None : request.CampaignType
                };
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                await context.UpsertAsync(new[] { campaign });
                await context.UpsertAsync(campaign.CriteriaList);
                await context.UpsertAsync(campaign.Conditions);
                await context.UpsertAsync(campaign.Conditions.SelectMany(t=>t.Rewards));

                if (campaign.Status == CampaignStatus.Active)
                {
                    await _campaignRepository.RefreshCampaign(campaignId);
                }
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When creating campaign");
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetAllCampaignsResponse> GetAllCampaigns()
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var campaigns = context.Campaigns
                .Include(t=>t.CriteriaList)
                .Include(t=>t.Conditions)
                .ThenInclude(t=>t.Rewards)
                .Include(t=>t.CampaignClientContexts)
                .ThenInclude(t=>t.Conditions)
                .Select(t => t.ToGrpcModel()).ToList();
            return new GetAllCampaignsResponse
            {
                Campaigns = campaigns
            };
        }

        public ParamsResponse GetAllParams()
        {
            return new ParamsResponse
            {
                CriteriaParams = (from CriteriaType type in Enum.GetValues(typeof(CriteriaType)) select new ParamEntity() { Type = type.ToString(), Params = ToListOfParams(AccessCriteriaFactory.GetParams(type)) }).ToList(),
                ConditionParams = (from ConditionType type in Enum.GetValues(typeof(ConditionType)) select new ParamEntity() { Type = type.ToString(), Params = ToListOfParams(ConditionFactory.GetParams(type)) }).ToList(),
                RewardParams = (from RewardType type in Enum.GetValues(typeof(RewardType)) select new ParamEntity() { Type = type.ToString(), Params = ToListOfParams(RewardFactory.GetParams(type)) }).ToList()
            };

            //locals
            List<ParamsDict> ToListOfParams(Dictionary<string, string> parameters) => parameters.Select(keyValuePair => new ParamsDict { ParamName = keyValuePair.Key, ParamType = keyValuePair.Value }).ToList();
        }

        public async Task<GetContextsResponse> GetContextsByClient(GetContextsByClientRequest request) => await _contextService.GetContextsByClient(request);

        public async Task<GetContextsResponse> GetContextsByCampaign(GetContextsByCampaignRequest request) => await _contextService.GetContextsByCampaign(request);

        public async Task<OperationResponse> BlockUserInCampaign(BlockUserRequest request)
        {
            _logger.LogInformation("Blocking client {clientId} at campaign {campaignId}", request.ClientId, request.CampaignId);
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var contexts = await ctx.CampaignClientContexts
                    .Include(t=>t.Conditions)
                    .Where(t => t.ClientId == request.ClientId && t.CampaignId == request.CampaignId).ToListAsync();

                if (contexts.Any())
                {
                    foreach (var condition in contexts.Where(context=>context.Conditions.Any()).SelectMany(context => context.Conditions.Where(condition => condition.Status == ConditionStatus.NotMet)))
                    {
                        condition.Status = ConditionStatus.Blocked;
                    }
                }
                
                await _contextRepository.UpsertContext(contexts);

                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When blocking client {clientId}", request.ClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> RemoveReward(RemoveRewardRequest request)
        {
            _logger.LogInformation("Removing reward {rewardId}", request.RewardId);
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var reward = await ctx.Rewards.FirstOrDefaultAsync(t => t.RewardId == request.RewardId); 
                if (reward != null)
                {
                    ctx.Rewards.Remove(reward);
                    await ctx.SaveChangesAsync();

                    var condition = await ctx.Conditions.FirstOrDefaultAsync(t => t.ConditionId == reward.ConditionId);
                    if(condition != null)
                        await _campaignRepository.RefreshCampaign(condition.CampaignId);

                }
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When removing reward {rewardId}", request.RewardId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }        
        }

        public async Task<OperationResponse> RemoveCondition(RemoveConditionRequest request)
        {
            _logger.LogInformation("Removing condition {conditionId}", request.ConditionId);
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var condition = await ctx.Conditions.FirstOrDefaultAsync(t => t.ConditionId == request.ConditionId);
                if (condition != null)
                {
                    ctx.Conditions.Remove(condition);
                    await ctx.SaveChangesAsync();
                    await _campaignRepository.RefreshCampaign(condition.CampaignId);
                }
                var states = await ctx.ClientConditionStates.Where(t => t.ConditionId == request.ConditionId)
                    .ToListAsync();
                if (states.Any())
                {
                    ctx.ClientConditionStates.RemoveRange(states);
                    await ctx.SaveChangesAsync();
                    await _contextRepository.CleanCache();
                }
                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When removing condition {conditionId}", request.ConditionId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> UnblockUserInCampaign(BlockUserRequest request)
        {
            _logger.LogInformation("Unblocking client {clientId} at campaign {campaignId}", request.ClientId, request.CampaignId);
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var contexts = await ctx.CampaignClientContexts
                    .Include(t=>t.Conditions)
                    .Where(t => t.ClientId == request.ClientId && t.CampaignId == request.CampaignId).ToListAsync();

                if (contexts.Any())
                {
                    foreach (var condition in contexts.Where(context=>context.Conditions.Any()).SelectMany(context => context.Conditions.Where(condition => condition.Status == ConditionStatus.Blocked)))
                    {
                        condition.Status = ConditionStatus.NotMet;
                    }
                }
                
                await _contextRepository.UpsertContext(contexts);

                return new OperationResponse() { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When unblocking client {clientId}", request.ClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
