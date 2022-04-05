using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.Context;

namespace Service.BonusCampaign.Domain.Models.NoSql
{
    public class ReferralPageInfoNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-referral-pageinfo";
        public static string GeneratePartitionKey() => "ReferralPageInfo";
        public static string GenerateRowKey() => "ReferralPageInfo";
        public List<string> ReferralTerms { get; set; }
        public string ReferralLink { get; set; }
        public string Title { get; set; }
        public string DescriptionLink { get; set; }
        public string ReferralDescLink { get; set; }

        public static ReferralPageInfoNoSqlEntity Create(string title, string descriptionLink, string referralLinkBase, string referralDescLink, List<string> referralTerms)
        {
            return new ReferralPageInfoNoSqlEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                ReferralTerms = referralTerms ?? new List<string>(),
                ReferralLink = referralLinkBase,
                Title = title,
                DescriptionLink = descriptionLink,
                ReferralDescLink = referralDescLink,
            };
        }
    }
}