using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class TradeCondition : ConditionBase
    {
        private const string TradeAssetParam = "TradeAsset";
        private const string TradeAmountParam = "TradeAmountInSelectedAsset";

        private readonly string _tradeAsset;
        private readonly decimal _tradeAmount;

        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.TradeCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }

        public TradeCondition()
        {
        }
        public TradeCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete)
        {
            Type = ConditionType.TradeCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.TradeMade };
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            TimeToComplete = timeToComplete;
            
            if (!parameters.TryGetValue(TradeAmountParam, out var tradeAmount) || !decimal.TryParse(tradeAmount, out _tradeAmount) || !parameters.TryGetValue(TradeAssetParam, out _tradeAsset) || string.IsNullOrWhiteSpace(_tradeAsset))
            {
                throw new Exception("Invalid arguments");
            }
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task<bool> Check(ContextUpdate context,
            IServiceBusPublisher<ExecuteRewardMessage> publisher, string paramsJson,
            CampaignClientContext campaignContext)
        {
            if (campaignContext.ActivationTime + TimeToComplete <= DateTime.UtcNow)
                return false;
            
            if (string.IsNullOrEmpty(paramsJson))
                return false;

            ParamsModel model;
            try
            {
                model = JsonSerializer.Deserialize<ParamsModel>(paramsJson);
            }
            catch (JsonException e)
            {
                return false;
            }

            if (model.TradeAmount >= _tradeAmount)
            {
                foreach (var reward in Rewards)
                {
                    await reward.ExecuteReward(context, publisher);
                }
                return true;
            }
            return false;
        }

        public override async Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient)
        {
            var convertPrice = pricesClient.GetConvertIndexPriceByPairAsync(context.TradeEvent.ToAssetId, _tradeAsset);
            
            var model = string.IsNullOrWhiteSpace(paramsJson)
                ? new ParamsModel
                {
                    TradeAmount = 0
                }
                : JsonSerializer.Deserialize<ParamsModel>(paramsJson);
            
            model.TradeAmount += context.TradeEvent.ToAmount * convertPrice.Price;
            return JsonSerializer.Serialize(model);
        }

        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { TradeAssetParam, typeof(string).ToString() },
            { TradeAmountParam, typeof(decimal).ToString() },
        };


        private class ParamsModel
        {
            public decimal TradeAmount { get; set; }
        }

    }
}