using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class CampaignName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "bonuscampaign",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "Weight",
                schema: "bonuscampaign",
                table: "campaigns");
        }
    }
}
