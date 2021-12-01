using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Context.ParamsModels;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusCampaign.Domain.Models.Stats;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Services
{
    public class CampaignStatService : ICampaignStatService
    {
        private readonly ILogger<CampaignStatService> _logger;
        private readonly CampaignClientContextRepository _contextRepository;
        private readonly CampaignRepository _campaignRepository;
        public CampaignStatService(CampaignClientContextRepository contextRepository, ILogger<CampaignStatService> logger, CampaignRepository campaignRepository)
        {
            _contextRepository = contextRepository;
            _logger = logger;
            _campaignRepository = campaignRepository;
        }

        public async Task<CampaignStatsResponse> GetCampaignsStats(CampaignStatRequest request)
        {
            var campaigns = await _campaignRepository.GetActiveCampaigns(request.ClientId);
            var contexts = await _contextRepository.GetContextById(request.ClientId);

            var ids = campaigns.Select(t => t.Id).ToList();
            contexts = contexts.Where(t => ids.Contains(t.CampaignId)).ToList();
            
            var conditions = campaigns
                .SelectMany(t => t.Conditions)
                .ToList();
            
            var rewards = conditions
                .SelectMany(t => t.Rewards)
                .Where(t => t.Type == RewardType.ClientPaymentAbsolute)
                .ToList();
            
            var stats = new List<CampaignStatModel>();

            foreach (var context in contexts)
            {
                var campaign = campaigns.First(t => t.Id == context.CampaignId);

                var conditionStates = context.Conditions
                    .Where(t => t.Type != ConditionType.ConditionsCondition)
                    .Select(condition => GetConditionStat(condition, rewards))
                    .ToList();
                
                var stat = new CampaignStatModel
                {
                    Title = campaign.Name,
                    Description = "TODO: add description",
                    TimeToComplete = default,
                    Conditions = conditionStates,
                    ImageUrl = "TODO: add image url",
                };
                
                stats.Add(stat);
            }

            return new CampaignStatsResponse
            {
                Campaigns = stats
            };

            //locals 
            ConditionStatModel GetConditionStat(ClientConditionState state, List<RewardBase> rewards)
            {
                switch (state.Type)
                {
                    case ConditionType.KYCCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.KYCCondition,
                            Params = new Dictionary<string, string>(){{"Passed", (state.Status == ConditionStatus.Met).ToString()}},
                            Reward = GetRewardStat(rewards.FirstOrDefault(t=>t.ConditionId == state.ConditionId))
                        };
                    case ConditionType.TradeCondition:
                    {
                        TradeParamsModel paramsModel;

                        if (string.IsNullOrEmpty(state.Params))
                        {
                            var condition = conditions.First(t => t.ConditionId == state.ConditionId);

                            var condParams = condition.Parameters;
                            paramsModel = new TradeParamsModel
                            {
                                TradeAmount = 0,
                                RequiredAmount = decimal.Parse(condParams[TradeCondition.TradeAmountParam]),
                                TradeAsset = condParams[TradeCondition.TradeAssetParam]
                            };
                        }
                        else
                        {
                            paramsModel = JsonSerializer.Deserialize<TradeParamsModel>(state.Params);
                        }

                        return new ConditionStatModel
                        {
                            Type = ConditionType.TradeCondition,
                            Params = new Dictionary<string, string>()
                                { { "Asset", paramsModel.TradeAsset }, { "RequiredAmount", paramsModel.RequiredAmount.ToString() }, { "TradedAmount", paramsModel.TradeAmount.ToString() } },
                            Reward = GetRewardStat(rewards.FirstOrDefault(t=>t.ConditionId == state.ConditionId))
                        };
                    }
                    case ConditionType.DepositCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.DepositCondition,
                            Params = new Dictionary<string, string>(){{"Passed", (state.Status == ConditionStatus.Met).ToString()}},
                            Reward = GetRewardStat(rewards.FirstOrDefault(t=>t.ConditionId == state.ConditionId))
                        };
                    default:
                        return null;
                }
            }

            RewardStatModel GetRewardStat(RewardBase reward)
            {
                if (reward == null) 
                    return null;
                
                var parameters = reward.GetParams();
                return new RewardStatModel
                {
                    Amount = decimal.Parse(parameters[RewardBase.AmountParam]),
                    Asset = parameters[RewardBase.PaidAsset]
                };
            }
        }
    }
}