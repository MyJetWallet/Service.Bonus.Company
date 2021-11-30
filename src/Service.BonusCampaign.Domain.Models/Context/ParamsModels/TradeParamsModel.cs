namespace Service.BonusCampaign.Domain.Models.Context.ParamsModels
{
    public class TradeParamsModel
    {
        public decimal TradeAmount { get; set; }
        public decimal RequiredAmount { get; set; }
        public string TradeAsset { get; set; }
    }
}