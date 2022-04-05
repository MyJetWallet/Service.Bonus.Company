namespace Service.BonusCampaign.Domain.Models.Context.ParamsModels
{
    public class DepositParamsModel
    {
        public decimal DepositedAmount { get; set; }
        public decimal RequiredAmount { get; set; }
        public string DepositAsset { get; set; }
    }
}