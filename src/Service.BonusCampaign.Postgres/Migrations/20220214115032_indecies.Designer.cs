﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.BonusCampaign.Postgres;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220214115032_indecies")]
    partial class indecies
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("bonuscampaign")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Campaign", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("Action")
                        .HasColumnType("integer");

                    b.Property<string>("CampaignType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("None");

                    b.Property<string>("DescriptionTemplateId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FromDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("SerializedRequest")
                        .HasColumnType("text");

                    b.Property<bool>("ShowReferrerStats")
                        .HasColumnType("boolean");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("TitleTemplateId")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<DateTime>("ToDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FromDateTime");

                    b.HasIndex("IsEnabled");

                    b.HasIndex("ToDateTime");

                    b.ToTable("campaigns", "bonuscampaign");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase", b =>
                {
                    b.Property<string>("ConditionId")
                        .HasColumnType("text");

                    b.Property<int>("Action")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("CampaignId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<Dictionary<string, string>>("Parameters")
                        .HasColumnType("jsonb");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<TimeSpan>("TimeToComplete")
                        .HasColumnType("interval");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ConditionId");

                    b.HasIndex("Type");

                    b.HasIndex("CampaignId", "Type");

                    b.ToTable("conditions", "bonuscampaign");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.CampaignClientContext", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("CampaignId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("ActivationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ClientId", "CampaignId");

                    b.HasIndex("CampaignId");

                    b.HasIndex("ClientId");

                    b.ToTable("campaigncontexts", "bonuscampaign");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.ClientConditionState", b =>
                {
                    b.Property<string>("ClientId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ConditionId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("CampaignId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("ExpirationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("Params")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ClientId", "ConditionId", "CampaignId");

                    b.HasIndex("ClientId", "CampaignId");

                    b.ToTable("conditionstates", "bonuscampaign");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase", b =>
                {
                    b.Property<string>("CriteriaId")
                        .HasColumnType("text");

                    b.Property<string>("CampaignId")
                        .HasColumnType("text");

                    b.Property<int>("CriteriaType")
                        .HasColumnType("integer");

                    b.Property<Dictionary<string, string>>("Parameters")
                        .HasColumnType("jsonb");

                    b.HasKey("CriteriaId");

                    b.HasIndex("CampaignId");

                    b.ToTable("criteria", "bonuscampaign");

                    b.HasDiscriminator<int>("CriteriaType");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Rewards.RewardBase", b =>
                {
                    b.Property<string>("RewardId")
                        .HasColumnType("text");

                    b.Property<string>("ConditionId")
                        .HasColumnType("text");

                    b.Property<Dictionary<string, string>>("Parameters")
                        .HasColumnType("jsonb");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("RewardId");

                    b.HasIndex("ConditionId");

                    b.ToTable("rewards", "bonuscampaign");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.ConditionsCondition", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase");

                    b.HasDiscriminator().HasValue(5);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.DepositCondition", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase");

                    b.HasDiscriminator().HasValue(3);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.KycCondition", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.TradeCondition", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.KycCriteria", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.ReferralCriteria", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.RegistrationCriteria", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Rewards.ClientPaymentReward", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Rewards.RewardBase");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Rewards.FeeShareReward", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Rewards.RewardBase");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Rewards.ReferralPaymentReward", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Rewards.RewardBase");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase", b =>
                {
                    b.HasOne("Service.BonusCampaign.Domain.Models.Campaign", null)
                        .WithMany("Conditions")
                        .HasForeignKey("CampaignId");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.CampaignClientContext", b =>
                {
                    b.HasOne("Service.BonusCampaign.Domain.Models.Campaign", null)
                        .WithMany("CampaignClientContexts")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.ClientConditionState", b =>
                {
                    b.HasOne("Service.BonusCampaign.Domain.Models.Context.CampaignClientContext", null)
                        .WithMany("Conditions")
                        .HasForeignKey("ClientId", "CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase", b =>
                {
                    b.HasOne("Service.BonusCampaign.Domain.Models.Campaign", null)
                        .WithMany("CriteriaList")
                        .HasForeignKey("CampaignId");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Rewards.RewardBase", b =>
                {
                    b.HasOne("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase", null)
                        .WithMany("Rewards")
                        .HasForeignKey("ConditionId");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Campaign", b =>
                {
                    b.Navigation("CampaignClientContexts");

                    b.Navigation("Conditions");

                    b.Navigation("CriteriaList");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase", b =>
                {
                    b.Navigation("Rewards");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.CampaignClientContext", b =>
                {
                    b.Navigation("Conditions");
                });
#pragma warning restore 612, 618
        }
    }
}
