using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    Statement = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    Source = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Questions_RoadmapNodes_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "RoadmapNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Letter = table.Column<char>(type: "character(1)", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    OptionId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionAttempts_QuestionOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionAttempts_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AuthorId", "CreatedAt", "Source", "Statement", "SubjectId" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Uma pilha começa vazia e recebe, nesta ordem, as operações:\n\npush(1), push(2), pop(), push(3), push(4), pop(), push(5)\n\nQual é o conteúdo da pilha ao final, da base para o topo?", 400 },
                    { 2, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "O número 45, em decimal, corresponde a quais valores em binário e em hexadecimal, respectivamente?", 200 },
                    { 3, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Um computador tem o endereço IPv4 192.168.10.70 com máscara /26. Qual é o endereço da sub-rede, o endereço de broadcast e a quantidade de hosts utilizáveis nessa sub-rede?", 800 },
                    { 4, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Considere as tabelas:\n\nCurso                 Aluno\nid | nome             id | nome  | curso_id\n---+------            ---+-------+---------\n 1 | ADS               1 | Ana   | 1\n 2 | Redes             2 | Bruno | 1\n 3 | Jogos             3 | Carla | 2\n                       4 | Davi  | NULL\n\nE a consulta:\n\nSELECT c.nome, COUNT(a.id)\nFROM Curso c\nLEFT JOIN Aluno a ON a.curso_id = c.id\nGROUP BY c.nome\nORDER BY c.nome;\n\nQual é o resultado?", 600 },
                    { 5, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Na especificação de um sistema acadêmico, qual dos itens abaixo é um requisito NÃO funcional?", 1200 },
                    { 6, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Um formulário aceita idades de 18 a 60 anos, inclusive. Pela técnica de análise de valor-limite, qual conjunto de valores de teste cobre os limites da regra?", 1500 },
                    { 7, null, new DateTimeOffset(new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Autoria própria – ENADE ADS", "Considere o código Java:\n\nclass Animal { String som() { return \"...\"; } }\nclass Cachorro extends Animal { String som() { return \"Au\"; } }\nclass Gato extends Animal { String som() { return \"Miau\"; } }\n\nAnimal[] lista = { new Cachorro(), new Gato(), new Animal() };\nfor (Animal a : lista) {\n    System.out.print(a.som() + \" \");\n}\n\nO que é impresso?", 500 }
                });

            migrationBuilder.InsertData(
                table: "QuestionOptions",
                columns: new[] { "Id", "Explanation", "IsCorrect", "Letter", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 11, "Correta. Depois de push(1) e push(2), o pop() remove o 2. Entram 3 e 4, o pop() remove o 4 (topo), e por fim entra o 5.", true, 'A', 1, "1, 3, 5" },
                    { 12, "O primeiro pop() remove o elemento do topo, que é o 2. Ele não fica na pilha.", false, 'B', 1, "1, 2, 5" },
                    { 13, "Os elementos estão certos, mas na ordem do topo para a base. A pergunta pede da base para o topo.", false, 'C', 1, "5, 3, 1" },
                    { 14, "Esse seria o resultado de uma fila (FIFO), em que cada remoção tira o elemento mais antigo (1 e depois 2). Na pilha (LIFO), sai sempre o topo.", false, 'D', 1, "3, 4, 5" },
                    { 15, "O segundo pop() acontece depois do push(4), então remove o 4.", false, 'E', 1, "1, 3, 4, 5" },
                    { 21, "Correta. 45 = 32 + 8 + 4 + 1 = 101101 em binário. Agrupando de 4 em 4 bits da direita: 0010 1101 = 2D.", true, 'A', 2, "101101 e 2D" },
                    { 22, "101011 vale 43 (32 + 8 + 2 + 1), não 45.", false, 'B', 2, "101011 e 2B" },
                    { 23, "110101 vale 53. O 35 hexadecimal também vale 53: é a leitura de 53, não de 45.", false, 'C', 2, "110101 e 35" },
                    { 24, "O binário está certo, mas 1101 vale 13, que é D em hexadecimal. E corresponde a 14.", false, 'D', 2, "101101 e 2E" },
                    { 25, "100101 vale 37. O 25 hexadecimal também vale 37.", false, 'E', 2, "100101 e 25" },
                    { 31, "Correta. /26 = 255.255.255.192, blocos de 64 no último octeto (0, 64, 128, 192). 70 está no bloco 64–127. Hosts: 2^6 − 2 = 62.", true, 'A', 3, "Rede 192.168.10.64, broadcast 192.168.10.127, 62 hosts" },
                    { 32, "Esse é o primeiro bloco /26. O endereço 70 está no segundo bloco, de 64 a 127.", false, 'B', 3, "Rede 192.168.10.0, broadcast 192.168.10.63, 62 hosts" },
                    { 33, "O bloco tem 64 endereços, mas o de rede e o de broadcast não podem ser usados por hosts: sobram 62.", false, 'C', 3, "Rede 192.168.10.64, broadcast 192.168.10.127, 64 hosts" },
                    { 34, "192.168.10.128 já é o endereço de rede do bloco seguinte. O broadcast é o último endereço do bloco: 127.", false, 'D', 3, "Rede 192.168.10.64, broadcast 192.168.10.128, 62 hosts" },
                    { 35, "O endereço de rede é sempre o início do bloco (múltiplo de 64 com /26), não o endereço do host.", false, 'E', 3, "Rede 192.168.10.70, broadcast 192.168.10.127, 57 hosts" },
                    { 41, "Correta. O LEFT JOIN mantém todos os cursos, inclusive Jogos sem alunos. COUNT(a.id) ignora NULL, então Jogos conta 0. Davi não tem curso e não aparece, porque a tabela da esquerda é Curso.", true, 'A', 4, "ADS 2; Jogos 0; Redes 1" },
                    { 42, "Esse seria o resultado com INNER JOIN, que descarta cursos sem alunos. O LEFT JOIN mantém Jogos.", false, 'B', 4, "ADS 2; Redes 1" },
                    { 43, "Seria o resultado com COUNT(*), que conta a linha de Jogos mesmo com os campos de Aluno nulos. COUNT(a.id) ignora os nulos.", false, 'C', 4, "ADS 2; Jogos 1; Redes 1" },
                    { 44, "Davi apareceria com um RIGHT JOIN (ou com Aluno à esquerda). Aqui a tabela da esquerda é Curso.", false, 'D', 4, "ADS 2; Redes 1; NULL 1" },
                    { 45, "Só Ana e Bruno têm curso_id = 1. Davi tem curso_id nulo e não entra em nenhum curso.", false, 'E', 4, "ADS 3; Jogos 0; Redes 1" },
                    { 51, "Descreve uma função que o sistema oferece ao usuário: é requisito funcional.", false, 'A', 5, "O sistema deve permitir que o professor lance as notas dos alunos." },
                    { 52, "Também é uma função do sistema (emitir boletim). O formato PDF faz parte do que a função entrega.", false, 'B', 5, "O sistema deve emitir o boletim do aluno em PDF." },
                    { 53, "Correta. Define uma qualidade (desempenho) de como o sistema deve se comportar, e não uma função nova: é requisito não funcional.", true, 'C', 5, "O sistema deve responder às consultas de notas em até 2 segundos em 95% das requisições." },
                    { 54, "Descreve um comportamento do sistema em resposta a um evento: é requisito funcional.", false, 'D', 5, "O sistema deve enviar um e-mail de confirmação após a matrícula." },
                    { 55, "É uma função disponível ao aluno: requisito funcional.", false, 'E', 5, "O sistema deve permitir que o aluno cancele uma disciplina." },
                    { 61, "Correta. Testa cada limite (18 e 60) e o valor imediatamente fora de cada um (17 e 61), onde os erros de < e <= costumam aparecer.", true, 'A', 6, "17, 18, 60 e 61" },
                    { 62, "Testa os limites e um valor do meio, mas não testa nenhum valor inválido logo fora dos limites.", false, 'B', 6, "18, 39 e 60" },
                    { 63, "0 e 100 são inválidos, mas estão longe dos limites. Um erro como usar > 60 em vez de >= 61 passaria despercebido.", false, 'C', 6, "0, 18, 60 e 100" },
                    { 64, "Só testa os valores inválidos. Um erro que recuse o próprio 18 ou 60 não seria encontrado.", false, 'D', 6, "17 e 61" },
                    { 65, "Só testa os valores válidos dos limites. Não confirma que 17 e 61 são recusados.", false, 'E', 6, "18 e 60" },
                    { 71, "Correta. É polimorfismo por sobrescrita: o método chamado depende do tipo real do objeto em tempo de execução, não do tipo da variável (Animal).", true, 'A', 7, "Au Miau ..." },
                    { 72, "Seria verdade se o método fosse escolhido pelo tipo da variável. Em Java, métodos de instância usam ligação dinâmica.", false, 'B', 7, "... ... ..." },
                    { 73, "Cada objeto da lista é de uma classe diferente e chama a própria versão de som().", false, 'C', 7, "Au Au Au" },
                    { 74, "Uma subclasse pode ser atribuída a uma variável da superclasse: Cachorro é um Animal.", false, 'D', 7, "Erro de compilação, porque Cachorro e Gato não podem ser guardados num vetor de Animal." },
                    { 75, "Animal é concreta e pode ser instanciada normalmente; o terceiro elemento imprime \"...\".", false, 'E', 7, "Erro de execução, porque Animal não é uma classe abstrata." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAttempts_OptionId",
                table: "QuestionAttempts",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAttempts_QuestionId",
                table: "QuestionAttempts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAttempts_UserId_QuestionId",
                table: "QuestionAttempts",
                columns: new[] { "UserId", "QuestionId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId_Letter",
                table: "QuestionOptions",
                columns: new[] { "QuestionId", "Letter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectId",
                table: "Questions",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionAttempts");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
