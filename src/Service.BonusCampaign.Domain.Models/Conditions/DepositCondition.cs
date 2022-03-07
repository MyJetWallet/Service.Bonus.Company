using System;
using System.Collections.Generic;
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
    public class DepositCondition : ConditionBase
    {
        private const string DepositParam = "DepositMade";
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.DepositCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }
        public override ActionEnum Action { get; set; }
        public override int Weight { get; set; }

        public DepositCondition()
        {
        }
        public DepositCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete, ActionEnum action)
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

        }

        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task<ConditionStatus> Check(ContextUpdate context,
            IServiceBusPublisher<ExecuteRewardMessage> publisher, string paramsJson,
            CampaignClientContext campaignContext)
        {
            if (IsExpired(campaignContext.ActivationTime))
                return ConditionStatus.Expired;
            
            if (context.DepositEvent != null && context.DepositEvent.Amount > 0)
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
            { DepositParam, typeof(bool).ToString() },
        };
        
    }
}