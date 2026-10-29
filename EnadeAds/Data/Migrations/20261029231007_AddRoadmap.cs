using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadmapNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Block = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapNodes_RoadmapNodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RoadmapNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapProgress",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapProgress", x => new { x.UserId, x.NodeId });
                    table.ForeignKey(
                        name: "FK_RoadmapProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadmapProgress_RoadmapNodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "RoadmapNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RoadmapNodes",
                columns: new[] { "Id", "Block", "Kind", "Order", "ParentId", "Slug", "Summary", "Title" },
                values: new object[,]
                {
                    { 100, 0, 0, 1, null, "logica-matematica", "Base para ler enunciados formais, condições de algoritmos e consultas. Cai em questões de raciocínio e em pseudocódigo.", "Lógica matemática e teoria dos conjuntos" },
                    { 200, 0, 0, 2, null, "arquitetura-computadores", "Como o computador representa e processa dados. Conversão de bases é ponto garantido com treino.", "Princípios de arquitetura e organização de computadores" },
                    { 300, 0, 0, 3, null, "algoritmos-programacao", "Ler e escrever algoritmos. A questão clássica do ENADE é descobrir o que um pseudocódigo imprime.", "Algoritmos e programação" },
                    { 400, 0, 0, 4, null, "estruturas-dados", "Como organizar dados para cada problema e quanto custa cada operação.", "Estruturas de dados" },
                    { 500, 0, 0, 5, null, "orientacao-objetos", "Modelar sistemas com classes e objetos. Cai muito como código que imprime algo com herança e polimorfismo.", "Orientação a objetos" },
                    { 600, 1, 0, 7, null, "banco-de-dados", "Modelagem e consultas. SQL aparece quase sempre: ler uma consulta e dizer o resultado.", "Banco de dados" },
                    { 700, 1, 0, 8, null, "sistemas-operacionais", "Como o sistema operacional divide CPU, memória e arquivos entre os programas.", "Princípios de sistemas operacionais" },
                    { 800, 1, 0, 9, null, "redes-sistemas-distribuidos", "Como os sistemas se comunicam. Endereçamento IP e sub-redes são conta: treine à mão.", "Princípios de redes de computadores e de sistemas distribuídos" },
                    { 900, 1, 0, 10, null, "seguranca-cibernetica", "Proteger dados e sistemas. Liga com redes, banco de dados e LGPD.", "Princípios de segurança cibernética" },
                    { 1000, 1, 0, 11, null, "estatistica-analise-dados", "Ler números e gráficos com segurança. Aparece também na Formação Geral.", "Princípios de estatística e análise de dados" },
                    { 1100, 2, 0, 13, null, "processo-software", "Como as equipes organizam o desenvolvimento, do cascata ao ágil.", "Processo de software" },
                    { 1200, 2, 0, 14, null, "engenharia-requisitos", "Descobrir e registrar o que o sistema deve fazer. A questão clássica é classificar requisitos.", "Engenharia de requisitos" },
                    { 1300, 2, 0, 15, null, "analise-arquitetura-sistemas", "Modelar o sistema antes de construir. UML aparece como diagrama para julgar afirmações.", "Análise e arquitetura de sistemas computacionais" },
                    { 1400, 2, 0, 16, null, "interacao-humano-computador", "Projetar interfaces fáceis de usar e acessíveis.", "Interação humano-computador" },
                    { 1500, 2, 0, 17, null, "qualidade-testes", "Garantir que o software faz o que deve. Valor-limite e partição de equivalência são contas que caem.", "Qualidade, verificação e validação de software" },
                    { 1600, 2, 0, 18, null, "gerencia-configuracao", "Controlar versões e mudanças do software com segurança.", "Gerência de configuração" },
                    { 1700, 2, 0, 19, null, "operacoes-manutencao", "O software depois de entrar em produção.", "Operações e manutenção de software" },
                    { 1800, 2, 0, 20, null, "gerencia-projetos", "Planejar e acompanhar projetos de software.", "Gerência de projetos" },
                    { 1900, 3, 0, 22, null, "legislacao-etica", "O profissional de TI e a sociedade. Costuma vir em situações-problema contextualizadas.", "Legislação, normas técnicas, ética e responsabilidade socioambiental" },
                    { 2000, 3, 0, 23, null, "empreendedorismo", "Transformar ideias em produtos e negócios.", "Empreendedorismo" },
                    { 2100, 4, 0, 24, null, "formacao-geral", "As 15 questões comuns aos cursos de tecnologia: interpretação, dados e temas da atualidade.", "Formação geral tecnológica" },
                    { 10001, 0, 2, 6, null, "checkpoint-fundamentos", "Antes de seguir, resolva questões de lógica, conversão de bases, pseudocódigo, estruturas de dados e orientação a objetos. Anote os erros no caderno de erros.", "Checkpoint: questões de fundamentos" },
                    { 10002, 1, 2, 12, null, "checkpoint-dados-infra", "Resolva questões de SQL, sub-redes, sistemas operacionais e segurança. Refaça as que errou depois de uma semana.", "Checkpoint: questões de dados e infraestrutura" },
                    { 10003, 2, 2, 21, null, "checkpoint-engenharia", "Resolva questões de requisitos, UML, testes e Scrum. Escreva uma resposta discursiva sobre uma decisão de arquitetura.", "Checkpoint: questões de engenharia de software" },
                    { 10004, 4, 2, 25, null, "checkpoint-simulado", "Faça um simulado de 4 horas no mesmo horário da prova. Depois, revise só o caderno de erros até o dia do ENADE.", "Checkpoint final: simulado completo" },
                    { 101, 0, 1, 1, 100, "proposicoes-tabela-verdade", "Conectivos (e, ou, não, se-então, se e somente se) e como montar a tabela-verdade de uma expressão.", "Proposições e tabela-verdade" },
                    { 102, 0, 1, 2, 100, "equivalencias-logicas", "Leis de De Morgan, contrapositiva e negação de condicionais, muito usadas para simplificar condições.", "Equivalências lógicas" },
                    { 103, 0, 1, 3, 100, "quantificadores", "Para todo e existe: como negar frases com quantificadores.", "Quantificadores" },
                    { 104, 0, 1, 4, 100, "conjuntos-operacoes", "União, interseção, diferença, complemento, produto cartesiano e diagramas de Venn.", "Conjuntos e operações" },
                    { 105, 0, 1, 5, 100, "relacoes-funcoes", "Domínio, imagem, funções injetoras, sobrejetoras e bijetoras.", "Relações e funções" },
                    { 201, 0, 1, 1, 200, "conversao-bases", "Decimal, binário, octal e hexadecimal nas duas direções, sem calculadora.", "Sistemas de numeração e conversão de bases" },
                    { 202, 0, 1, 2, 200, "aritmetica-binaria", "Soma e subtração em binário, complemento de dois e overflow.", "Aritmética binária" },
                    { 203, 0, 1, 3, 200, "representacao-dados", "Bits, bytes, KB x KiB, caracteres (ASCII, Unicode) e ponto flutuante.", "Representação de dados" },
                    { 204, 0, 1, 4, 200, "cpu-memoria-barramentos", "Ciclo de busca e execução, registradores, ULA e unidade de controle.", "CPU, memória e barramentos" },
                    { 205, 0, 1, 5, 200, "hierarquia-memoria", "Registradores, cache, RAM e armazenamento: velocidade, custo e localidade.", "Hierarquia de memória" },
                    { 301, 0, 1, 1, 300, "variaveis-tipos", "Tipos primitivos, precedência de operadores e divisão inteira x real.", "Variáveis, tipos e operadores" },
                    { 302, 0, 1, 2, 300, "estruturas-controle", "Condicionais e laços (enquanto, para, repita) e suas condições de parada.", "Estruturas de controle" },
                    { 303, 0, 1, 3, 300, "funcoes-recursao", "Parâmetros por valor e por referência, caso base e pilha de chamadas.", "Funções e recursão" },
                    { 304, 0, 1, 4, 300, "vetores-matrizes", "Índices, percursos e operações comuns em arrays.", "Vetores e matrizes" },
                    { 305, 0, 1, 5, 300, "rastreamento-pseudocodigo", "Simular a execução linha a linha anotando o valor de cada variável.", "Rastreamento de pseudocódigo" },
                    { 401, 0, 1, 1, 400, "pilhas", "LIFO: push e pop, usos em desfazer, chamadas de função e avaliação de expressões.", "Pilhas" },
                    { 402, 0, 1, 2, 400, "filas", "FIFO: enqueue e dequeue, filas circulares e de prioridade.", "Filas" },
                    { 403, 0, 1, 3, 400, "listas-encadeadas", "Simples, duplas e circulares; inserção e remoção sem deslocar elementos.", "Listas encadeadas" },
                    { 404, 0, 1, 4, 400, "arvores", "Percursos (pré, em e pós-ordem), inserção e busca em BST, balanceamento.", "Árvores binárias e de busca" },
                    { 405, 0, 1, 5, 400, "tabelas-hash", "Função de hash, colisões e custo médio de busca.", "Tabelas hash" },
                    { 406, 0, 1, 6, 400, "grafos", "Representações, busca em largura e em profundidade, caminho mínimo.", "Grafos" },
                    { 407, 0, 1, 7, 400, "complexidade", "Notação Big-O e comparação entre O(1), O(log n), O(n), O(n log n) e O(n²).", "Complexidade de algoritmos" },
                    { 408, 0, 1, 8, 400, "ordenacao-busca", "Bubble, insertion, merge e quick sort; busca linear x binária.", "Ordenação e busca" },
                    { 501, 0, 1, 1, 500, "classes-objetos", "Estado, comportamento, instância e construtor da superclasse.", "Classes, objetos e construtores" },
                    { 502, 0, 1, 2, 500, "encapsulamento", "Modificadores de acesso e por que esconder o estado interno.", "Encapsulamento" },
                    { 503, 0, 1, 3, 500, "heranca", "Reuso e especialização; quando preferir composição.", "Herança" },
                    { 504, 0, 1, 4, 500, "polimorfismo", "Sobrescrita x sobrecarga e ligação dinâmica.", "Polimorfismo" },
                    { 505, 0, 1, 5, 500, "abstrata-interface", "O que cada uma pode ter e quando usar.", "Classe abstrata x interface" },
                    { 506, 0, 1, 6, 500, "solid", "Responsabilidade única, aberto/fechado, Liskov, segregação de interfaces e inversão de dependência.", "Princípios SOLID" },
                    { 507, 0, 1, 7, 500, "padroes-projeto", "Singleton, Factory, Strategy, Observer e Adapter.", "Padrões de projeto" },
                    { 601, 1, 1, 1, 600, "modelo-er", "Entidades, atributos, relacionamentos e cardinalidade.", "Modelo entidade-relacionamento" },
                    { 602, 1, 1, 2, 600, "modelo-relacional", "Tabelas, chave primária, chave estrangeira e integridade referencial.", "Modelo relacional e chaves" },
                    { 603, 1, 1, 3, 600, "normalizacao", "1FN, 2FN e 3FN: dependências funcionais e como eliminar redundância.", "Normalização" },
                    { 604, 1, 1, 4, 600, "sql-consultas", "SELECT, WHERE, ORDER BY, funções de agregação e subconsultas.", "SQL: consultas" },
                    { 605, 1, 1, 5, 600, "sql-join-group", "INNER e LEFT JOIN, GROUP BY com HAVING.", "SQL: JOIN e GROUP BY" },
                    { 606, 1, 1, 6, 600, "transacoes-acid", "Atomicidade, consistência, isolamento, durabilidade e controle de concorrência.", "Transações e ACID" },
                    { 607, 1, 1, 7, 600, "nosql", "Documento, chave-valor, coluna e grafo; quando cada um faz sentido.", "NoSQL" },
                    { 701, 1, 1, 1, 700, "processos-threads", "Estados de um processo, troca de contexto e diferença entre processo e thread.", "Processos e threads" },
                    { 702, 1, 1, 2, 700, "escalonamento", "FIFO, SJF, Round Robin e prioridade; tempo de espera e de resposta.", "Escalonamento" },
                    { 703, 1, 1, 3, 700, "sincronizacao-deadlock", "Condição de corrida, semáforos, as quatro condições do deadlock e como evitá-lo.", "Sincronização e deadlock" },
                    { 704, 1, 1, 4, 700, "memoria-virtual", "Paginação, segmentação e substituição de páginas.", "Memória virtual" },
                    { 705, 1, 1, 5, 700, "sistemas-arquivos", "Diretórios, permissões e alocação de blocos.", "Sistemas de arquivos" },
                    { 801, 1, 1, 1, 800, "modelos-osi-tcpip", "As camadas, o que cada uma faz e onde ficam os protocolos.", "Modelos OSI e TCP/IP" },
                    { 802, 1, 1, 2, 800, "enderecamento-ipv4", "Notação /N, máscara decimal, faixas privadas.", "Endereçamento IPv4 e máscara" },
                    { 803, 1, 1, 3, 800, "sub-redes-gateway", "Endereço de rede, broadcast, número de hosts e quando o pacote vai para o gateway.", "Sub-redes e gateway" },
                    { 804, 1, 1, 4, 800, "protocolos", "Para que serve cada um, portas comuns e diferença entre TCP e UDP.", "HTTP, DNS, TCP e UDP" },
                    { 805, 1, 1, 5, 800, "cliente-servidor-rest", "Métodos HTTP, códigos de status e APIs REST.", "Cliente-servidor e REST" },
                    { 806, 1, 1, 6, 800, "sistemas-distribuidos", "Escalabilidade, tolerância a falhas e o teorema CAP.", "Sistemas distribuídos e microsserviços" },
                    { 901, 1, 1, 1, 900, "triade-cid", "A tríade CID e exemplos de ataque a cada pilar.", "Confidencialidade, integridade e disponibilidade" },
                    { 902, 1, 1, 2, 900, "criptografia", "Chave única x par de chaves, onde cada uma é usada (ex.: TLS).", "Criptografia simétrica e assimétrica" },
                    { 903, 1, 1, 3, 900, "hash-assinatura", "Integridade, armazenamento de senhas e certificados.", "Hash e assinatura digital" },
                    { 904, 1, 1, 4, 900, "owasp", "Injeção de SQL, XSS, quebra de autenticação e outras falhas comuns.", "OWASP Top 10" },
                    { 905, 1, 1, 5, 900, "autenticacao-autorizacao", "Diferença entre as duas, MFA e menor privilégio.", "Autenticação e autorização" },
                    { 1001, 1, 1, 1, 1000, "tendencia-central", "Quando cada medida representa melhor os dados.", "Média, mediana e moda" },
                    { 1002, 1, 1, 2, 1000, "dispersao", "O quanto os dados se espalham em torno da média.", "Variância e desvio-padrão" },
                    { 1003, 1, 1, 3, 1000, "probabilidade", "Eventos, probabilidade condicional e independência.", "Probabilidade" },
                    { 1004, 1, 1, 4, 1000, "leitura-graficos", "Barras, linhas, setores e histogramas; armadilhas de escala.", "Leitura de gráficos" },
                    { 1005, 1, 1, 5, 1000, "analise-dados", "Amostra x população, correlação x causalidade.", "Noções de análise de dados" },
                    { 1101, 2, 1, 1, 1100, "modelos-processo", "Cascata, incremental, iterativo e espiral: vantagens e riscos.", "Modelos de processo" },
                    { 1102, 2, 1, 2, 1100, "scrum", "Papéis, eventos e artefatos.", "Scrum" },
                    { 1103, 2, 1, 3, 1100, "kanban", "Quadro, limite de trabalho em progresso e fluxo contínuo.", "Kanban" },
                    { 1104, 2, 1, 4, 1100, "devops", "Cultura, automação e entrega contínua.", "DevOps" },
                    { 1201, 2, 1, 1, 1200, "funcionais-nao-funcionais", "O que o sistema faz x como ele deve se comportar (desempenho, segurança, usabilidade).", "Requisitos funcionais x não funcionais" },
                    { 1202, 2, 1, 2, 1200, "levantamento", "Entrevista, questionário, workshop, observação e prototipação.", "Técnicas de levantamento" },
                    { 1203, 2, 1, 3, 1200, "historias-usuario", "Formato \"como… quero… para…\" e critérios de aceite.", "Histórias de usuário" },
                    { 1204, 2, 1, 4, 1200, "validacao-rastreabilidade", "Revisões, protótipos e matriz de rastreabilidade.", "Validação e rastreabilidade" },
                    { 1301, 2, 1, 1, 1300, "uml-casos-uso", "Ator, include x extend, generalização.", "UML: casos de uso" },
                    { 1302, 2, 1, 2, 1300, "uml-classes", "Associação, agregação, composição, herança e multiplicidade.", "UML: diagrama de classes" },
                    { 1303, 2, 1, 3, 1300, "uml-sequencia", "Linhas de vida, mensagens síncronas e assíncronas.", "UML: diagrama de sequência" },
                    { 1304, 2, 1, 4, 1300, "arquitetura-camadas", "Separação de responsabilidades entre apresentação, negócio e dados.", "Arquitetura em camadas e MVC" },
                    { 1305, 2, 1, 5, 1300, "estilos-arquiteturais", "Monólito, cliente-servidor, microsserviços e orientação a eventos.", "Estilos arquiteturais" },
                    { 1401, 2, 1, 1, 1400, "heuristicas-nielsen", "As 10 heurísticas de usabilidade e exemplos de violação.", "Heurísticas de Nielsen" },
                    { 1402, 2, 1, 2, 1400, "usabilidade", "Testes com usuários, avaliação heurística e métricas.", "Usabilidade e avaliação" },
                    { 1403, 2, 1, 3, 1400, "acessibilidade", "WCAG, leitores de tela, contraste e navegação por teclado.", "Acessibilidade" },
                    { 1404, 2, 1, 4, 1400, "prototipacao", "Baixa e alta fidelidade e quando usar cada uma.", "Prototipação" },
                    { 1501, 2, 1, 1, 1500, "niveis-teste", "Unidade, integração, sistema e aceitação.", "Níveis de teste" },
                    { 1502, 2, 1, 2, 1500, "caixa-preta-branca", "Testar pela especificação x pelo código (cobertura de caminhos).", "Caixa-preta x caixa-branca" },
                    { 1503, 2, 1, 3, 1500, "particao-valor-limite", "Quantos casos de teste são necessários para uma regra.", "Partição de equivalência e valor-limite" },
                    { 1504, 2, 1, 4, 1500, "tdd", "Ciclo vermelho-verde-refatorar e testes de regressão.", "TDD e testes automatizados" },
                    { 1505, 2, 1, 5, 1500, "verificacao-validacao", "Construir o produto certo x construir o produto do jeito certo.", "Verificação x validação" },
                    { 1506, 2, 1, 6, 1500, "modelos-qualidade", "ISO/IEC 25010 e métricas de qualidade.", "Modelos de qualidade" },
                    { 1601, 2, 1, 1, 1600, "controle-versao", "Commit, branch, merge, rebase e conflitos.", "Controle de versão com Git" },
                    { 1602, 2, 1, 2, 1600, "estrategias-branch", "Git flow, trunk-based e pull requests.", "Estratégias de branch" },
                    { 1603, 2, 1, 3, 1600, "ci-cd", "Pipeline de build, teste e deploy automatizados.", "Integração e entrega contínuas" },
                    { 1604, 2, 1, 4, 1600, "gestao-mudancas", "Itens de configuração, baselines e controle de mudanças.", "Gestão de mudanças e baseline" },
                    { 1701, 2, 1, 1, 1700, "tipos-manutencao", "Corretiva, adaptativa, perfectiva e preventiva.", "Tipos de manutenção" },
                    { 1702, 2, 1, 2, 1700, "implantacao", "Ambientes, deploy, rollback e containers.", "Implantação" },
                    { 1703, 2, 1, 3, 1700, "monitoramento", "Logs, métricas, alertas e resposta a incidentes.", "Monitoramento e incidentes" },
                    { 1704, 2, 1, 4, 1700, "refatoracao-divida", "Melhorar o código sem mudar o comportamento.", "Refatoração e dívida técnica" },
                    { 1801, 2, 1, 1, 1800, "escopo-cronograma-custo", "EAP, estimativas e a restrição tripla.", "Escopo, cronograma e custo" },
                    { 1802, 2, 1, 2, 1800, "caminho-critico", "Rede de atividades, folga e o caminho que define a duração do projeto.", "Caminho crítico" },
                    { 1803, 2, 1, 3, 1800, "riscos", "Identificar, analisar e responder a riscos.", "Gestão de riscos" },
                    { 1804, 2, 1, 4, 1800, "pmbok", "Áreas de conhecimento e comunicação com stakeholders.", "PMBOK e partes interessadas" },
                    { 1901, 3, 1, 1, 1900, "lgpd", "Bases legais, dados sensíveis, controlador, operador e direitos do titular.", "LGPD" },
                    { 1902, 3, 1, 2, 1900, "marco-civil", "Neutralidade de rede, guarda de registros e responsabilidade de provedores.", "Marco Civil da Internet" },
                    { 1903, 3, 1, 3, 1900, "propriedade-intelectual", "Lei de software, direito autoral e licenças livres.", "Propriedade intelectual e licenças" },
                    { 1904, 3, 1, 4, 1900, "etica-profissional", "Responsabilidade sobre o que se constrói e vieses em algoritmos.", "Ética profissional" },
                    { 1905, 3, 1, 5, 1900, "ti-sustentavel", "Consumo de energia, lixo eletrônico e inclusão digital.", "TI e sustentabilidade" },
                    { 2001, 3, 1, 1, 2000, "modelo-negocio", "Os nove blocos do Business Model Canvas.", "Modelo de negócio e Canvas" },
                    { 2002, 3, 1, 2, 2000, "mvp-startups", "Produto mínimo viável, validação e pivotar.", "MVP e startups" },
                    { 2003, 3, 1, 3, 2000, "inovacao", "Inovação incremental x disruptiva e ecossistemas de inovação.", "Inovação" },
                    { 2101, 4, 1, 1, 2100, "interpretacao-texto", "Identificar tese, argumentos e a afirmação que o texto sustenta.", "Interpretação de texto" },
                    { 2102, 4, 1, 2, 2100, "graficos-tabelas", "Tirar conclusões corretas de dados apresentados visualmente.", "Gráficos e tabelas" },
                    { 2103, 4, 1, 3, 2100, "atualidades", "Tecnologia e sociedade, inteligência artificial, meio ambiente, inclusão e direitos humanos.", "Temas da atualidade" },
                    { 2104, 4, 1, 4, 2100, "discursiva", "Estrutura problema → proposta → justificativa em cerca de 15 linhas.", "Escrita da resposta discursiva" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodes_ParentId",
                table: "RoadmapNodes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapNodes_Slug",
                table: "RoadmapNodes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapProgress_NodeId",
                table: "RoadmapProgress",
                column: "NodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoadmapProgress");

            migrationBuilder.DropTable(
                name: "RoadmapNodes");
        }
    }
}
