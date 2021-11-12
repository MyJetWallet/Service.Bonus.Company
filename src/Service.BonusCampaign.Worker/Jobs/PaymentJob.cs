using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Rewards;
using Service.BonusCampaign.Postgres;

namespace Service.BonusCampaign.Worker.Jobs
{
    public class PaymentJob : IStartable, IDisposable
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly MyTaskTimer _timer;
        private readonly ILogger<PaymentJob> _logger;

        public PaymentJob(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<PaymentJob> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            
            _timer = new MyTaskTimer(typeof(PaymentJob),
                TimeSpan.FromSeconds(60),
                logger, DoTime);
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task DoTime()
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var conditions = await ctx.Conditions.ToDictionaryAsync(condition=>condition.ConditionId, condition=>condition);

            var campaigns = await ctx.Campaigns.ToListAsync();
            var contexts = campaigns.SelectMany(t => t.CampaignClientContexts.Values).ToList();
            foreach (var context in contexts)
            {
                foreach (var (conditionId, status) in context.Conditions)
                {
                    if (status == ConditionStatus.Met)
                    {
                        var condition = conditions[conditionId];
                        await HandleRewards(condition);
                        context.Conditions[conditionId] = ConditionStatus.RewardsPaid;
                    }
                }
            }
            await ctx.UpsertAsync(campaigns);
        }

        private async Task HandleRewards(ConditionBase condition)
        {
            foreach (var reward in condition.Rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.FeeShareAssignment:
                        break;
                    case RewardType.ReferrerPaymentAbsolute:
                        break;
                    case RewardType.ClientPaymentAbsolute:
                        break;
                    case RewardType.ReferrerPaymentRelative:
                        break;
                    case RewardType.ClientPaymentRelative:
                        break;
                    default:
                        break;
                }
            }
        }
        

        public void Start()
        {
            _timer.Start();
        }
    }
}