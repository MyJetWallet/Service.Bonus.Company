using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class version_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaigncontexts_campaigns_CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.DropForeignKey(
                name: "FK_conditionstates_campaigncontexts_CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conditionstates",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropIndex(
                name: "IX_conditionstates_CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_campaigncontexts",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.DropColumn(
                name: "CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropColumn(
                name: "CampaignContextId",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.AddColumn<string>(
                name: "CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates",
                type: "character varying(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_conditionstates",
                schema: "bonuscampaign",
                table: "conditionstates",
                columns: new[] { "ClientId", "ConditionId", "CampaignId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_campaigncontexts",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                columns: new[] { "ClientId", "CampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_conditionstates_ClientId_CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates",
                columns: new[] { "ClientId", "CampaignId" });

            migrationBuilder.AddForeignKey(
                name: "FK_campaigncontexts_campaigns_CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                column: "CampaignId",
                principalSchema: "bonuscampaign",
                principalTable: "campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_conditionstates_campaigncontexts_ClientId_CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates",
                columns: new[] { "ClientId", "CampaignId" },
                principalSchema: "bonuscampaign",
                principalTable: "campaigncontexts",
                principalColumns: new[] { "ClientId", "CampaignId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaigncontexts_campaigns_CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.DropForeignKey(
                name: "FK_conditionstates_campaigncontexts_ClientId_CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conditionstates",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropIndex(
                name: "IX_conditionstates_ClientId_CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_campaigncontexts",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                schema: "bonuscampaign",
                table: "conditionstates");

            migrationBuilder.AddColumn<int>(
                name: "CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "CampaignContextId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_conditionstates",
                schema: "bonuscampaign",
                table: "conditionstates",
                columns: new[] { "ClientId", "ConditionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_campaigncontexts",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                column: "CampaignContextId");

            migrationBuilder.CreateIndex(
                name: "IX_conditionstates_CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates",
                column: "CampaignContextId");

            migrationBuilder.AddForeignKey(
                name: "FK_campaigncontexts_campaigns_CampaignId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                column: "CampaignId",
                principalSchema: "bonuscampaign",
                principalTable: "campaigns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_conditionstates_campaigncontexts_CampaignContextId",
                schema: "bonuscampaign",
                table: "conditionstates",
                column: "CampaignContextId",
                principalSchema: "bonuscampaign",
                principalTable: "campaigncontexts",
                principalColumn: "CampaignContextId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
