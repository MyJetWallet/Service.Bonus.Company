using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "bonuscampaign";

        public const string CampaignsTableName = "campaigns";
        public const string CriteriaTableName = "criteria";
        public const string ConditionTableName = "conditions";
        public const string RewardTableName = "rewards";

        public DbSet<Campaign> Campaigns { get; set; }
        
        public DbSet<AccessCriteriaBase> Criteria { get; set; }
        public DbSet<KycCriteria> KycCriteria { get; set; }
        
        public DbSet<ConditionBase> Conditions { get; set; }
        public DbSet<KycCondition> KycConditions { get; set; }
        
        public DbSet<RewardBase> Rewards { get; set; }
        public DbSet<FeeShareReward> FeeShareRewards { get; set; }
        public DbSet<ClientPaymentReward> ClientPaymentRewards { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public static ILoggerFactory LoggerFactory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.HasDefaultSchema(Schema);
            SetCampaigns(modelBuilder);
            SetConditions(modelBuilder);
            SetCriteria(modelBuilder);
            SetRewards(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SetCampaigns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campaign>().ToTable(CampaignsTableName);
            modelBuilder.Entity<Campaign>().HasKey(e => e.Id);
            modelBuilder.Entity<Campaign>().Property(e => e.Name).HasMaxLength(2048);
            modelBuilder.Entity<Campaign>().Property(e => e.Status);
            modelBuilder.Entity<Campaign>().Property(e => e.IsEnabled);
            modelBuilder.Entity<Campaign>().Property(e => e.BannerId).HasMaxLength(128);
            modelBuilder.Entity<Campaign>().Property(e => e.FromDateTime);
            modelBuilder.Entity<Campaign>().Property(e => e.ToDateTime);
            
            modelBuilder.Entity<Campaign>().HasIndex(e => e.IsEnabled);
            modelBuilder.Entity<Campaign>().HasIndex(e => e.FromDateTime);
            modelBuilder.Entity<Campaign>().HasIndex(e => e.ToDateTime);
        }
        
        private void SetCriteria(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessCriteriaBase>().ToTable(CriteriaTableName);
            modelBuilder.Entity<AccessCriteriaBase>().HasKey(e => e.CriteriaId);
            modelBuilder.Entity<AccessCriteriaBase>().Property(e => e.Parameters).HasColumnType("jsonb");
            modelBuilder.Entity<AccessCriteriaBase>().HasDiscriminator(e => e.CriteriaType)
                .HasValue<KycCriteria>(CriteriaType.KycType);
            modelBuilder.Entity<KycCriteria>().HasBaseType<AccessCriteriaBase>();

            modelBuilder.Entity<AccessCriteriaBase>().HasOne<Campaign>().WithMany(t => t.CriteriaList).HasForeignKey(t=>t.CampaignId);
            
            // modelBuilder.Entity<AccessCriteriaBase>().HasIndex(e => e.IsEnabled);
            // modelBuilder.Entity<AccessCriteriaBase>().HasIndex(e => e.FromDateTime);
            // modelBuilder.Entity<AccessCriteriaBase>().HasIndex(e => e.ToDateTime);
        }
        
        private void SetConditions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConditionBase>().ToTable(ConditionTableName);
            modelBuilder.Entity<ConditionBase>().HasKey(e => e.ConditionId);
            modelBuilder.Entity<ConditionBase>().Property(e => e.CampaignId).HasMaxLength(128);
            modelBuilder.Entity<ConditionBase>().Property(e => e.Parameters).HasColumnType("jsonb");

            modelBuilder.Entity<ConditionBase>().HasDiscriminator(e => e.Type)
                .HasValue<KycCondition>(ConditionType.KYCCondition);
            modelBuilder.Entity<KycCondition>().HasBaseType<ConditionBase>();
            
            modelBuilder.Entity<ConditionBase>().HasOne<Campaign>().WithMany(t => t.Conditions).HasForeignKey(t=>t.CampaignId);

        }
        
        private void SetRewards(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RewardBase>().ToTable(RewardTableName);
            modelBuilder.Entity<RewardBase>().HasKey(e => e.RewardId);
            modelBuilder.Entity<RewardBase>().Property(e => e.Parameters).HasColumnType("jsonb");

            modelBuilder.Entity<RewardBase>().HasDiscriminator(e => e.Type)
                .HasValue<FeeShareReward>(RewardType.FeeShareAssignment)
                .HasValue<ClientPaymentReward>(RewardType.ClientPaymentAbsolute);
            
            modelBuilder.Entity<FeeShareReward>().HasBaseType<RewardBase>();
            modelBuilder.Entity<ClientPaymentReward>().HasBaseType<RewardBase>();

            
            modelBuilder.Entity<RewardBase>().HasOne<ConditionBase>().WithMany(t => t.Rewards).HasForeignKey(t=>t.ConditionId);


        }

        public async Task<int> UpsertAsync(IEnumerable<Campaign> entities)
        {
            var result = await Campaigns.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        public async Task<int> UpsertAsync(IEnumerable<AccessCriteriaBase> entities)
        {
            var result = await Criteria.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }        
        public async Task<int> UpsertAsync(IEnumerable<ConditionBase> entities)
        {
            var result = await Conditions.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }        
        public async Task<int> UpsertAsync(IEnumerable<RewardBase> entities)
        {
            var result = await Rewards.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
    }
}
