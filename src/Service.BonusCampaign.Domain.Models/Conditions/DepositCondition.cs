using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;
using Service.BonusRewards.Domain.Models;
using Service.IndexPrices.Client;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class DepositCondition : ConditionBase
    {
        private const string DepositParam = "DepositMade";
        private readonly bool _depositMade;
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.DepositCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }

        public DepositCondition()
        {
        }
        public DepositCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId)
        {
            Type = ConditionType.DepositCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.DepositMade };
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            
            if (!parameters.TryGetValue(DepositParam, out var depositMade) && !bool.TryParse(depositMade, out _depositMade))
            {
                throw new Exception("Invalid arguments");
            }
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task<bool> Check(ContextUpdate context, IServiceBusPublisher<ExecuteRewardMessage> publisher, string paramsJson)
        {
            if (context.DepositEvent != null && context.DepositEvent.Amount > 0)
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
            { DepositParam, typeof(bool).ToString() },
        };
    }
}