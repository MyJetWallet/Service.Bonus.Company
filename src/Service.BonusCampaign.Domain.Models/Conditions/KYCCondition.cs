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
    public class KycCondition : ConditionBase
    {
        private const string KycDepositParam = "CheckDepositKyc";
        private const string KycTradeParam = "CheckTradeKyc";
        private const string KycWithdrawalParam = "CheckWithdrawalKyc";
        private bool _kycDepositStatus;
        private bool _kycTradeStatus;
        private bool _kycWithdrawalStatus;
        
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.KYCCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }
        public override TimeSpan TimeToComplete { get; set; }
        public override ActionEnum Action { get; set; }
        public override int Weight { get; set; }
        public override DateTime LastUpdate { get; set; }

        public KycCondition()
        {
        }
        public KycCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId, TimeSpan timeToComplete, ActionEnum action, int weight)
        {
            Type = ConditionType.KYCCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.KYCPassed };
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

            Init();

            if (context.KycEvent == null) 
                return ConditionStatus.NotMet;
            
            if(_kycDepositStatus && !context.KycEvent.KycDepositPassed)
                return ConditionStatus.NotMet;

            if(_kycTradeStatus && !context.KycEvent.KycTradePassed)
                return ConditionStatus.NotMet;
            
            if(_kycWithdrawalStatus && !context.KycEvent.KycWithdrawalPassed)
                return ConditionStatus.NotMet;
                
            foreach (var reward in Rewards)
            {
                await reward.ExecuteReward(context, publisher);
            }
            return ConditionStatus.Met;

        }

        private void Init()
        {
            if (!Parameters.TryGetValue(KycDepositParam, out var deposit)
                && !Parameters.TryGetValue(KycTradeParam, out var trade)
                && !Parameters.TryGetValue(KycWithdrawalParam, out var withdrawal)
                && !bool.TryParse(deposit, out _kycDepositStatus)
                && !bool.TryParse(trade, out _kycTradeStatus)
                && !bool.TryParse(withdrawal, out _kycWithdrawalStatus))
                throw new Exception("Invalid arguments");
        }
        
        public override Task<string> UpdateConditionStateParams(ContextUpdate context, string paramsJson, IConvertIndexPricesClient pricesClient) => Task.FromResult(paramsJson);

        public static readonly Dictionary<string, string> ParamDictionary = new ()
        {
            { KycDepositParam, typeof(bool).ToString() },
            { KycTradeParam, typeof(bool).ToString() },
            { KycWithdrawalParam, typeof(bool).ToString() }
        };
    }
}