using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Service.BonusCampaign.Domain.Models.Context;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class version_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignClientContexts",
                schema: "bonuscampaign",
                table: "campaigns");

            migrationBuilder.CreateTable(
                name: "campaigncontexts",
                schema: "bonuscampaign",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ActivationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigncontexts", x => new { x.ClientId, x.CampaignId });
                    table.ForeignKey(
                        name: "FK_campaigncontexts_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "bonuscampaign",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conditionstates",
                schema: "bonuscampaign",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ConditionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Params = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conditionstates", x => new { x.ClientId, x.ConditionId });
                    table.ForeignKey(
                        name: "FK_conditionstates_campaigncontexts_ClientId_ConditionId",
                        columns: x => new { x.ClientId, x.ConditionId },
                        principalSchema: "bonuscampaign",
                        principalTable: "campaigncontexts",
                        principalColumns: new[] { "ClientId", "CampaignId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaigncontexts_CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conditionstates",
                schema: "bonuscampaign");

            migrationBuilder.DropTable(
                name: "campaigncontexts",
                schema: "bonuscampaign");

            migrationBuilder.AddColumn<Dictionary<string, CampaignClientContext>>(
                name: "CampaignClientContexts",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "jsonb",
                nullable: true);
        }
    }
}
