using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Domain.Models.Criteria
{
    public class RegistrationCriteria : AccessCriteriaBase
    {
        private const string CountriesParam = "CountriesList";
        private List<string> _countries;
        public override string CriteriaId { get; set; }
        public override string CampaignId { get; set; }
        public override CriteriaType CriteriaType { get; set; }
        public override DateTime LastUpdate { get; set; }
        public override Dictionary<string, string> Parameters { get; set; }

        public RegistrationCriteria(Dictionary<string, string> parameters, string criteriaId, string campaignId) : base(parameters)
        {
            CriteriaId = criteriaId ?? Guid.NewGuid().ToString("N");
            CampaignId = campaignId;
            CriteriaType = CriteriaType.RegistrationType;
            Parameters = parameters;
            LastUpdate = DateTime.UtcNow;
            Init();
        }


        public override Task<bool> Check(ClientContext context)
        {
            Init();
            if (_countries.All(string.IsNullOrWhiteSpace))
                return Task.FromResult(context.RegistrationDate >= LastUpdate);
            
            if(string.IsNullOrEmpty(context.Country))
                return Task.FromResult(false);

            return Task.FromResult(_countries.Contains(context.Country) && context.RegistrationDate >= LastUpdate);
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { CountriesParam, typeof(string).ToString() },
        };

        private void Init()
        {
            if (!Parameters.TryGetValue(CountriesParam, out var countriesString))
            {
                _countries = new List<string>();
                return;
            }

            try
            {
                _countries = countriesString.Split(';').Select(t=>t.Trim().ToUpper()).ToList();
            }
            catch
            {
                throw new Exception("Invalid arguments");
            }
        }
    }
}