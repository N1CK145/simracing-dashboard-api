using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimRacingDashboard.Api.Migrations
{
    /// <inheritdoc />
    public partial class Tracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    LayoutVersion = table.Column<string>(type: "text", nullable: false),
                    Turns = table.Column<int>(type: "integer", nullable: true),
                    LengthKm = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
