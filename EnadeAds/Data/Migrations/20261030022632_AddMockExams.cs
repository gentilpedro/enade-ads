using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMockExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MockExams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeadlineAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SubmittedByTimeout = table.Column<bool>(type: "boolean", nullable: false),
                    ObjectiveCount = table.Column<int>(type: "integer", nullable: false),
                    CorrectCount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockExams_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockExamQuestions",
                columns: table => new
                {
                    MockExamId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsGeneralEducation = table.Column<bool>(type: "boolean", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: true),
                    DiscursiveText = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockExamQuestions", x => new { x.MockExamId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_MockExamQuestions_MockExams_MockExamId",
                        column: x => x.MockExamId,
                        principalTable: "MockExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MockExamQuestions_QuestionOptions_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MockExamQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MockExamQuestions_QuestionId",
                table: "MockExamQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MockExamQuestions_SelectedOptionId",
                table: "MockExamQuestions",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MockExams_OpenPerUser",
                table: "MockExams",
                column: "UserId",
                unique: true,
                filter: "\"SubmittedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MockExams_UserId_Id",
                table: "MockExams",
                columns: new[] { "UserId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MockExamQuestions");

            migrationBuilder.DropTable(
                name: "MockExams");
        }
    }
}
