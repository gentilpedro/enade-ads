using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscursiveQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpectedAnswer",
                table: "Questions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuggestedLines",
                table: "Questions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscursiveAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    AnsweredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscursiveAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscursiveAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscursiveAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ExpectedAnswer", "Kind", "SuggestedLines" },
                values: new object[] { null, 0, null });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "ArchivedAt", "AuthorId", "CreatedAt", "ExpectedAnswer", "Kind", "Source", "Statement", "SubjectId", "SuggestedLines" },
                values: new object[] { 8, null, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Uma boa resposta traz dois requisitos não funcionais mensuráveis, ligados aos problemas do enunciado, e um jeito concreto de verificar cada um.\n\n1. **Desempenho e disponibilidade:** por exemplo, \"o aplicativo deve atender 5.000 alunos simultâneos com tempo de resposta de até 2 segundos em 95% das requisições, e ficar disponível 99,5% do tempo durante o período de rematrícula\". Verificação: teste de carga simulando o pico do primeiro dia, com medição do tempo de resposta e da taxa de erros.\n\n2. **Acessibilidade:** por exemplo, \"as telas devem seguir as diretrizes WCAG 2.1 nível AA, funcionando com leitores de tela e navegação sem toque\". Verificação: avaliação com ferramentas automáticas de acessibilidade, revisão pelos critérios da WCAG e teste com usuários que usam leitor de tela.\n\nPerde pontos a resposta que:\n- descreve funções do sistema (\"o aluno deve escolher as disciplinas\"), que são requisitos funcionais.\n- usa termos vagos sem métrica (\"o sistema deve ser rápido\", \"deve ser fácil de usar\").\n- não diz como verificar o requisito.", 1, "Autoria própria – ENADE ADS", "Uma universidade vai lançar um aplicativo para os alunos fazerem a rematrícula pelo celular.\nNo último semestre, o sistema antigo ficou fora do ar no primeiro dia de rematrícula e houve\nreclamações de alunos com deficiência visual que não conseguiam usar as telas.\n\nProponha **dois requisitos não funcionais** para o novo aplicativo, ligados aos problemas descritos.\nPara cada um, explique como a equipe poderia **verificar** se ele foi atendido antes da entrega.", 1200, 15 });

            migrationBuilder.CreateIndex(
                name: "IX_DiscursiveAnswers_QuestionId",
                table: "DiscursiveAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscursiveAnswers_UserId_QuestionId",
                table: "DiscursiveAnswers",
                columns: new[] { "UserId", "QuestionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscursiveAnswers");

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "ExpectedAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SuggestedLines",
                table: "Questions");
        }
    }
}
