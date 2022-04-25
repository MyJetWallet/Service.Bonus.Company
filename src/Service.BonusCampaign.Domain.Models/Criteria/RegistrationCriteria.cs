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
        public const string CountriesParam = "CountriesList";
        private List<string> _countries;
        public const string DateParam = "DateParam";
        private DateTime _startingDate;
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
            return Task.FromResult(CheckCountry() && CheckDate());

            //locals
            bool CheckCountry()
            {
                if (!_countries.Any())
                    return true;

                if (string.IsNullOrWhiteSpace(context.Country))
                    return true;
                
                return _countries.Contains(context.Country);
            }

            bool CheckDate()
            {
                if (_startingDate == DateTime.MinValue)
                    return true;
                
                if (context.RegistrationDate == DateTime.MinValue)
                    return true;
                
                return context.RegistrationDate >= _startingDate;
            }
        }

        public override Dictionary<string, string> GetParams() => Parameters;
        
        public static readonly Dictionary<string, string> ParamDictionary = new Dictionary<string, string>()
        {
            { CountriesParam, typeof(string).ToString() },
            { DateParam, typeof(DateTime).ToString() },

        };

        private void Init()
        {
            if (Parameters.TryGetValue(CountriesParam, out var countriesString))
            {
                try
                {
                    _countries = countriesString.Split(';').Where(t=>!string.IsNullOrWhiteSpace(t)).Select(t => t.Trim().ToUpper()).ToList();
                }
                catch
                {
                    throw new Exception("Invalid arguments");
                }
            }
            else
            {
                _countries = new List<string>();
            }

            if (Parameters.TryGetValue(DateParam, out var startingDateStr))
            {
                try
                {
                    _startingDate = DateTime.Parse(startingDateStr);
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid arguments");
                }
            }
            else
            {
                _startingDate = DateTime.MinValue;
            }
        }
    }
}