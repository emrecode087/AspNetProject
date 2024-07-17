using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectOneMil.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "onemildata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Column1 = table.Column<string>(type: "text", nullable: false),
                    Column2 = table.Column<int>(type: "integer", nullable: false),
                    Column3 = table.Column<decimal>(type: "numeric", nullable: false),
                    Column4 = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Column5 = table.Column<bool>(type: "boolean", nullable: false),
                    Column6 = table.Column<string>(type: "text", nullable: false),
                    Column7 = table.Column<string>(type: "text", nullable: false),
                    Column8 = table.Column<int>(type: "integer", nullable: false),
                    Column9 = table.Column<decimal>(type: "numeric", nullable: false),
                    Column10 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onemildata", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "onemildata");
        }
    }
}
