using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityFeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Até aqui só professor e admin publicavam: todo post existente é aviso oficial.
            migrationBuilder.AddColumn<bool>(
                name: "IsOfficial",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RemovedAt",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedById",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PostReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: true),
                    ReporterId = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedById = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReports_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PostReports_AspNetUsers_ResolvedById",
                        column: x => x.ResolvedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PostReports_PostComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "PostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReports_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostVotes",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVotes", x => new { x.PostId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PostVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostVotes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_RemovedById",
                table: "Posts",
                column: "RemovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_CommentId",
                table: "PostReports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_PostId",
                table: "PostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReporterId",
                table: "PostReports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ResolvedAt",
                table: "PostReports",
                column: "ResolvedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ResolvedById",
                table: "PostReports",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_PostVotes_UserId",
                table: "PostVotes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_RemovedById",
                table: "Posts",
                column: "RemovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_RemovedById",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "PostReports");

            migrationBuilder.DropTable(
                name: "PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_Posts_RemovedById",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsOfficial",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RemovedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RemovedById",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Posts");
        }
    }
}
