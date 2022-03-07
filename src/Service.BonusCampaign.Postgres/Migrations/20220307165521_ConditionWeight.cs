using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class ConditionWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Weight",
                schema: "bonuscampaign",
                table: "conditions",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                schema: "bonuscampaign",
                table: "conditions");
        }
    }
}
