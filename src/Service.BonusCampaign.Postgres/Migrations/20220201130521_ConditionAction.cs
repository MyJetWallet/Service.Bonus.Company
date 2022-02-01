using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class ConditionAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "bonuscampaign",
                table: "conditions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                schema: "bonuscampaign",
                table: "conditions");
        }
    }
}
