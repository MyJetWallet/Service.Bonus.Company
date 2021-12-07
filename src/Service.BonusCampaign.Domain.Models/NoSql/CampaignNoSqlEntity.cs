using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.GrpcModels;

namespace Service.BonusCampaign.Domain.Models.NoSql
{
    public class CampaignNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-bonus-campaign";

        public static string GeneratePartitionKey() => "Campaign";
        public static string GenerateRowKey(string campaignId) => campaignId;

        public CampaignGrpcModel Campaign { get; set; }

        public static CampaignNoSqlEntity Create(CampaignGrpcModel campaign)
        {
            return new CampaignNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(campaign.Id),
                Campaign = campaign
            };
        }
        
        public static CampaignNoSqlEntity Create(Campaign campaign)
        {
            return new CampaignNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(campaign.Id),
                Campaign = campaign.ToGrpcModel()
            };
        }
    }
}