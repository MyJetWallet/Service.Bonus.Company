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
using Service.DynamicLinkGenerator.Domain.Models.Enums;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class ConditionsCondition : ConditionBase
    {
        private const string ConditionsParam = "ConditionsList";
        private const string AllowExpiredParam = "AllowExpired";

        private List<string> _conditions;
        private bool _allowExpired;
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.ConditionsCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }
        public override ActionEnum Action { get; set; }
        public override int Weight { get; set; }
        public override DateTime LastUpdate { get; set; }

        public ConditionsCondition()
        {
        }
        public ConditionsCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete, ActionEnum action, int weight)
        {
            Type = ConditionType.ConditionsCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>();
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            TimeToComplete = timeToComplete;
            Action = action;
            Weight = weight;

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

            if (_conditions == null)
                Init();

            var conditions = campaignContext.Conditions.Where(t => _conditions.Contains(t.ConditionId)).ToList();
            if (conditions.Count != _conditions.Count)
                return ConditionStatus.NotMet;
            
            var passed = _allowExpired 
                    ? conditions.All(conditionState => conditionState.Status != ConditionStatus.NotMet)
                    : conditions.All(conditionState => conditionState.Status == ConditionStatus.Met);
            if (passed)
            {
                foreach (var reward in Rewards)
                {
                    await reward.ExecuteReward(context, publisher);
                }
                return ConditionStatus.Met;
            }
            
            return ConditionStatus.NotMet;
        }

        public override Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient) => Task.FromResult(paramsJson);

        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { ConditionsParam, typeof(string).ToString() },
            { AllowExpiredParam, typeof(bool).ToString() },
        };

        private void Init()
        {
            if (!Parameters.TryGetValue(ConditionsParam, out var conditionsString))
            {
                throw new Exception("Invalid arguments");
            }

            try
            {
                _conditions = conditionsString.Split(';').Select(t=>t.Trim()).ToList();
            }
            catch
            {
                throw new Exception("Invalid arguments");
            }

            if (!_conditions.Any())
            {
                throw new Exception("Invalid arguments");
            }

            if (!Parameters.TryGetValue(AllowExpiredParam, out var kycStatus) && !bool.TryParse(kycStatus, out _allowExpired))
            {
                _allowExpired = false;
            }
        }
    }
}