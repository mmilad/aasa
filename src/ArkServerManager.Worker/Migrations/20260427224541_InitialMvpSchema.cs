using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArkServerManager.Worker.Migrations
{
    /// <inheritdoc />
    public partial class InitialMvpSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    state = table.Column<string>(type: "TEXT", nullable: false),
                    install_root = table.Column<string>(type: "TEXT", nullable: false),
                    game_port = table.Column<int>(type: "INTEGER", nullable: false),
                    query_port = table.Column<int>(type: "INTEGER", nullable: false),
                    rcon_port = table.Column<int>(type: "INTEGER", nullable: false),
                    rcon_password = table.Column<string>(type: "TEXT", nullable: false),
                    map_name = table.Column<string>(type: "TEXT", nullable: false),
                    session_name = table.Column<string>(type: "TEXT", nullable: false),
                    pid = table.Column<int>(type: "INTEGER", nullable: true),
                    last_job_id = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    server_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    started_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    finished_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    exit_code = table.Column<int>(type: "INTEGER", nullable: true),
                    log_blob = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_jobs_servers_server_id",
                        column: x => x.server_id,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_jobs_server_id",
                table: "jobs",
                column: "server_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "servers");
        }
    }
}
