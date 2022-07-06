using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;

namespace Service.BonusCampaign.Domain
{
    public class AccessCriteriaFactory
    {
        public static AccessCriteriaBase CreateCriteria(CriteriaType type, Dictionary<string, string> parameters, string criteriaId, string campaignId)
        {
            switch (type)
            {
                case CriteriaType.KycType:
                    return new KycCriteria(parameters, criteriaId, campaignId);
                case CriteriaType.ReferralType:
                    return new ReferralCriteria(parameters, criteriaId, campaignId);
                case CriteriaType.RegistrationType:
                    return new RegistrationCriteria(parameters, criteriaId, campaignId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static Dictionary<string, string> GetParams(CriteriaType type)
        {
            return type switch
            {
                CriteriaType.KycType => KycCriteria.ParamDictionary,
                CriteriaType.ReferralType => ReferralCriteria.ParamDictionary,
                CriteriaType.RegistrationType => RegistrationCriteria.ParamDictionary,
                _ => new Dictionary<string, string>()
            };
        }
    }
}