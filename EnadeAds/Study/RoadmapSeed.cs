namespace EnadeAds.Study;

/// <summary>
/// Conteúdo inicial do roadmap: os 20 objetos de conhecimento da Portaria Inep nº 169/2026 (Art. 6º)
/// mais a Formação Geral, na ordem sugerida de estudo. Os ids são fixos porque entram como seed da migration.
///
/// ATENÇÃO: desde a #14 os professores editam o roadmap pela tela. Mudar este arquivo gera uma migration
/// com UpdateData que sobrescreve no banco o que eles editaram. Para mudar conteúdo, use a tela.
/// </summary>
public static class RoadmapSeed
{
    private sealed record Subject(string Slug, string Title, StudyBlock Block, string Summary, (string Slug, string Title, string Summary)[] Topics);

    private sealed record Checkpoint(string Slug, string Title, StudyBlock Block, string Summary);

    private static readonly object[] Spine =
    [
        new Subject("logica-matematica", "Lógica matemática e teoria dos conjuntos", StudyBlock.Fundamentals,
            "Base para ler enunciados formais, condições de algoritmos e consultas. Cai em questões de raciocínio e em pseudocódigo.",
            [
                ("proposicoes-tabela-verdade", "Proposições e tabela-verdade", "Conectivos (e, ou, não, se-então, se e somente se) e como montar a tabela-verdade de uma expressão."),
                ("equivalencias-logicas", "Equivalências lógicas", "Leis de De Morgan, contrapositiva e negação de condicionais, muito usadas para simplificar condições."),
                ("quantificadores", "Quantificadores", "Para todo e existe: como negar frases com quantificadores."),
                ("conjuntos-operacoes", "Conjuntos e operações", "União, interseção, diferença, complemento, produto cartesiano e diagramas de Venn."),
                ("relacoes-funcoes", "Relações e funções", "Domínio, imagem, funções injetoras, sobrejetoras e bijetoras."),
            ]),
        new Subject("arquitetura-computadores", "Princípios de arquitetura e organização de computadores", StudyBlock.Fundamentals,
            "Como o computador representa e processa dados. Conversão de bases é ponto garantido com treino.",
            [
                ("conversao-bases", "Sistemas de numeração e conversão de bases", "Decimal, binário, octal e hexadecimal nas duas direções, sem calculadora."),
                ("aritmetica-binaria", "Aritmética binária", "Soma e subtração em binário, complemento de dois e overflow."),
                ("representacao-dados", "Representação de dados", "Bits, bytes, KB x KiB, caracteres (ASCII, Unicode) e ponto flutuante."),
                ("cpu-memoria-barramentos", "CPU, memória e barramentos", "Ciclo de busca e execução, registradores, ULA e unidade de controle."),
                ("hierarquia-memoria", "Hierarquia de memória", "Registradores, cache, RAM e armazenamento: velocidade, custo e localidade."),
            ]),
        new Subject("algoritmos-programacao", "Algoritmos e programação", StudyBlock.Fundamentals,
            "Ler e escrever algoritmos. A questão clássica do ENADE é descobrir o que um pseudocódigo imprime.",
            [
                ("variaveis-tipos", "Variáveis, tipos e operadores", "Tipos primitivos, precedência de operadores e divisão inteira x real."),
                ("estruturas-controle", "Estruturas de controle", "Condicionais e laços (enquanto, para, repita) e suas condições de parada."),
                ("funcoes-recursao", "Funções e recursão", "Parâmetros por valor e por referência, caso base e pilha de chamadas."),
                ("vetores-matrizes", "Vetores e matrizes", "Índices, percursos e operações comuns em arrays."),
                ("rastreamento-pseudocodigo", "Rastreamento de pseudocódigo", "Simular a execução linha a linha anotando o valor de cada variável."),
            ]),
        new Subject("estruturas-dados", "Estruturas de dados", StudyBlock.Fundamentals,
            "Como organizar dados para cada problema e quanto custa cada operação.",
            [
                ("pilhas", "Pilhas", "LIFO: push e pop, usos em desfazer, chamadas de função e avaliação de expressões."),
                ("filas", "Filas", "FIFO: enqueue e dequeue, filas circulares e de prioridade."),
                ("listas-encadeadas", "Listas encadeadas", "Simples, duplas e circulares; inserção e remoção sem deslocar elementos."),
                ("arvores", "Árvores binárias e de busca", "Percursos (pré, em e pós-ordem), inserção e busca em BST, balanceamento."),
                ("tabelas-hash", "Tabelas hash", "Função de hash, colisões e custo médio de busca."),
                ("grafos", "Grafos", "Representações, busca em largura e em profundidade, caminho mínimo."),
                ("complexidade", "Complexidade de algoritmos", "Notação Big-O e comparação entre O(1), O(log n), O(n), O(n log n) e O(n²)."),
                ("ordenacao-busca", "Ordenação e busca", "Bubble, insertion, merge e quick sort; busca linear x binária."),
            ]),
        new Subject("orientacao-objetos", "Orientação a objetos", StudyBlock.Fundamentals,
            "Modelar sistemas com classes e objetos. Cai muito como código que imprime algo com herança e polimorfismo.",
            [
                ("classes-objetos", "Classes, objetos e construtores", "Estado, comportamento, instância e construtor da superclasse."),
                ("encapsulamento", "Encapsulamento", "Modificadores de acesso e por que esconder o estado interno."),
                ("heranca", "Herança", "Reuso e especialização; quando preferir composição."),
                ("polimorfismo", "Polimorfismo", "Sobrescrita x sobrecarga e ligação dinâmica."),
                ("abstrata-interface", "Classe abstrata x interface", "O que cada uma pode ter e quando usar."),
                ("solid", "Princípios SOLID", "Responsabilidade única, aberto/fechado, Liskov, segregação de interfaces e inversão de dependência."),
                ("padroes-projeto", "Padrões de projeto", "Singleton, Factory, Strategy, Observer e Adapter."),
            ]),
        new Checkpoint("checkpoint-fundamentos", "Checkpoint: questões de fundamentos", StudyBlock.Fundamentals,
            "Antes de seguir, resolva questões de lógica, conversão de bases, pseudocódigo, estruturas de dados e orientação a objetos. Anote os erros no caderno de erros."),

        new Subject("banco-de-dados", "Banco de dados", StudyBlock.DataAndInfrastructure,
            "Modelagem e consultas. SQL aparece quase sempre: ler uma consulta e dizer o resultado.",
            [
                ("modelo-er", "Modelo entidade-relacionamento", "Entidades, atributos, relacionamentos e cardinalidade."),
                ("modelo-relacional", "Modelo relacional e chaves", "Tabelas, chave primária, chave estrangeira e integridade referencial."),
                ("normalizacao", "Normalização", "1FN, 2FN e 3FN: dependências funcionais e como eliminar redundância."),
                ("sql-consultas", "SQL: consultas", "SELECT, WHERE, ORDER BY, funções de agregação e subconsultas."),
                ("sql-join-group", "SQL: JOIN e GROUP BY", "INNER e LEFT JOIN, GROUP BY com HAVING."),
                ("transacoes-acid", "Transações e ACID", "Atomicidade, consistência, isolamento, durabilidade e controle de concorrência."),
                ("nosql", "NoSQL", "Documento, chave-valor, coluna e grafo; quando cada um faz sentido."),
            ]),
        new Subject("sistemas-operacionais", "Princípios de sistemas operacionais", StudyBlock.DataAndInfrastructure,
            "Como o sistema operacional divide CPU, memória e arquivos entre os programas.",
            [
                ("processos-threads", "Processos e threads", "Estados de um processo, troca de contexto e diferença entre processo e thread."),
                ("escalonamento", "Escalonamento", "FIFO, SJF, Round Robin e prioridade; tempo de espera e de resposta."),
                ("sincronizacao-deadlock", "Sincronização e deadlock", "Condição de corrida, semáforos, as quatro condições do deadlock e como evitá-lo."),
                ("memoria-virtual", "Memória virtual", "Paginação, segmentação e substituição de páginas."),
                ("sistemas-arquivos", "Sistemas de arquivos", "Diretórios, permissões e alocação de blocos."),
            ]),
        new Subject("redes-sistemas-distribuidos", "Princípios de redes de computadores e de sistemas distribuídos", StudyBlock.DataAndInfrastructure,
            "Como os sistemas se comunicam. Endereçamento IP e sub-redes são conta: treine à mão.",
            [
                ("modelos-osi-tcpip", "Modelos OSI e TCP/IP", "As camadas, o que cada uma faz e onde ficam os protocolos."),
                ("enderecamento-ipv4", "Endereçamento IPv4 e máscara", "Notação /N, máscara decimal, faixas privadas."),
                ("sub-redes-gateway", "Sub-redes e gateway", "Endereço de rede, broadcast, número de hosts e quando o pacote vai para o gateway."),
                ("protocolos", "HTTP, DNS, TCP e UDP", "Para que serve cada um, portas comuns e diferença entre TCP e UDP."),
                ("cliente-servidor-rest", "Cliente-servidor e REST", "Métodos HTTP, códigos de status e APIs REST."),
                ("sistemas-distribuidos", "Sistemas distribuídos e microsserviços", "Escalabilidade, tolerância a falhas e o teorema CAP."),
            ]),
        new Subject("seguranca-cibernetica", "Princípios de segurança cibernética", StudyBlock.DataAndInfrastructure,
            "Proteger dados e sistemas. Liga com redes, banco de dados e LGPD.",
            [
                ("triade-cid", "Confidencialidade, integridade e disponibilidade", "A tríade CID e exemplos de ataque a cada pilar."),
                ("criptografia", "Criptografia simétrica e assimétrica", "Chave única x par de chaves, onde cada uma é usada (ex.: TLS)."),
                ("hash-assinatura", "Hash e assinatura digital", "Integridade, armazenamento de senhas e certificados."),
                ("owasp", "OWASP Top 10", "Injeção de SQL, XSS, quebra de autenticação e outras falhas comuns."),
                ("autenticacao-autorizacao", "Autenticação e autorização", "Diferença entre as duas, MFA e menor privilégio."),
            ]),
        new Subject("estatistica-analise-dados", "Princípios de estatística e análise de dados", StudyBlock.DataAndInfrastructure,
            "Ler números e gráficos com segurança. Aparece também na Formação Geral.",
            [
                ("tendencia-central", "Média, mediana e moda", "Quando cada medida representa melhor os dados."),
                ("dispersao", "Variância e desvio-padrão", "O quanto os dados se espalham em torno da média."),
                ("probabilidade", "Probabilidade", "Eventos, probabilidade condicional e independência."),
                ("leitura-graficos", "Leitura de gráficos", "Barras, linhas, setores e histogramas; armadilhas de escala."),
                ("analise-dados", "Noções de análise de dados", "Amostra x população, correlação x causalidade."),
            ]),
        new Checkpoint("checkpoint-dados-infra", "Checkpoint: questões de dados e infraestrutura", StudyBlock.DataAndInfrastructure,
            "Resolva questões de SQL, sub-redes, sistemas operacionais e segurança. Refaça as que errou depois de uma semana."),

        new Subject("processo-software", "Processo de software", StudyBlock.SoftwareEngineering,
            "Como as equipes organizam o desenvolvimento, do cascata ao ágil.",
            [
                ("modelos-processo", "Modelos de processo", "Cascata, incremental, iterativo e espiral: vantagens e riscos."),
                ("scrum", "Scrum", "Papéis, eventos e artefatos."),
                ("kanban", "Kanban", "Quadro, limite de trabalho em progresso e fluxo contínuo."),
                ("devops", "DevOps", "Cultura, automação e entrega contínua."),
            ]),
        new Subject("engenharia-requisitos", "Engenharia de requisitos", StudyBlock.SoftwareEngineering,
            "Descobrir e registrar o que o sistema deve fazer. A questão clássica é classificar requisitos.",
            [
                ("funcionais-nao-funcionais", "Requisitos funcionais x não funcionais", "O que o sistema faz x como ele deve se comportar (desempenho, segurança, usabilidade)."),
                ("levantamento", "Técnicas de levantamento", "Entrevista, questionário, workshop, observação e prototipação."),
                ("historias-usuario", "Histórias de usuário", "Formato \"como… quero… para…\" e critérios de aceite."),
                ("validacao-rastreabilidade", "Validação e rastreabilidade", "Revisões, protótipos e matriz de rastreabilidade."),
            ]),
        new Subject("analise-arquitetura-sistemas", "Análise e arquitetura de sistemas computacionais", StudyBlock.SoftwareEngineering,
            "Modelar o sistema antes de construir. UML aparece como diagrama para julgar afirmações.",
            [
                ("uml-casos-uso", "UML: casos de uso", "Ator, include x extend, generalização."),
                ("uml-classes", "UML: diagrama de classes", "Associação, agregação, composição, herança e multiplicidade."),
                ("uml-sequencia", "UML: diagrama de sequência", "Linhas de vida, mensagens síncronas e assíncronas."),
                ("arquitetura-camadas", "Arquitetura em camadas e MVC", "Separação de responsabilidades entre apresentação, negócio e dados."),
                ("estilos-arquiteturais", "Estilos arquiteturais", "Monólito, cliente-servidor, microsserviços e orientação a eventos."),
            ]),
        new Subject("interacao-humano-computador", "Interação humano-computador", StudyBlock.SoftwareEngineering,
            "Projetar interfaces fáceis de usar e acessíveis.",
            [
                ("heuristicas-nielsen", "Heurísticas de Nielsen", "As 10 heurísticas de usabilidade e exemplos de violação."),
                ("usabilidade", "Usabilidade e avaliação", "Testes com usuários, avaliação heurística e métricas."),
                ("acessibilidade", "Acessibilidade", "WCAG, leitores de tela, contraste e navegação por teclado."),
                ("prototipacao", "Prototipação", "Baixa e alta fidelidade e quando usar cada uma."),
            ]),
        new Subject("qualidade-testes", "Qualidade, verificação e validação de software", StudyBlock.SoftwareEngineering,
            "Garantir que o software faz o que deve. Valor-limite e partição de equivalência são contas que caem.",
            [
                ("niveis-teste", "Níveis de teste", "Unidade, integração, sistema e aceitação."),
                ("caixa-preta-branca", "Caixa-preta x caixa-branca", "Testar pela especificação x pelo código (cobertura de caminhos)."),
                ("particao-valor-limite", "Partição de equivalência e valor-limite", "Quantos casos de teste são necessários para uma regra."),
                ("tdd", "TDD e testes automatizados", "Ciclo vermelho-verde-refatorar e testes de regressão."),
                ("verificacao-validacao", "Verificação x validação", "Construir o produto certo x construir o produto do jeito certo."),
                ("modelos-qualidade", "Modelos de qualidade", "ISO/IEC 25010 e métricas de qualidade."),
            ]),
        new Subject("gerencia-configuracao", "Gerência de configuração", StudyBlock.SoftwareEngineering,
            "Controlar versões e mudanças do software com segurança.",
            [
                ("controle-versao", "Controle de versão com Git", "Commit, branch, merge, rebase e conflitos."),
                ("estrategias-branch", "Estratégias de branch", "Git flow, trunk-based e pull requests."),
                ("ci-cd", "Integração e entrega contínuas", "Pipeline de build, teste e deploy automatizados."),
                ("gestao-mudancas", "Gestão de mudanças e baseline", "Itens de configuração, baselines e controle de mudanças."),
            ]),
        new Subject("operacoes-manutencao", "Operações e manutenção de software", StudyBlock.SoftwareEngineering,
            "O software depois de entrar em produção.",
            [
                ("tipos-manutencao", "Tipos de manutenção", "Corretiva, adaptativa, perfectiva e preventiva."),
                ("implantacao", "Implantação", "Ambientes, deploy, rollback e containers."),
                ("monitoramento", "Monitoramento e incidentes", "Logs, métricas, alertas e resposta a incidentes."),
                ("refatoracao-divida", "Refatoração e dívida técnica", "Melhorar o código sem mudar o comportamento."),
            ]),
        new Subject("gerencia-projetos", "Gerência de projetos", StudyBlock.SoftwareEngineering,
            "Planejar e acompanhar projetos de software.",
            [
                ("escopo-cronograma-custo", "Escopo, cronograma e custo", "EAP, estimativas e a restrição tripla."),
                ("caminho-critico", "Caminho crítico", "Rede de atividades, folga e o caminho que define a duração do projeto."),
                ("riscos", "Gestão de riscos", "Identificar, analisar e responder a riscos."),
                ("pmbok", "PMBOK e partes interessadas", "Áreas de conhecimento e comunicação com stakeholders."),
            ]),
        new Checkpoint("checkpoint-engenharia", "Checkpoint: questões de engenharia de software", StudyBlock.SoftwareEngineering,
            "Resolva questões de requisitos, UML, testes e Scrum. Escreva uma resposta discursiva sobre uma decisão de arquitetura."),

        new Subject("legislacao-etica", "Legislação, normas técnicas, ética e responsabilidade socioambiental", StudyBlock.ProfessionalContext,
            "O profissional de TI e a sociedade. Costuma vir em situações-problema contextualizadas.",
            [
                ("lgpd", "LGPD", "Bases legais, dados sensíveis, controlador, operador e direitos do titular."),
                ("marco-civil", "Marco Civil da Internet", "Neutralidade de rede, guarda de registros e responsabilidade de provedores."),
                ("propriedade-intelectual", "Propriedade intelectual e licenças", "Lei de software, direito autoral e licenças livres."),
                ("etica-profissional", "Ética profissional", "Responsabilidade sobre o que se constrói e vieses em algoritmos."),
                ("ti-sustentavel", "TI e sustentabilidade", "Consumo de energia, lixo eletrônico e inclusão digital."),
            ]),
        new Subject("empreendedorismo", "Empreendedorismo", StudyBlock.ProfessionalContext,
            "Transformar ideias em produtos e negócios.",
            [
                ("modelo-negocio", "Modelo de negócio e Canvas", "Os nove blocos do Business Model Canvas."),
                ("mvp-startups", "MVP e startups", "Produto mínimo viável, validação e pivotar."),
                ("inovacao", "Inovação", "Inovação incremental x disruptiva e ecossistemas de inovação."),
            ]),

        new Subject("formacao-geral", "Formação geral tecnológica", StudyBlock.GeneralEducation,
            "As 15 questões comuns aos cursos de tecnologia: interpretação, dados e temas da atualidade.",
            [
                ("interpretacao-texto", "Interpretação de texto", "Identificar tese, argumentos e a afirmação que o texto sustenta."),
                ("graficos-tabelas", "Gráficos e tabelas", "Tirar conclusões corretas de dados apresentados visualmente."),
                ("atualidades", "Temas da atualidade", "Tecnologia e sociedade, inteligência artificial, meio ambiente, inclusão e direitos humanos."),
                ("discursiva", "Escrita da resposta discursiva", "Estrutura problema → proposta → justificativa em cerca de 15 linhas."),
            ]),
        new Checkpoint("checkpoint-simulado", "Checkpoint final: simulado completo", StudyBlock.GeneralEducation,
            "Faça um simulado de 4 horas no mesmo horário da prova. Depois, revise só o caderno de erros até o dia do ENADE."),
    ];

