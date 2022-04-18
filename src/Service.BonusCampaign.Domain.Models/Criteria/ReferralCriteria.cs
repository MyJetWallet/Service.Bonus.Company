using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public class ReferralCriteria : AccessCriteriaBase
    {
        public const string ReferrerParam = "HasReferrer";
        private bool _hasReferrer;
        public override string CriteriaId { get; set; }
        public override string CampaignId { get; set; }
        public override CriteriaType CriteriaType { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override Dictionary<string, string> Parameters { get; set; }

        public ReferralCriteria(Dictionary<string, string> parameters, string criteriaId, string campaignId) : base(parameters)
        {
            CriteriaId = criteriaId ?? Guid.NewGuid().ToString("N");
            CampaignId = campaignId;
            CriteriaType = CriteriaType.ReferralType;
            Parameters = parameters;
            LastUpdate = DateTime.UtcNow;
            Init();
        }


        public override Task<bool> Check(ClientContext context)
        {
            Init();
            return Task.FromResult(string.IsNullOrWhiteSpace(context.ReferrerClientId) != _hasReferrer);
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { ReferrerParam, typeof(bool).ToString() },
        };

        private void Init()
        {
            if (!Parameters.TryGetValue(ReferrerParam, out var hasReferral) || !bool.TryParse(hasReferral.ToLower(), out _hasReferrer))
            {
                throw new Exception("Invalid arguments");
            }
        }
    }
}