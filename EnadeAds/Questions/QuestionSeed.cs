namespace EnadeAds.Questions;

/// <summary>
/// Questões de exemplo de autoria própria, para o banco não nascer vazio. As questões das provas do Inep
/// entram na #19. Ids fixos porque vão para a seed da migration.
/// </summary>
public static class QuestionSeed
{
    public static readonly DateTimeOffset SeedDate = new(2026, 10, 29, 0, 0, 0, TimeSpan.Zero);

    private const string OwnWork = "Autoria própria – ENADE ADS";

    private sealed record Seed(int Id, int SubjectId, string Statement, (string Text, string Explanation)[] Options, char Correct);

    // Ids das matérias vêm da RoadmapSeed: n-ésima matéria = n × 100.
    private const int Architecture = 200;
    private const int DataStructures = 400;
    private const int ObjectOrientation = 500;
    private const int Databases = 600;
    private const int Networks = 800;
    private const int Requirements = 1200;
    private const int Quality = 1500;

    private static readonly Seed[] Seeds =
    [
        new(1, DataStructures,
            """
            Uma pilha começa vazia e recebe, nesta ordem, as operações:

            push(1), push(2), pop(), push(3), push(4), pop(), push(5)

            Qual é o conteúdo da pilha ao final, da base para o topo?
            """,
            [
                ("1, 3, 5", "Correta. Depois de push(1) e push(2), o pop() remove o 2. Entram 3 e 4, o pop() remove o 4 (topo), e por fim entra o 5."),
                ("1, 2, 5", "O primeiro pop() remove o elemento do topo, que é o 2. Ele não fica na pilha."),
                ("5, 3, 1", "Os elementos estão certos, mas na ordem do topo para a base. A pergunta pede da base para o topo."),
                ("3, 4, 5", "Esse seria o resultado de uma fila (FIFO), em que cada remoção tira o elemento mais antigo (1 e depois 2). Na pilha (LIFO), sai sempre o topo."),
                ("1, 3, 4, 5", "O segundo pop() acontece depois do push(4), então remove o 4."),
            ], 'A'),

        new(2, Architecture,
            "O número 45, em decimal, corresponde a quais valores em binário e em hexadecimal, respectivamente?",
            [
                ("101101 e 2D", "Correta. 45 = 32 + 8 + 4 + 1 = 101101 em binário. Agrupando de 4 em 4 bits da direita: 0010 1101 = 2D."),
                ("101011 e 2B", "101011 vale 43 (32 + 8 + 2 + 1), não 45."),
                ("110101 e 35", "110101 vale 53. O 35 hexadecimal também vale 53: é a leitura de 53, não de 45."),
                ("101101 e 2E", "O binário está certo, mas 1101 vale 13, que é D em hexadecimal. E corresponde a 14."),
                ("100101 e 25", "100101 vale 37. O 25 hexadecimal também vale 37."),
            ], 'A'),

        new(3, Networks,
            "Um computador tem o endereço IPv4 192.168.10.70 com máscara /26. Qual é o endereço da sub-rede, o endereço de broadcast e a quantidade de hosts utilizáveis nessa sub-rede?",
            [
                ("Rede 192.168.10.64, broadcast 192.168.10.127, 62 hosts", "Correta. /26 = 255.255.255.192, blocos de 64 no último octeto (0, 64, 128, 192). 70 está no bloco 64–127. Hosts: 2^6 − 2 = 62."),
                ("Rede 192.168.10.0, broadcast 192.168.10.63, 62 hosts", "Esse é o primeiro bloco /26. O endereço 70 está no segundo bloco, de 64 a 127."),
                ("Rede 192.168.10.64, broadcast 192.168.10.127, 64 hosts", "O bloco tem 64 endereços, mas o de rede e o de broadcast não podem ser usados por hosts: sobram 62."),
                ("Rede 192.168.10.64, broadcast 192.168.10.128, 62 hosts", "192.168.10.128 já é o endereço de rede do bloco seguinte. O broadcast é o último endereço do bloco: 127."),
                ("Rede 192.168.10.70, broadcast 192.168.10.127, 57 hosts", "O endereço de rede é sempre o início do bloco (múltiplo de 64 com /26), não o endereço do host."),
            ], 'A'),

        new(4, Databases,
            """
            Considere as tabelas:

            Curso                 Aluno
            id | nome             id | nome  | curso_id
            ---+------            ---+-------+---------
             1 | ADS               1 | Ana   | 1
             2 | Redes             2 | Bruno | 1
             3 | Jogos             3 | Carla | 2
                                   4 | Davi  | NULL

            E a consulta:

            SELECT c.nome, COUNT(a.id)
            FROM Curso c
            LEFT JOIN Aluno a ON a.curso_id = c.id
            GROUP BY c.nome
            ORDER BY c.nome;

            Qual é o resultado?
            """,
            [
                ("ADS 2; Jogos 0; Redes 1", "Correta. O LEFT JOIN mantém todos os cursos, inclusive Jogos sem alunos. COUNT(a.id) ignora NULL, então Jogos conta 0. Davi não tem curso e não aparece, porque a tabela da esquerda é Curso."),
                ("ADS 2; Redes 1", "Esse seria o resultado com INNER JOIN, que descarta cursos sem alunos. O LEFT JOIN mantém Jogos."),
                ("ADS 2; Jogos 1; Redes 1", "Seria o resultado com COUNT(*), que conta a linha de Jogos mesmo com os campos de Aluno nulos. COUNT(a.id) ignora os nulos."),
                ("ADS 2; Redes 1; NULL 1", "Davi apareceria com um RIGHT JOIN (ou com Aluno à esquerda). Aqui a tabela da esquerda é Curso."),
                ("ADS 3; Jogos 0; Redes 1", "Só Ana e Bruno têm curso_id = 1. Davi tem curso_id nulo e não entra em nenhum curso."),
            ], 'A'),

        new(5, Requirements,
            "Na especificação de um sistema acadêmico, qual dos itens abaixo é um requisito NÃO funcional?",
            [
                ("O sistema deve permitir que o professor lance as notas dos alunos.", "Descreve uma função que o sistema oferece ao usuário: é requisito funcional."),
                ("O sistema deve emitir o boletim do aluno em PDF.", "Também é uma função do sistema (emitir boletim). O formato PDF faz parte do que a função entrega."),
                ("O sistema deve responder às consultas de notas em até 2 segundos em 95% das requisições.", "Correta. Define uma qualidade (desempenho) de como o sistema deve se comportar, e não uma função nova: é requisito não funcional."),
                ("O sistema deve enviar um e-mail de confirmação após a matrícula.", "Descreve um comportamento do sistema em resposta a um evento: é requisito funcional."),
                ("O sistema deve permitir que o aluno cancele uma disciplina.", "É uma função disponível ao aluno: requisito funcional."),
            ], 'C'),

        new(6, Quality,
            "Um formulário aceita idades de 18 a 60 anos, inclusive. Pela técnica de análise de valor-limite, qual conjunto de valores de teste cobre os limites da regra?",
            [
                ("17, 18, 60 e 61", "Correta. Testa cada limite (18 e 60) e o valor imediatamente fora de cada um (17 e 61), onde os erros de < e <= costumam aparecer."),
                ("18, 39 e 60", "Testa os limites e um valor do meio, mas não testa nenhum valor inválido logo fora dos limites."),
                ("0, 18, 60 e 100", "0 e 100 são inválidos, mas estão longe dos limites. Um erro como usar > 60 em vez de >= 61 passaria despercebido."),
                ("17 e 61", "Só testa os valores inválidos. Um erro que recuse o próprio 18 ou 60 não seria encontrado."),
                ("18 e 60", "Só testa os valores válidos dos limites. Não confirma que 17 e 61 são recusados."),
            ], 'A'),

        new(7, ObjectOrientation,
            """
            Considere o código Java:

            class Animal { String som() { return "..."; } }
            class Cachorro extends Animal { String som() { return "Au"; } }
            class Gato extends Animal { String som() { return "Miau"; } }

            Animal[] lista = { new Cachorro(), new Gato(), new Animal() };
            for (Animal a : lista) {
                System.out.print(a.som() + " ");
            }

            O que é impresso?
            """,
            [
                ("Au Miau ...", "Correta. É polimorfismo por sobrescrita: o método chamado depende do tipo real do objeto em tempo de execução, não do tipo da variável (Animal)."),
                ("... ... ...", "Seria verdade se o método fosse escolhido pelo tipo da variável. Em Java, métodos de instância usam ligação dinâmica."),
                ("Au Au Au", "Cada objeto da lista é de uma classe diferente e chama a própria versão de som()."),
                ("Erro de compilação, porque Cachorro e Gato não podem ser guardados num vetor de Animal.", "Uma subclasse pode ser atribuída a uma variável da superclasse: Cachorro é um Animal."),
                ("Erro de execução, porque Animal não é uma classe abstrata.", "Animal é concreta e pode ser instanciada normalmente; o terceiro elemento imprime \"...\"."),
            ], 'A'),
    ];

    public static IReadOnlyList<Question> Questions { get; } = Seeds.Select(seed => new Question
    {
        Id = seed.Id,
        SubjectId = seed.SubjectId,
        Statement = Normalize(seed.Statement),
        Source = OwnWork,
        CreatedAt = SeedDate,
    }).ToList();

    public static IReadOnlyList<QuestionOption> Options { get; } = Seeds.SelectMany(seed => seed.Options.Select((option, index) =>
    {
        var letter = (char)('A' + index);
        return new QuestionOption
        {
            Id = seed.Id * 10 + index + 1,
            QuestionId = seed.Id,
            Letter = letter,
            Text = option.Text,
            Explanation = option.Explanation,
            IsCorrect = letter == seed.Correct,
        };
    })).ToList();

    private static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();
}
