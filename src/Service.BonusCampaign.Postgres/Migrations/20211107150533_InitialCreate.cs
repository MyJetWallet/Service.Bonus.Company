using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bonuscampaign");

            migrationBuilder.CreateTable(
                name: "campaigns",
                schema: "bonuscampaign",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FromDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ToDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BannerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "conditions",
                schema: "bonuscampaign",
                columns: table => new
                {
                    ConditionId = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Parameters = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CampaignId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conditions", x => x.ConditionId);
                    table.ForeignKey(
                        name: "FK_conditions_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "bonuscampaign",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conditions_campaigns_CampaignId1",
                        column: x => x.CampaignId1,
                        principalSchema: "bonuscampaign",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "criteria",
                schema: "bonuscampaign",
                columns: table => new
                {
                    CriteriaId = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "text", nullable: true),
                    CriteriaType = table.Column<int>(type: "integer", nullable: false),
                    Parameters = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_criteria", x => x.CriteriaId);
                    table.ForeignKey(
                        name: "FK_criteria_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "bonuscampaign",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rewards",
                schema: "bonuscampaign",
                columns: table => new
                {
                    RewardId = table.Column<string>(type: "text", nullable: false),
                    Parameters = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    ConditionId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rewards", x => x.RewardId);
                    table.ForeignKey(
                        name: "FK_rewards_conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalSchema: "bonuscampaign",
                        principalTable: "conditions",
                        principalColumn: "ConditionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_FromDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                column: "FromDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_IsEnabled",
                schema: "bonuscampaign",
                table: "campaigns",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_ToDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                column: "ToDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_conditions_CampaignId",
                schema: "bonuscampaign",
                table: "conditions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_conditions_CampaignId1",
                schema: "bonuscampaign",
                table: "conditions",
                column: "CampaignId1");

            migrationBuilder.CreateIndex(
                name: "IX_criteria_CampaignId",
                schema: "bonuscampaign",
                table: "criteria",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_rewards_ConditionId",
                schema: "bonuscampaign",
                table: "rewards",
                column: "ConditionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "criteria",
                schema: "bonuscampaign");

            migrationBuilder.DropTable(
                name: "rewards",
                schema: "bonuscampaign");

            migrationBuilder.DropTable(
                name: "conditions",
                schema: "bonuscampaign");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "bonuscampaign");
        }
    }
}
