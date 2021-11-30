using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.NoSql;

namespace Service.BonusCampaign.Domain
{
    public class CampaignClientContextCacheManager
    {
        private readonly IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> _writer;

        public CampaignClientContextCacheManager(IMyNoSqlServerDataWriter<CampaignClientContextNoSqlEntity> writer)
        {
            _writer = writer;
        }

        public async Task UpdateContext(List<CampaignClientContext> contexts)
        {
            await _writer.BulkInsertOrReplaceAsync(contexts.Select(CampaignClientContextNoSqlEntity.Create));
            await _writer.CleanAndKeepMaxPartitions(10000);
        }
        
        public async Task UpdateContext(CampaignClientContext context)
        {
            await _writer.InsertOrReplaceAsync(CampaignClientContextNoSqlEntity.Create(context));
            await _writer.CleanAndKeepMaxPartitions(10000);
        }
    }
}