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
using Service.DynamicLinkGenerator.Domain.Models.Enums;
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
            var campaignList = _campaignReader.Get().Select(t => t.Campaign).ToList();
            var clientContextList = await _contextClient.GetActiveContextsByClient(new GetContextsByClientRequest
            {
                ClientId = request.ClientId
            });
            if (clientContextList.Contexts == null || !clientContextList.Contexts.Any())
            {
                return new CampaignStatsResponse
                {
                    Campaigns = new List<CampaignStatModel>()
                };
            }
            var ids = campaignList
                .Where(e => e.Status== CampaignStatus.Active)
                .Select(t => t.Id)
                .ToList();
            
            if (!ids.Any())
            {
                return new CampaignStatsResponse
                {
                    Campaigns = new List<CampaignStatModel>()
                };
            }
            var contexts = clientContextList
                .Contexts
                .Where(t => ids.Contains(t.CampaignId))
                .ToList();

            var conditions = campaignList
                .SelectMany(t => t.Conditions ?? new List<ConditionGrpcModel>())
                .ToList();

            var rewards = conditions
                .SelectMany(t => t.Rewards ?? new List<RewardGrpcModel>())
                .Where(t => t.Type == RewardType.ClientPaymentAbsolute)
                .ToList();

            var stats = new List<CampaignStatModel>();

            foreach (var context in contexts)
            {
                var campaign = campaignList.First(t => t.Id == context.CampaignId);

                context.Conditions ??= new List<ConditionStateGrpcModel>();
                
                var conditionStates = (context.Conditions)
                    .Where(t => t.Type != ConditionType.ConditionsCondition)
                    .Select(condition => GetConditionStat(condition, rewards))
                    .OrderByDescending(t=>t.Weight)
                    .ToList();

                var (longLink, shortLink) = GenerateDeepLink(campaign.Action, campaign.SerializedRequest, request.Brand);
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
                    DeepLink = shortLink,
                    Weight = campaign.Weight,
                    DeepLinkWeb = longLink,
                    ShowReferrerStats = campaign.ShowReferrerStats,
                    CampaignType = campaign.CampaignType
                };

                stats.Add(stat);
            }

            return new CampaignStatsResponse
            {
                Campaigns = stats.OrderByDescending(model=>model.Weight).ToList()
            };

            //locals 
            ConditionStatModel GetConditionStat(ConditionStateGrpcModel state, List<RewardGrpcModel> rewardsList)
            {
                var condition = conditions.First(t => t.ConditionId == state.ConditionId);
                var (longLink, shortLink) = GenerateDeepLink(condition.Action, null, request.Brand);

                switch (state.Type)
                {
                    case ConditionType.KYCCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.KYCCondition,
                            Params = new Dictionary<string, string>()
                                { { "Passed", (state.Status == ConditionStatus.Met).ToString().ToLower()  } },
                            Reward = GetRewardStat(rewardsList.FirstOrDefault(t => t.ConditionId == state.ConditionId)),
                            DeepLink = shortLink,
                            DeepLinkWeb = longLink,
                            Weight = condition.Weight
                        };
                    case ConditionType.TradeCondition:
                    {
                        TradeParamsModel paramsModel;

                        if (string.IsNullOrEmpty(state.Params))
                        {
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
                            Reward = GetRewardStat(rewardsList.FirstOrDefault(t => t.ConditionId == state.ConditionId)),
                            DeepLink = shortLink,
                            DeepLinkWeb = longLink,
                            Weight = condition.Weight
                        };
                    }
                    case ConditionType.DepositCondition:
                        return new ConditionStatModel
                        {
                            Type = ConditionType.DepositCondition,
                            Params = new Dictionary<string, string>()
                                { { "Passed", (state.Status == ConditionStatus.Met).ToString().ToLower() } },
                            Reward = GetRewardStat(rewardsList.FirstOrDefault(t => t.ConditionId == state.ConditionId)),
                            DeepLink = shortLink,
                            DeepLinkWeb = longLink,
                            Weight = condition.Weight
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

            (string longLink, string shortLink) GenerateDeepLink(ActionEnum action, string serializedRequest, string brand)
            {
                switch (action)
                {
                    case ActionEnum.InviteFriend:
                        return _dynamicLinkClient.GenerateInviteFriendLink(new ()
                        {
                            Brand = brand,
                            DeviceType = DeviceTypeEnum.Unknown
                        });;
                    case ActionEnum.KycVerification:
                        return _dynamicLinkClient.GenerateKycVerificationLink(new ()
                        {
                            Brand = brand,
                            DeviceType = DeviceTypeEnum.Unknown
                        });;
                    case ActionEnum.DepositStart:
                        return _dynamicLinkClient.GenerateDepositStartLink(new ()
                        {
                            Brand = brand,
                            DeviceType = DeviceTypeEnum.Unknown
                        });;;
                    case ActionEnum.TradingStart:
                        return _dynamicLinkClient.GenerateTradingStartLink(new ()
                        {
                            Brand = brand,
                            DeviceType = DeviceTypeEnum.Unknown
                        });
                    case ActionEnum.EarnLanding:
                        return _dynamicLinkClient.GenerateEarnLandingLink(new GenerateEarnLandingLinkRequest()
                        {
                            Brand = brand,
                            DeviceType = DeviceTypeEnum.Unknown
                        });
                    default:
                        return (String.Empty, String.Empty);
                }
            }
        }
    }
}