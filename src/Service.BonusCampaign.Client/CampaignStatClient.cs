using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.DynamicLinkGenerator.Models;
using MyJetWallet.DynamicLinkGenerator.Services;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Context.ParamsModels;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.GrpcModels;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusCampaign.Domain.Models.Stats;
using Service.BonusCampaign.Grpc;
using Service.BonusCampaign.Grpc.Models;
using Service.MessageTemplates.Client;

namespace Service.BonusCampaign.Client
{
    public class CampaignStatClient : ICampaignStatService
    {
        private readonly IClientContextService _contextClient;
        private readonly IMyNoSqlServerDataReader<CampaignNoSqlEntity> _campaignReader;
        private readonly ITemplateClient _templateClient;
        private readonly IDynamicLinkClient _dynamicLinkClient;

        public CampaignStatClient(IClientContextService contextClient, ITemplateClient templateClient, IMyNoSqlServerDataReader<CampaignNoSqlEntity> campaignReader, IDynamicLinkClient dynamicLinkClient)
        {
            _contextClient = contextClient;
            _templateClient = templateClient;
            _campaignReader = campaignReader;
            _dynamicLinkClient = dynamicLinkClient;
        }

        public async Task<CampaignStatsResponse> GetCampaignsStats(CampaignStatRequest request)
        {
            var campaigns = _campaignReader.Get().Select(t=>t.Campaign).Where(campaign=>campaign.Contexts.Any(context=>context.ClientId == request.ClientId)).ToList();
            var contextResponse = await _contextClient.GetActiveContextsByClient(new GetContextsByClientRequest() { ClientId = request.ClientId });

            var ids = campaigns.Select(t => t.Id).ToList();
            var contexts = contextResponse.Contexts.Where(t => ids.Contains(t.CampaignId)).ToList();

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
                    Title =
                        await _templateClient.GetTemplateBody(campaign.TitleTemplateId, request.Brand, request.Lang),
                    Description = await _templateClient.GetTemplateBody(campaign.DescriptionTemplateId, request.Brand,
                        request.Lang),
                    ExpirationTime = GetExpirationTime(context.Conditions),
                    Conditions = conditionStates,
                    ImageUrl = campaign.ImageUrl,
                    CampaignId = campaign.Id,
                    DeepLink = _dynamicLinkClient.GenerateInviteFriendLink(new GenerateInviteFriendLinkRequest()
                    {
                        Brand = request.Brand,
                        DeviceType = DeviceTypeEnum.Unknown
                    }),
                };

                stats.Add(stat);
            }

            return new CampaignStatsResponse
            {
                Campaigns = stats
            };

            //locals 
            ConditionStatModel GetConditionStat(ConditionStateGrpcModel state, List<RewardGrpcModel> rewards)
            {
                switch (state.Type)
                {
                    case ConditionType.KYCCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.KYCCondition,
                            Params = new Dictionary<string, string>()
                                { { "Passed", (state.Status == ConditionStatus.Met).ToString().ToLower()  } },
                            Reward = GetRewardStat(rewards.FirstOrDefault(t => t.ConditionId == state.ConditionId))
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
                            {
                                { "Asset", paramsModel.TradeAsset },
                                { "RequiredAmount", paramsModel.RequiredAmount.ToString() },
                                { "TradedAmount", paramsModel.TradeAmount.ToString() },
                                { "Passed", (state.Status == ConditionStatus.Met).ToString().ToLower()  }
                            },
                            Reward = GetRewardStat(rewards.FirstOrDefault(t => t.ConditionId == state.ConditionId))
                        };
                    }
                    case ConditionType.DepositCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.DepositCondition,
                            Params = new Dictionary<string, string>()
                                { { "Passed", (state.Status == ConditionStatus.Met).ToString().ToLower() } },
                            Reward = GetRewardStat(rewards.FirstOrDefault(t => t.ConditionId == state.ConditionId))
                        };
                    default:
                        return null;
                }
            }

            RewardStatModel GetRewardStat(RewardGrpcModel reward)
            {
                if (reward == null)
                    return null;

                var parameters = reward.Parameters;
                return new RewardStatModel
                {
                    Amount = decimal.Parse(parameters[RewardBase.AmountParam]),
                    Asset = parameters[RewardBase.PaidAsset]
                };
            }

            DateTime GetExpirationTime(List<ConditionStateGrpcModel> states)
            { 
                var notMetStates = states.Where(t =>
                    t.Status == ConditionStatus.NotMet 
                    && t.Type != ConditionType.ConditionsCondition).ToList();
                    
                return notMetStates.Any() ? notMetStates.Min(t=>t.ExpirationTime) : DateTime.MinValue;
            }        
        }
    }
}