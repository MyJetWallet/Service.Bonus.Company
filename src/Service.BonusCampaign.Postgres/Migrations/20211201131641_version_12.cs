using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class version_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "TitleTemplateId");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionTemplateId",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionTemplateId",
                schema: "bonuscampaign",
                table: "campaigns");

            migrationBuilder.RenameColumn(
                name: "TitleTemplateId",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "bonuscampaign",
                table: "campaigns",
                newName: "BannerId");
        }
    }
}
