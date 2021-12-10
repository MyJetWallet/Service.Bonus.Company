using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class DeepLinkActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DynamicLink",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "SerializedRequest");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                schema: "bonuscampaign",
                table: "campaigns");

            migrationBuilder.RenameColumn(
                name: "SerializedRequest",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "DynamicLink");
        }
    }
}
