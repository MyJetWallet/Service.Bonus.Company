using System.Collections.Generic;

namespace Service.BonusCampaign.Domain.Models
{
    public class CampaignBanner
    {
        public string BannerId { get; set; }
        public string TitleTemplateId { get; set; }
        public string DescriptionTemplateId { get; set; }
        public Dictionary<string, string> Params  { get; set; }
    }
}