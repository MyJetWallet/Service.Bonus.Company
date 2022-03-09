using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class LastUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "rewards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "criteria",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "conditions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "rewards");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "criteria");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "bonuscampaign",
                table: "conditions");
        }
    }
}
