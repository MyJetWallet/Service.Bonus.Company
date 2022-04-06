using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Context.ParamsModels;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.DynamicLinkGenerator.Domain.Models.Enums;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class DepositCondition : ConditionBase
    {
        public const string DepositAssetParam = "DepositAsset";
        public const string DepositAmountParam = "DepositAmountInSelectedAsset";

        private string _depositAsset;
        private decimal _depositAmount;

        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.DepositCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }
        public override ActionEnum Action { get; set; }
        public override int Weight { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override string DescriptionTemplateId { get; set; }

        public DepositCondition()
        {
        }
        public DepositCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete, ActionEnum action, int weight, string descriptionTemplateId)
        {
            Type = ConditionType.DepositCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.DepositMade };
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
            
            Init();
            DepositParamsModel model;
            try
            {
                model = JsonSerializer.Deserialize<DepositParamsModel>(paramsJson);
            }
            catch (JsonException e)
            {
                return ConditionStatus.NotMet;
            }

            if (model.DepositedAmount >= _depositAmount)
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
            var convertPrice = pricesClient.GetConvertIndexPriceByPairAsync(context.DepositEvent.AssetId, _depositAsset);

            var model = new DepositParamsModel
            {
                DepositedAmount = 0,
                RequiredAmount = _depositAmount,
                DepositAsset = _depositAsset
            };
            
            if (!string.IsNullOrWhiteSpace(paramsJson))
            {
                model = JsonSerializer.Deserialize<DepositParamsModel>(paramsJson) ?? new DepositParamsModel
                {
                    DepositedAmount = 0,
                    RequiredAmount = _depositAmount,
                    DepositAsset = _depositAsset
                };
            }

            model.DepositedAmount += context.DepositEvent.Amount * convertPrice.Price;
            return JsonSerializer.Serialize(model);
        }
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { DepositAssetParam, typeof(string).ToString() },
            { DepositAmountParam, typeof(decimal).ToString() },
        };
        
        private void Init()
        {
            if (!Parameters.TryGetValue(DepositAmountParam, out var amount) || !decimal.TryParse(amount, out _depositAmount) || !Parameters.TryGetValue(DepositAssetParam, out _depositAsset) || string.IsNullOrWhiteSpace(_depositAsset))
            {
                throw new Exception("Invalid arguments");
            }
        }
        
    }
}