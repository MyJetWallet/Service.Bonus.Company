using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public class ReferralCriteria : AccessCriteriaBase
    {
        private const string ReferralParam = "HasReferral";
        private readonly bool _hasReferral;
        public override string CriteriaId { get; set; }
        public override string CampaignId { get; set; }
        public override CriteriaType CriteriaType { get; set; }
        public override Dictionary<string, string> Parameters { get; set; }

        public ReferralCriteria(Dictionary<string, string> parameters, string criteriaId, string campaignId) : base(parameters)
        {
            CriteriaId = criteriaId ?? Guid.NewGuid().ToString("N");
            CampaignId = campaignId;
            CriteriaType = CriteriaType.KycType;
            Parameters = parameters;
            if (!parameters.TryGetValue(ReferralParam, out var hasReferral) && !bool.TryParse(hasReferral, out _hasReferral))
            {
                throw new Exception("Invalid arguments");
            }
        }


        public override Task<bool> Check(ClientContext context)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(context.ReferrerClientId) != _hasReferral);
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { ReferralParam, typeof(bool).ToString() },
        };
    }
}