using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MyJetWallet.DynamicLinkGenerator.Models;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Context.ParamsModels;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class TradeCondition : ConditionBase
    {
        public const string TradeAssetParam = "TradeAsset";
        public const string TradeAmountParam = "TradeAmountInSelectedAsset";

        private string _tradeAsset;
        private decimal _tradeAmount;

        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.TradeCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }
        public override ActionEnum Action { get; set; }
        public override int Weight { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override string DescriptionTemplateId { get; set; }

        public TradeCondition()
        {
        }
        public TradeCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete, ActionEnum action, int weight, string descriptionTemplateId)
        {
            Type = ConditionType.TradeCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.TradeMade };
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            TimeToComplete = timeToComplete;
            Action = action;
            Weight = weight;
            DescriptionTemplateId = descriptionTemplateId;
            
            LastUpdate = DateTime.UtcNow;
            
            Init();
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task<ConditionStatus> Check(ContextUpdate context,
            IServiceBusPublisher<ExecuteRewardMessage> publisher, string paramsJson,
            CampaignClientContext campaignContext)
        {
            if (IsExpired(campaignContext.ActivationTime))
                return ConditionStatus.Expired;
            
            if (string.IsNullOrEmpty(paramsJson))
                return ConditionStatus.NotMet;

            Init();
            TradeParamsModel model;
            try
            {
                model = JsonSerializer.Deserialize<TradeParamsModel>(paramsJson);
            }
            catch (JsonException e)
            {
                return ConditionStatus.NotMet;
            }

            if (model.TradeAmount >= _tradeAmount)
            {
                foreach (var reward in Rewards)
                {
                    await reward.ExecuteReward(context, publisher);
                }
                return ConditionStatus.Met;
            }
            return ConditionStatus.NotMet;
        }

        public override async Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient)
        {
            Init();
            var convertPrice = pricesClient.GetConvertIndexPriceByPairAsync(context.TradeEvent.ToAssetId, _tradeAsset);
           
            var model =  new TradeParamsModel
            {
                TradeAmount = 0,
                RequiredAmount = _tradeAmount,
                TradeAsset = _tradeAsset
            };
            
            if (!string.IsNullOrWhiteSpace(paramsJson))
            {
                model = JsonSerializer.Deserialize<TradeParamsModel>(paramsJson) ?? new TradeParamsModel
                {
                    TradeAmount = 0,
                    RequiredAmount = _tradeAmount,
                    TradeAsset = _tradeAsset
                };
            }

            model.TradeAmount += context.TradeEvent.ToAmount * convertPrice.Price;
            return JsonSerializer.Serialize(model);
        }

        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { TradeAssetParam, typeof(string).ToString() },
            { TradeAmountParam, typeof(decimal).ToString() },
        };

        private void Init()
        {
            if (!Parameters.TryGetValue(TradeAmountParam, out var tradeAmount) || !decimal.TryParse(tradeAmount, out _tradeAmount) || !Parameters.TryGetValue(TradeAssetParam, out _tradeAsset) || string.IsNullOrWhiteSpace(_tradeAsset))
            {
                throw new Exception("Invalid arguments");
            }
        }
    }
}