    public static IReadOnlyList<RoadmapNode> Nodes { get; } = Build();

    private static List<RoadmapNode> Build()
    {
        var nodes = new List<RoadmapNode>();
        var spineOrder = 0;
        var subjectNumber = 0;
        var checkpointNumber = 0;

        foreach (var item in Spine)
        {
            spineOrder++;
            switch (item)
            {
                case Subject subject:
                    subjectNumber++;
                    var subjectId = subjectNumber * 100;
                    nodes.Add(new RoadmapNode
                    {
                        Id = subjectId, Slug = subject.Slug, Title = subject.Title, Summary = subject.Summary,
                        Kind = NodeKind.Subject, Block = subject.Block, Order = spineOrder,
                    });
                    for (var i = 0; i < subject.Topics.Length; i++)
                    {
                        var topic = subject.Topics[i];
                        nodes.Add(new RoadmapNode
                        {
                            Id = subjectId + i + 1, Slug = topic.Slug, Title = topic.Title, Summary = topic.Summary,
                            Kind = NodeKind.Topic, Block = subject.Block, Order = i + 1, ParentId = subjectId,
                        });
                    }
                    break;

                case Checkpoint checkpoint:
                    checkpointNumber++;
                    nodes.Add(new RoadmapNode
                    {
                        Id = 10_000 + checkpointNumber, Slug = checkpoint.Slug, Title = checkpoint.Title, Summary = checkpoint.Summary,
                        Kind = NodeKind.Checkpoint, Block = checkpoint.Block, Order = spineOrder,
                    });
                    break;
            }
        }

        return nodes;
    }
}
