using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Service.BonusCampaign.Domain.Models;
using Service.BonusCampaign.Domain.Models.Context;

#nullable disable

namespace Service.BonusCampaign.Postgres.Migrations
{
    public partial class version_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<Dictionary<string, CampaignClientContext>>(
                name: "CampaignClientContexts",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignClientContexts",
                schema: "bonuscampaign",
                table: "campaigns");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ToDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FromDateTime",
                schema: "bonuscampaign",
                table: "campaigns",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
