using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoadmapNodes_Slug",
                table: "RoadmapNodes");

            migrationBuilder.DropIndex(
                name: "IX_MockExams_OpenPerUser",
                table: "MockExams");

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "RoadmapNodes",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Posts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "MockExams",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Challenges",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ExamName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Label = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Basis = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MockExamStyle = table.Column<int>(type: "integer", nullable: false),
                    ExamDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SecondExamDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 102,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 103,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 104,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 105,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 203,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 204,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 205,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 300,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 301,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 302,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 303,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 304,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 305,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 400,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 401,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 402,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 403,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 404,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 405,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 406,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 407,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 408,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 500,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 501,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 502,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 503,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 504,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 505,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 506,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 507,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 600,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 601,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 602,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 603,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 604,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 605,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 606,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 607,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 700,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 701,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 702,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 703,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 704,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 705,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 800,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 801,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 802,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 803,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 804,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 805,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 806,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 900,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 901,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 902,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 903,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 904,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 905,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1000,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1001,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1002,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1003,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1004,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1005,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1100,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1101,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1102,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1103,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1104,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1200,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1201,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1202,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1203,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1204,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1300,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1301,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1302,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1303,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1304,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1305,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1400,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1401,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1402,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1403,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1404,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1500,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1501,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1502,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1503,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1504,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1505,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1506,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1600,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1601,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1602,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1603,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1604,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1700,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1701,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1702,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1703,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1704,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1800,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1801,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1802,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1803,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1804,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1900,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1901,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1902,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1903,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1904,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 1905,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2000,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2001,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2002,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2003,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2100,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2101,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2102,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2103,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 2104,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 10001,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 10002,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 10003,
                column: "TrackId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 10004,
                column: "TrackId",
                value: 1);

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Basis", "ExamDate", "ExamName", "Label", "Level", "MockExamStyle", "Name", "Order", "SecondExamDate", "Slug" },
                values: new object[,]
                {
                    { 1, "Os conteúdos de Análise e Desenvolvimento de Sistemas da Portaria Inep nº 169/2026 e a Formação Geral, na ordem sugerida de estudo.", new DateOnly(2026, 11, 29), "ENADE", "ENADE ADS", 2, 0, "ENADE 2026 · ADS", 1, null, "enade-ads" },
                    { 2, "As quatro áreas e a redação da Matriz de Referência do ENEM, na ordem dos dias de prova.", new DateOnly(2026, 11, 8), "ENEM", "ENEM", 1, 1, "ENEM 2026", 2, new DateOnly(2026, 11, 15), "enem" }
                });

            migrationBuilder.InsertData(
                table: "RoadmapNodes",
                columns: new[] { "Id", "Block", "Kind", "Order", "ParentId", "Slug", "Summary", "Title", "TrackId" },
                values: new object[,]
                {
                    { 100100, 5, 0, 1, null, "interpretacao-generos", "A base de Linguagens e de toda a prova: quase toda questão começa com um texto, um gráfico ou uma imagem.", "Leitura, interpretação e gêneros textuais", 2 },
                    { 100200, 5, 0, 2, null, "gramatica-uso", "O ENEM não pede regra decorada: pede o efeito de sentido de uma escolha gramatical no texto.", "Gramática em uso", 2 },
                    { 100300, 5, 0, 3, null, "literatura", "Escolas literárias lidas a partir de trechos: o que o texto mostra do contexto em que foi escrito.", "Literatura", 2 },
                    { 100400, 5, 0, 4, null, "artes-corpo-tecnologia", "A parte de Linguagens que vai além do texto verbal.", "Artes, educação física e tecnologias da comunicação", 2 },
                    { 100500, 5, 0, 5, null, "lingua-estrangeira", "Cinco questões, escolhidas na inscrição. É leitura, não gramática: entender a ideia geral e o propósito do texto.", "Língua estrangeira (inglês ou espanhol)", 2 },
                    { 100600, 6, 0, 7, null, "historia-brasil", "Da colonização à Nova República, sempre ligando o passado a permanências de hoje.", "História do Brasil", 2 },
                    { 100700, 6, 0, 8, null, "historia-geral", "Os grandes processos que explicam o mundo atual.", "História geral", 2 },
                    { 100800, 6, 0, 9, null, "geografia-fisica-ambiental", "Natureza e sociedade: clima, relevo e os impactos da ação humana.", "Geografia física e questões ambientais", 2 },
                    { 100900, 6, 0, 10, null, "geografia-humana", "População, cidades, campo e economia no Brasil e no mundo.", "Geografia humana e econômica", 2 },
                    { 101000, 6, 0, 11, null, "filosofia-sociologia", "Conceitos que aparecem em trechos de autores: identificar a ideia e aplicá-la a uma situação.", "Filosofia e sociologia", 2 },
                    { 101100, 9, 0, 13, null, "redacao", "Um texto de até 30 linhas sobre um tema social, avaliado em cinco competências de 200 pontos cada.", "Redação dissertativo-argumentativa", 2 },
                    { 101200, 7, 0, 15, null, "biologia", "A disciplina com mais questões em Natureza, quase sempre ligada a saúde e meio ambiente.", "Biologia", 2 },
                    { 101300, 7, 0, 16, null, "quimica", "Contas e conceitos aplicados ao cotidiano: combustíveis, alimentos, poluição.", "Química", 2 },
                    { 101400, 7, 0, 17, null, "fisica", "Fenômenos do dia a dia: energia em casa, movimento, ondas e eletricidade.", "Física", 2 },
                    { 101500, 8, 0, 19, null, "matematica-basica", "O que mais cai em Matemática: razão, proporção e porcentagem em situações do cotidiano.", "Matemática básica e proporcionalidade", 2 },
                    { 101600, 8, 0, 20, null, "estatistica-probabilidade", "Ler gráficos e tabelas e calcular medidas: presença garantida na prova.", "Estatística e probabilidade", 2 },
                    { 101700, 8, 0, 21, null, "geometria", "Áreas, volumes e formas em embalagens, terrenos e construções.", "Geometria", 2 },
                    { 101800, 8, 0, 22, null, "funcoes-algebra", "Modelar situações com funções e ler o gráfico delas.", "Funções e álgebra", 2 },
                    { 110001, 5, 2, 6, null, "checkpoint-linguagens", "Resolva questões de interpretação, gramática em uso e literatura com o relógio ligado: cerca de 3 minutos por questão.", "Checkpoint: questões de Linguagens", 2 },
                    { 110002, 6, 2, 12, null, "checkpoint-humanas", "Resolva questões de História, Geografia, Filosofia e Sociologia. Anote no caderno de erros os conceitos que confundiu.", "Checkpoint: questões de Ciências Humanas", 2 },
                    { 110003, 9, 2, 14, null, "checkpoint-redacao", "Escreva uma redação sobre um tema de prova anterior em até 1 hora e confira cada competência.", "Checkpoint: uma redação completa", 2 },
                    { 110004, 7, 2, 18, null, "checkpoint-natureza", "Resolva questões de Biologia, Química e Física. Refaça as contas que errou sem olhar a resolução.", "Checkpoint: questões de Ciências da Natureza", 2 },
                    { 110005, 8, 2, 23, null, "checkpoint-simulado-enem", "Faça um simulado completo no horário da prova (início às 13h30). Depois, revise o caderno de erros até o dia do ENEM.", "Checkpoint final: simulado de um dia de prova", 2 },
                    { 100101, 5, 1, 1, 100100, "generos-textuais", "Reconhecer o gênero (notícia, crônica, charge, propaganda, artigo de opinião) e a função dele.", "Gêneros textuais", 2 },
                    { 100102, 5, 1, 2, 100100, "coesao-coerencia", "Conectivos, retomadas e o que mantém o texto com sentido.", "Coesão e coerência", 2 },
                    { 100103, 5, 1, 3, 100100, "intertextualidade", "Paródia, paráfrase, citação e alusão entre textos.", "Intertextualidade", 2 },
                    { 100104, 5, 1, 4, 100100, "variacao-linguistica", "Variedades regionais, sociais e de registro, sem preconceito linguístico.", "Variação linguística", 2 },
                    { 100105, 5, 1, 5, 100100, "funcoes-linguagem", "Referencial, emotiva, apelativa, poética, fática e metalinguística.", "Funções da linguagem", 2 },
                    { 100201, 5, 1, 1, 100200, "classes-palavras", "Adjetivos, advérbios e pronomes como escolhas de quem escreve.", "Classes de palavras e efeitos de sentido", 2 },
                    { 100202, 5, 1, 2, 100200, "pontuacao", "Vírgula, aspas, travessão e o que cada uma muda no sentido.", "Pontuação", 2 },
                    { 100203, 5, 1, 3, 100200, "figuras-linguagem", "Metáfora, metonímia, ironia, hipérbole e antítese.", "Figuras de linguagem", 2 },
                    { 100204, 5, 1, 4, 100200, "concordancia-regencia", "Os casos que mais aparecem em textos formais.", "Concordância, regência e crase", 2 },
                    { 100301, 5, 1, 1, 100300, "escolas-literarias", "Do Barroco ao Realismo: características e autores que caem.", "Escolas literárias", 2 },
                    { 100302, 5, 1, 2, 100300, "modernismo", "Semana de 22, as gerações modernistas e a ruptura com a tradição.", "Modernismo", 2 },
                    { 100303, 5, 1, 3, 100300, "literatura-contemporanea", "Autores atuais, literatura indígena, afro-brasileira e periférica.", "Literatura contemporânea", 2 },
                    { 100401, 5, 1, 1, 100400, "artes-visuais", "Ler obras, performances e arte urbana dentro do contexto delas.", "Artes visuais e movimentos artísticos", 2 },
                    { 100402, 5, 1, 2, 100400, "praticas-corporais", "Esporte, dança, lutas e saúde como fenômenos culturais.", "Práticas corporais", 2 },
                    { 100403, 5, 1, 3, 100400, "midias-digitais", "Redes sociais, fake news e linguagem da internet.", "Mídias e tecnologias digitais", 2 },
                    { 100501, 5, 1, 1, 100500, "leitura-estrategica", "Palavras cognatas, falsos cognatos e pistas do contexto.", "Leitura estratégica", 2 },
                    { 100502, 5, 1, 2, 100500, "generos-lingua-estrangeira", "Tirinhas, anúncios, letras de música e notícias.", "Gêneros em língua estrangeira", 2 },
                    { 100601, 6, 1, 1, 100600, "brasil-colonia", "Economia açucareira, escravidão e resistência, mineração.", "Brasil Colônia", 2 },
                    { 100602, 6, 1, 2, 100600, "brasil-imperio", "Independência, Primeiro e Segundo Reinado, abolição.", "Brasil Império", 2 },
                    { 100603, 6, 1, 3, 100600, "republica", "Coronelismo, revoltas, trabalhismo e Estado Novo.", "Primeira República e Era Vargas", 2 },
                    { 100604, 6, 1, 4, 100600, "ditadura-redemocratizacao", "Golpe de 1964, AI-5, Diretas Já e a Constituição de 1988.", "Ditadura militar e redemocratização", 2 },
                    { 100701, 6, 1, 1, 100700, "antiguidade-medieval", "Grécia, Roma, feudalismo e a Igreja medieval.", "Antiguidade e Idade Média", 2 },
                    { 100702, 6, 1, 2, 100700, "idade-moderna", "Grandes navegações, Reforma, absolutismo e Iluminismo.", "Idade Moderna", 2 },
                    { 100703, 6, 1, 3, 100700, "revolucoes", "Revolução Francesa, Revolução Industrial e as ideias do século XIX.", "Revoluções e industrialização", 2 },
                    { 100704, 6, 1, 4, 100700, "seculo-xx", "Guerras mundiais, Guerra Fria, descolonização e globalização.", "Século XX", 2 },
                    { 100801, 6, 1, 1, 100800, "cartografia", "Escala, projeções, coordenadas e leitura de mapas.", "Cartografia", 2 },
                    { 100802, 6, 1, 2, 100800, "clima-biomas", "Tipos de clima, fenômenos climáticos e os biomas do Brasil.", "Clima e biomas brasileiros", 2 },
                    { 100803, 6, 1, 3, 100800, "relevo-hidrografia", "Formação do relevo, erosão e bacias hidrográficas.", "Relevo, solo e hidrografia", 2 },
                    { 100804, 6, 1, 4, 100800, "questoes-ambientais", "Desmatamento, mudanças climáticas, acordos internacionais e sustentabilidade.", "Questões ambientais", 2 },
                    { 100901, 6, 1, 1, 100900, "populacao", "Pirâmides etárias, transição demográfica e fluxos migratórios.", "População e migrações", 2 },
                    { 100902, 6, 1, 2, 100900, "urbanizacao", "Metropolização, segregação socioespacial e problemas urbanos.", "Urbanização", 2 },
                    { 100903, 6, 1, 3, 100900, "espaco-agrario", "Agronegócio, agricultura familiar, conflitos no campo e reforma agrária.", "Espaço agrário", 2 },
                    { 100904, 6, 1, 4, 100900, "globalizacao-industria", "Fluxos globais, blocos econômicos e matriz energética.", "Globalização, indústria e energia", 2 },
                    { 101001, 6, 1, 1, 101000, "filosofia-antiga", "Pré-socráticos, Sócrates, Platão e Aristóteles.", "Filosofia antiga", 2 },
                    { 101002, 6, 1, 2, 101000, "filosofia-moderna-etica", "Racionalismo, empirismo, contratualistas e Kant.", "Filosofia moderna e ética", 2 },
                    { 101003, 6, 1, 3, 101000, "sociologia-classica", "Durkheim, Weber e Marx: fato social, ação social e classes.", "Sociologia clássica", 2 },
                    { 101004, 6, 1, 4, 101000, "cidadania-movimentos", "Direitos humanos, democracia, cultura e indústria cultural.", "Cidadania e movimentos sociais", 2 },
                    { 101101, 9, 1, 1, 101100, "competencias-redacao", "Norma culta, tema e tipo textual, argumentação, coesão e proposta de intervenção.", "As cinco competências", 2 },
                    { 101102, 9, 1, 2, 101100, "estrutura-texto", "Introdução com tese, dois parágrafos de desenvolvimento e conclusão.", "Estrutura do texto", 2 },
                    { 101103, 9, 1, 3, 101100, "repertorio", "Dados, leis, obras e fatos históricos usados a favor da tese, não só citados.", "Repertório sociocultural", 2 },
                    { 101104, 9, 1, 4, 101100, "proposta-intervencao", "Agente, ação, meio, finalidade e detalhamento, respeitando os direitos humanos.", "Proposta de intervenção", 2 },
                    { 101105, 9, 1, 5, 101100, "erros-zeram", "Fuga ao tema, texto com menos de 7 linhas, parte desconectada e outros casos do edital.", "O que zera a redação", 2 },
                    { 101201, 7, 1, 1, 101200, "ecologia", "Cadeias alimentares, ciclos biogeoquímicos, relações ecológicas e impactos ambientais.", "Ecologia", 2 },
                    { 101202, 7, 1, 2, 101200, "genetica", "Leis de Mendel, heredogramas, DNA e biotecnologia.", "Genética", 2 },
                    { 101203, 7, 1, 3, 101200, "fisiologia-humana", "Sistemas do corpo, vacinas, doenças e saneamento.", "Fisiologia humana e saúde", 2 },
                    { 101204, 7, 1, 4, 101200, "citologia", "Célula, organelas, membrana e divisão celular.", "Citologia", 2 },
                    { 101205, 7, 1, 5, 101200, "evolucao", "Darwin, seleção natural e especiação.", "Evolução", 2 },
                    { 101301, 7, 1, 1, 101300, "estequiometria", "Mol, massa molar e proporções numa reação.", "Estequiometria", 2 },
                    { 101302, 7, 1, 2, 101300, "solucoes", "Concentração comum, molaridade, diluição e mistura.", "Soluções e concentração", 2 },
                    { 101303, 7, 1, 3, 101300, "quimica-organica", "Funções orgânicas, polímeros e combustíveis.", "Química orgânica", 2 },
                    { 101304, 7, 1, 4, 101300, "fisico-quimica", "Energia das reações, velocidade e pH.", "Termoquímica, cinética e equilíbrio", 2 },
                    { 101305, 7, 1, 5, 101300, "quimica-ambiental", "Chuva ácida, efeito estufa, tratamento de água e reciclagem.", "Química ambiental", 2 },
                    { 101401, 7, 1, 1, 101400, "mecanica", "Velocidade, aceleração, leis de Newton, trabalho e energia.", "Mecânica", 2 },
                    { 101402, 7, 1, 2, 101400, "eletricidade", "Circuitos, potência, consumo de energia e conta de luz.", "Eletricidade", 2 },
                    { 101403, 7, 1, 3, 101400, "ondas-optica", "Som, luz, espelhos, lentes e fenômenos ondulatórios.", "Ondas e óptica", 2 },
                    { 101404, 7, 1, 4, 101400, "termologia", "Calor, temperatura, dilatação e máquinas térmicas.", "Termologia", 2 },
                    { 101501, 8, 1, 1, 101500, "razao-proporcao", "Grandezas direta e inversamente proporcionais, escalas.", "Razão, proporção e regra de três", 2 },
                    { 101502, 8, 1, 2, 101500, "porcentagem", "Aumentos, descontos sucessivos e juros simples e compostos.", "Porcentagem e juros", 2 },
                    { 101503, 8, 1, 3, 101500, "unidades-medida", "Conversões de comprimento, área, volume e tempo.", "Unidades de medida", 2 },
                    { 101601, 8, 1, 1, 101600, "medidas-tendencia", "Quando usar cada medida e como calcular com tabelas de frequência.", "Média, mediana e moda", 2 },
                    { 101602, 8, 1, 2, 101600, "graficos-tabelas-enem", "Barras, linhas, setores e o que cada gráfico permite concluir.", "Gráficos e tabelas", 2 },
                    { 101603, 8, 1, 3, 101600, "probabilidade-enem", "Casos favoráveis sobre possíveis, eventos independentes e complementares.", "Probabilidade", 2 },
                    { 101604, 8, 1, 4, 101600, "analise-combinatoria", "Princípio fundamental da contagem, arranjos e combinações.", "Análise combinatória", 2 },
                    { 101701, 8, 1, 1, 101700, "geometria-plana", "Áreas e perímetros, semelhança de triângulos e Pitágoras.", "Geometria plana", 2 },
                    { 101702, 8, 1, 2, 101700, "geometria-espacial", "Volumes de prismas, cilindros, cones e esferas; planificações.", "Geometria espacial", 2 },
                    { 101703, 8, 1, 3, 101700, "trigonometria", "Seno, cosseno e tangente no triângulo retângulo.", "Trigonometria", 2 },
                    { 101801, 8, 1, 1, 101800, "funcao-afim-quadratica", "Gráfico, raízes, vértice e problemas de máximo e mínimo.", "Funções do 1º e do 2º grau", 2 },
                    { 101802, 8, 1, 2, 101800, "exponencial-logaritmo", "Crescimento, decaimento e escalas logarítmicas.", "Exponencial e logaritmo", 2 },
                    { 101803, 8, 1, 3, 101800, "sequencias", "Termo geral e soma dos termos.", "Progressões aritmética e geométrica", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodes_TrackId_Slug",
                table: "RoadmapNodes",
                columns: new[] { "TrackId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TrackId",
                table: "Posts",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_MockExams_OpenPerUser",
                table: "MockExams",
                columns: new[] { "UserId", "TrackId" },
                unique: true,
                filter: "\"SubmittedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MockExams_TrackId",
                table: "MockExams",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_TrackId",
                table: "Challenges",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_Slug",
                table: "Tracks",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Tracks_TrackId",
                table: "Challenges",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MockExams_Tracks_TrackId",
                table: "MockExams",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Tracks_TrackId",
                table: "Posts",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadmapNodes_Tracks_TrackId",
                table: "RoadmapNodes",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Tudo o que existia antes das trilhas era do ENADE ADS (por isso o default 1 acima). A data que o admin
            // tinha gravado na tela passa para a trilha, e a configuração antiga sai.
            migrationBuilder.Sql("""
                UPDATE "Tracks" SET "ExamDate" = CAST(s."Value" AS date)
                FROM "SiteSettings" s
                WHERE s."Key" = 'ExamDate' AND "Tracks"."Id" = 1;
                """);
            migrationBuilder.Sql("""DELETE FROM "SiteSettings" WHERE "Key" = 'ExamDate';""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Tracks_TrackId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_MockExams_Tracks_TrackId",
                table: "MockExams");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Tracks_TrackId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadmapNodes_Tracks_TrackId",
                table: "RoadmapNodes");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_RoadmapNodes_TrackId_Slug",
                table: "RoadmapNodes");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TrackId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_MockExams_OpenPerUser",
                table: "MockExams");

            migrationBuilder.DropIndex(
                name: "IX_MockExams_TrackId",
                table: "MockExams");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_TrackId",
                table: "Challenges");

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100101);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100102);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100103);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100104);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100105);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100201);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100202);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100203);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100204);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100301);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100302);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100303);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100401);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100402);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100403);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100501);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100502);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100601);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100602);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100603);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100604);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100701);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100702);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100703);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100704);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100801);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100802);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100803);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100804);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100901);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100902);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100903);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100904);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101001);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101002);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101003);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101004);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101101);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101102);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101103);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101104);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101105);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101201);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101202);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101203);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101204);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101205);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101301);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101302);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101303);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101304);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101305);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101401);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101402);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101403);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101404);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101501);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101502);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101503);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101601);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101602);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101603);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101604);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101701);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101702);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101703);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101801);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101802);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101803);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 110001);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 110002);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 110003);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 110004);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 110005);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100100);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100200);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100300);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100400);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100500);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100600);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100700);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100800);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 100900);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101000);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101100);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101200);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101300);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101400);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101500);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101600);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101700);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 101800);

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "RoadmapNodes");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "MockExams");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Challenges");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodes_Slug",
                table: "RoadmapNodes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MockExams_OpenPerUser",
                table: "MockExams",
                column: "UserId",
                unique: true,
                filter: "\"SubmittedAt\" IS NULL");
        }
    }
}
