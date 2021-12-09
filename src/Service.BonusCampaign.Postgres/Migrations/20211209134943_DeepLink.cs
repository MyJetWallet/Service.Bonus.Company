using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class DeepLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DynamicLink",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DynamicLink",
                schema: "bonuscampaign",
                table: "campaigns");
        }
    }
}
