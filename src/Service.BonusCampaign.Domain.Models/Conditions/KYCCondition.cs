using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Conditions
{
    public class KycCondition : ConditionBase
    {
        private const string KycParam = "KYCPassed";
        private readonly bool _kycStatus;
        public override string ConditionId { get; set; }
        public override string CampaignId { get; set; }
        public override ConditionType Type { get; set; } = ConditionType.KYCCondition;
        public override Dictionary<string, string> Parameters { get; set; }
        public override List<RewardBase> Rewards { get; set; }
        public override ConditionStatus Status { get; set; }

        public KycCondition()
        {
        }
        public KycCondition(string campaignId, Dictionary<string, string> parameters, List<RewardBase> rewards, string conditionId)
        {
            Type = ConditionType.KYCCondition;
            ConditionId = conditionId ?? Guid.NewGuid().ToString("N");
            
            CampaignId = campaignId;
            EventTypes = new List<EventType>() { EventType.KYCPassed };
            Status = ConditionStatus.NotMet;
            Parameters = parameters;
            Rewards = rewards;
            
            if (!parameters.TryGetValue(KycParam, out var kycStatus) && !bool.TryParse(kycStatus, out _kycStatus))
            {
                throw new Exception("Invalid arguments");
            }
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        public override async Task<bool> Check() => true;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { KycParam, typeof(bool).ToString() },
        };
    }
}