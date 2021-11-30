using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.Context;

namespace Service.BonusCampaign.Domain.Models.NoSql
{
    public class CampaignClientContextNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-campaigns-contexts";

        public static string GeneratePartitionKey(string clientId) => clientId;
        public static string GenerateRowKey(string campaignId) => campaignId;

        public CampaignClientContext Context { get; set; }

        public static CampaignClientContextNoSqlEntity Create(CampaignClientContext context)
        {
            return new CampaignClientContextNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(context.ClientId),
                RowKey = GenerateRowKey(context.CampaignId),
                Context = context
            };
        }
    }
}