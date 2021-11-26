using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ConditionsCondition : ConditionBase
    {
        private const string ConditionsParam = "ConditionsList";
        private readonly List<string> _conditions;
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.ConditionsCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }

        public ConditionsCondition()
        {
        }
        public ConditionsCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete)
        {
            Type = ConditionType.ConditionsCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>();
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            TimeToComplete = timeToComplete;

            if (!parameters.TryGetValue(ConditionsParam, out var conditionsString))
            {
                throw new Exception("Invalid arguments");
            }

            try
            {
                _conditions = conditionsString.Split(';').ToList();
            }
            catch
            {
                throw new Exception("Invalid arguments");
            }

            if (!_conditions.Any())
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
            
            var passed = campaignContext.Conditions.All(conditionState => _conditions.Contains(conditionState.ConditionId) && conditionState.Status == ConditionStatus.Met);
            if (passed)
            {
                foreach (var reward in Rewards)
                {
                    await reward.ExecuteReward(context, publisher);
                }
                return true;
            }
            
            return false;
        }

        public override Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient) => Task.FromResult(paramsJson);

        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { ConditionsParam, typeof(string).ToString() },
        };
    }
}