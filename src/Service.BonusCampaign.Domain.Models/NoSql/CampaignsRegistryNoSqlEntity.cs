using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.BonusCampaign.Domain.Models.NoSql
{
    public class CampaignsRegistryNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-campaigns-registry";

        public static string GeneratePartitionKey() => "CampaignsRegistry";
        public static string GenerateRowKey(string clientId) => clientId;

        public List<string> ActiveCampaigns { get; set; }

        public static CampaignsRegistryNoSqlEntity Create(string clientId, List<string> campaigns)
        {
            return new CampaignsRegistryNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(clientId),
                ActiveCampaigns = campaigns
            };
        }
    }
}