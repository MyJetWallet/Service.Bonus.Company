using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class version_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_conditions_campaigns_CampaignId1",
                schema: "bonuscampaign",
                table: "conditions");

            migrationBuilder.DropIndex(
                name: "IX_conditions_CampaignId1",
                schema: "bonuscampaign",
                table: "conditions");

            migrationBuilder.DropColumn(
                name: "CampaignId1",
                schema: "bonuscampaign",
                table: "conditions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampaignId1",
                schema: "bonuscampaign",
                table: "conditions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_conditions_CampaignId1",
                schema: "bonuscampaign",
                table: "conditions",
                column: "CampaignId1");

            migrationBuilder.AddForeignKey(
                name: "FK_conditions_campaigns_CampaignId1",
                schema: "bonuscampaign",
                table: "conditions",
                column: "CampaignId1",
                principalSchema: "bonuscampaign",
                principalTable: "campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
