using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Postgres;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusCampaign.Worker.Jobs
{
    public class ConditionCheckerJob
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public ConditionCheckerJob(ISubscriber<ContextUpdate> subscriber, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            subscriber.Subscribe(HandleUpdates);
        }

        private async ValueTask HandleUpdates(ContextUpdate update)
        {
            switch (update.EventType)
            {
                case EventType.ClientRegistered:
                    break;
                case EventType.KYCPassed:
                    await HandleKycUpdates(update);
                    break;
                case EventType.ReferrerAdded:
                    break;
                case EventType.DepositMade:
                    break;
                case EventType.TradeMade:
                    break;
                case EventType.WithdrawalMade:
                    break;
                default:
                    break;
            }
        }

        private async Task HandleKycUpdates(ContextUpdate update)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var campaigns = await ctx.Campaigns
                .Include(campaign => campaign.Conditions)
                .Where(campaign => campaign.Conditions.Any(condition => condition.Type == ConditionType.KYCCondition))
                .ToListAsync();

            foreach (var campaign in campaigns)
            {
                if (campaign.CampaignClientContexts.TryGetValue(update.ClientId, out var context))
                {
                    var conditions = campaign.Conditions.Where(t => t.Type == ConditionType.KYCCondition).ToList();
                    foreach (var condition in conditions.Where(condition => update.KycEvent.KycPassed))
                    {
                        context.Conditions[condition.ConditionId] = ConditionStatus.Met;
                    }
                }
            }

            await ctx.UpsertAsync(campaigns);
        }
        
    }
}