﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusCampaign.Domain;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
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

        public CampaignManager(ILogger<CampaignManager> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
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
                    select ConditionFactory.CreateCondition(conditionRequest.Type, conditionRequest.Parameters, rewards, campaignId, conditionRequest.ConditionId));

                var campaign = new Campaign
                {
                    Id = campaignId,
                    Name = request.Name,
                    FromDateTime = request.FromDateTime,
                    ToDateTime = request.ToDateTime,
                    IsEnabled = request.IsEnabled,
                    Status = CampaignStatus.Scheduled,
                    BannerId = request.BannerId,
                    CriteriaList = criteriaList,
                    Conditions = conditions
                };
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                await context.UpsertAsync(new[] { campaign });
                await context.UpsertAsync(campaign.CriteriaList);
                await context.UpsertAsync(campaign.Conditions);
                await context.UpsertAsync(campaign.Conditions.SelectMany(t=>t.Rewards));


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
    }
}
