using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Postgres;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Conditions;
using Service.BonusCampaign.Domain.Models.Context;
using Service.BonusCampaign.Domain.Models.Criteria;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Domain.Models.Rewards;

namespace Service.BonusCampaign.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "bonuscampaign";

        public const string CampaignsTableName = "campaigns";
        public const string CriteriaTableName = "criteria";
        public const string ConditionTableName = "conditions";
        public const string RewardTableName = "rewards";
        public const string CampaignClientContextTableName = "campaigncontexts";
        public const string ConditionStateTableName = "conditionstates";
        public DbSet<Campaign> Campaigns { get; set; }
        
        public DbSet<AccessCriteriaBase> Criteria { get; set; }
        public DbSet<KycCriteria> KycCriteria { get; set; }
        
        public DbSet<ConditionBase> Conditions { get; set; }
        public DbSet<KycCondition> KycConditions { get; set; }
        
        public DbSet<RewardBase> Rewards { get; set; }
        public DbSet<FeeShareReward> FeeShareRewards { get; set; }
        public DbSet<ClientPaymentReward> ClientPaymentRewards { get; set; }

        public DbSet<CampaignClientContext> CampaignClientContexts { get; set; }
        public DbSet<ClientConditionState> ClientConditionStates { get; set; }
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.HasDefaultSchema(Schema);
            SetCampaigns(modelBuilder);
            SetConditions(modelBuilder);
            SetCriteria(modelBuilder);
            SetRewards(modelBuilder);
            SetContexts(modelBuilder);
            SetConditionStates(modelBuilder);
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
            modelBuilder.Entity<Campaign>().Property(e => e.FromDateTime).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<Campaign>().Property(e => e.ToDateTime).HasDefaultValue(DateTime.MinValue);

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
                .HasValue<KycCriteria>(CriteriaType.KycType)
                .HasValue<ReferralCriteria>(CriteriaType.ReferralType);
            
            modelBuilder.Entity<KycCriteria>().HasBaseType<AccessCriteriaBase>();
            modelBuilder.Entity<ReferralCriteria>().HasBaseType<AccessCriteriaBase>();

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
                .HasValue<KycCondition>(ConditionType.KYCCondition)
                .HasValue<TradeCondition>(ConditionType.TradeCondition)
                .HasValue<DepositCondition>(ConditionType.DepositCondition)
                .HasValue<ConditionsCondition>(ConditionType.ConditionsCondition);
            
            modelBuilder.Entity<KycCondition>().HasBaseType<ConditionBase>();
            modelBuilder.Entity<TradeCondition>().HasBaseType<ConditionBase>();
            modelBuilder.Entity<DepositCondition>().HasBaseType<ConditionBase>();
            modelBuilder.Entity<ConditionsCondition>().HasBaseType<ConditionBase>();

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
        
        private void SetContexts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampaignClientContext>().ToTable(CampaignClientContextTableName);
            modelBuilder.Entity<CampaignClientContext>().HasKey(e => new{ e.ClientId, e.CampaignId });
            modelBuilder.Entity<CampaignClientContext>().Property(e => e.CampaignId).HasMaxLength(128);           
            modelBuilder.Entity<CampaignClientContext>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<CampaignClientContext>().Property(e => e.ActivationTime).HasDefaultValue(DateTime.MinValue);

            //modelBuilder.Entity<CampaignClientContext>().Property(e => e.).HasColumnType("jsonb");
            
            modelBuilder.Entity<CampaignClientContext>().HasOne<Campaign>().WithMany(t => t.CampaignClientContexts).HasForeignKey(t=>t.CampaignId);
        }

        private void SetConditionStates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientConditionState>().ToTable(ConditionStateTableName);
            modelBuilder.Entity<ClientConditionState>().HasKey(e => new { e.ClientId, e.ConditionId, e.CampaignId});
            modelBuilder.Entity<ClientConditionState>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<ClientConditionState>().Property(e => e.ConditionId).HasMaxLength(128);
            modelBuilder.Entity<ClientConditionState>().Property(e => e.CampaignId).HasMaxLength(128);
            //modelBuilder.Entity<CampaignClientContext>().Property(e => e.).HasColumnType("jsonb");
            
            modelBuilder.Entity<ClientConditionState>().HasOne<CampaignClientContext>().WithMany(t => t.Conditions).HasForeignKey(t=>new {t.ClientId, t.CampaignId});
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
        
        public async Task<int> UpsertAsync(IEnumerable<CampaignClientContext> entities)
        {
            var result = await CampaignClientContexts.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsertAsync(IEnumerable<ClientConditionState> entities)
        {
            var result = await ClientConditionStates.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
    }
}
