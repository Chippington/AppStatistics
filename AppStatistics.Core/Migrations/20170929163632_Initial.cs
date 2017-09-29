using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AppStatistics.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    analyticsEndpoint = table.Column<string>(type: "TEXT", nullable: true),
                    applicationName = table.Column<string>(type: "TEXT", nullable: true),
                    creationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    applicationID = table.Column<string>(type: "TEXT", nullable: true),
                    category = table.Column<string>(type: "TEXT", nullable: true),
                    message = table.Column<string>(type: "TEXT", nullable: true),
                    metadata = table.Column<string>(type: "TEXT", nullable: true),
                    timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Exceptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    SqlExceptionid = table.Column<string>(type: "TEXT", nullable: true),
                    applicationID = table.Column<string>(type: "TEXT", nullable: true),
                    hresult = table.Column<int>(type: "INTEGER", nullable: false),
                    message = table.Column<string>(type: "TEXT", nullable: true),
                    metadata = table.Column<string>(type: "TEXT", nullable: true),
                    severity = table.Column<int>(type: "INTEGER", nullable: false),
                    stackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    timeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exceptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Exceptions_Exceptions_SqlExceptionid",
                        column: x => x.SqlExceptionid,
                        principalTable: "Exceptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exceptions_SqlExceptionid",
                table: "Exceptions",
                column: "SqlExceptionid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Exceptions");
        }
    }
}
