using MyNoSqlServer.Abstractions;

namespace Service.BonusCampaign.Domain.Models.NoSql
{
    public class CampaignNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-bonus-campaign";

        public static string GeneratePartitionKey() => "Campaign";
        public static string GenerateRowKey(string campaignId) => campaignId;

        public Campaign Campaign { get; set; }

        public static CampaignNoSqlEntity Create(Campaign campaign)
        {
            return new CampaignNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(campaign.Id),
                Campaign = campaign
            };
        }
    }
}