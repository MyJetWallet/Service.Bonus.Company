using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public class KycCriteria : AccessCriteriaBase
    {
        private const string KycDepositParam = "KycDepositPassed";
        private const string KycTradeParam = "KycTradePassed";
        private const string KycWithdrawalParam = "KycWithdrawalPassed";
        private bool _kycDepositStatus;
        private bool _kycTradeStatus;
        private bool _kycWithdrawalStatus;
        public override string CriteriaId { get; set; }
        public override string CampaignId { get; set; }
        public override CriteriaType CriteriaType { get; set; }
        public override Dictionary<string, string> Parameters { get; set; }

        public KycCriteria(Dictionary<string, string> parameters, string criteriaId, string campaignId) : base(
            parameters)
        {
            CriteriaId = criteriaId ?? Guid.NewGuid().ToString("N");
            CampaignId = campaignId;
            CriteriaType = CriteriaType.KycType;
            Parameters = parameters;
            Init();
        }


        public override Task<bool> Check(ClientContext context)
        {
            Init();
            return Task.FromResult(context.KycDepositAllowed == _kycDepositStatus &&
                                   context.KycTradeAllowed == _kycTradeStatus &&
                                   context.KycWithdrawalAllowed == _kycWithdrawalStatus);
        }

        public override Dictionary<string, string> GetParams()
        {
            return Parameters;
        }

        public static readonly Dictionary<string, string> ParamDictionary = new()
        {
            { KycDepositParam, typeof(bool).ToString() },
            { KycTradeParam, typeof(bool).ToString() },
            { KycWithdrawalParam, typeof(bool).ToString() }
        };

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
    }
}