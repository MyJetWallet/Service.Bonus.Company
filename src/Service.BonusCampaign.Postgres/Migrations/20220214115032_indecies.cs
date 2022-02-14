using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class indecies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conditions_CampaignId",
                schema: "bonuscampaign",
                table: "conditions");

            migrationBuilder.CreateIndex(
                name: "IX_conditions_CampaignId_Type",
                schema: "bonuscampaign",
                table: "conditions",
                columns: new[] { "CampaignId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_conditions_Type",
                schema: "bonuscampaign",
                table: "conditions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_campaigncontexts_ClientId",
                schema: "bonuscampaign",
                table: "campaigncontexts",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conditions_CampaignId_Type",
                schema: "bonuscampaign",
                table: "conditions");

            migrationBuilder.DropIndex(
                name: "IX_conditions_Type",
                schema: "bonuscampaign",
                table: "conditions");

            migrationBuilder.DropIndex(
                name: "IX_campaigncontexts_ClientId",
                schema: "bonuscampaign",
                table: "campaigncontexts");

            migrationBuilder.CreateIndex(
                name: "IX_conditions_CampaignId",
                schema: "bonuscampaign",
                table: "conditions",
                column: "CampaignId");
        }
    }
}
