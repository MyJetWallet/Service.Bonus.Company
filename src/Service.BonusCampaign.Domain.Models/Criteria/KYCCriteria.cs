using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public class KycCriteria : AccessCriteriaBase
    {
        private const string KycParam = "KYCPassed";
        private readonly bool _kycStatus;
        public override string CriteriaId { get; set; }
        public override string CampaignId { get; set; }
        public override CriteriaType CriteriaType { get; set; }
        public override Dictionary<string, string> Parameters { get; set; }

        public KycCriteria(Dictionary<string, string> parameters) : base(parameters)
        {
            CriteriaId = Guid.NewGuid().ToString("N");
            
            CriteriaType = CriteriaType.KycType;
            Parameters = parameters;
            if (!parameters.TryGetValue(KycParam, out var kycStatus) && !bool.TryParse(kycStatus, out _kycStatus))
            {
                throw new Exception("Invalid arguments");
            }
        }


        public override Task<bool> Check(ClientContext context)
        {
            return Task.FromResult(context.KYCDone == _kycStatus);
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { KycParam, typeof(bool).ToString() },
        };
    }
}