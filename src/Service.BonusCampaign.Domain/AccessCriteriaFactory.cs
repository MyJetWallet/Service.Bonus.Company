using System;
using System.Collections.Generic;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Criteria;

namespace Service.BonusCampaign.Domain
{
    public class AccessCriteriaFactory
    {
        public static AccessCriteriaBase CreateCriteria(CriteriaType type, Dictionary<string, string> parameters)
        {
            switch (type)
            {
                case CriteriaType.KycType:
                    return new KycCriteria(parameters);
                case CriteriaType.RegistrationType:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static Dictionary<string, string> GetParams(CriteriaType type)
        {
            switch (type)
            {
                case CriteriaType.KycType:
                    return KycCriteria.ParamDictionary;
                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}