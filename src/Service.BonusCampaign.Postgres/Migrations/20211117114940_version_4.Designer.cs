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
    [Migration("20211117114940_version_4")]
    partial class version_4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("bonuscampaign")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Campaign", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BannerId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("FromDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ToDateTime")
                        .HasColumnType("timestamp with time zone");

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

                    b.Property<string>("CampaignId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<Dictionary<string, string>>("Parameters")
                        .HasColumnType("jsonb");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ConditionId");

                    b.HasIndex("CampaignId");

                    b.ToTable("conditions", "bonuscampaign");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Context.CampaignClientContext", b =>
                {
                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<string>("CampaignId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("ActivationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ClientId", "CampaignId");

                    b.HasIndex("CampaignId");

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

                    b.Property<string>("Params")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ClientId", "ConditionId");

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

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Conditions.KycCondition", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Conditions.ConditionBase");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Service.BonusCampaign.Domain.Models.Criteria.KycCriteria", b =>
                {
                    b.HasBaseType("Service.BonusCampaign.Domain.Models.Criteria.AccessCriteriaBase");

                    b.HasDiscriminator().HasValue(1);
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
                        .HasForeignKey("ClientId", "ConditionId")
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
