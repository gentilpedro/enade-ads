namespace EnadeAds.Study;

/// <summary>
/// Roadmap inicial da trilha ENEM, a partir da Matriz de Referência do ENEM (Inep): as quatro áreas e a redação,
/// na ordem dos dias de prova (Linguagens, Humanas e Redação no primeiro domingo; Natureza e Matemática no segundo).
/// Ids a partir de 100.000 para não cruzar com os do ENADE.
///
/// Como na <see cref="RoadmapSeed"/>: depois de publicado, o conteúdo muda pela tela, não por este arquivo.
/// </summary>
public static class EnemRoadmapSeed
{
    public const int IdBase = 100_000;

    private static readonly object[] Spine =
    [
        new RoadmapSeed.Subject("interpretacao-generos", "Leitura, interpretação e gêneros textuais", StudyBlock.Languages,
            "A base de Linguagens e de toda a prova: quase toda questão começa com um texto, um gráfico ou uma imagem.",
            [
                ("generos-textuais", "Gêneros textuais", "Reconhecer o gênero (notícia, crônica, charge, propaganda, artigo de opinião) e a função dele."),
                ("coesao-coerencia", "Coesão e coerência", "Conectivos, retomadas e o que mantém o texto com sentido."),
                ("intertextualidade", "Intertextualidade", "Paródia, paráfrase, citação e alusão entre textos."),
                ("variacao-linguistica", "Variação linguística", "Variedades regionais, sociais e de registro, sem preconceito linguístico."),
                ("funcoes-linguagem", "Funções da linguagem", "Referencial, emotiva, apelativa, poética, fática e metalinguística."),
            ]),
        new RoadmapSeed.Subject("gramatica-uso", "Gramática em uso", StudyBlock.Languages,
            "O ENEM não pede regra decorada: pede o efeito de sentido de uma escolha gramatical no texto.",
            [
                ("classes-palavras", "Classes de palavras e efeitos de sentido", "Adjetivos, advérbios e pronomes como escolhas de quem escreve."),
                ("pontuacao", "Pontuação", "Vírgula, aspas, travessão e o que cada uma muda no sentido."),
                ("figuras-linguagem", "Figuras de linguagem", "Metáfora, metonímia, ironia, hipérbole e antítese."),
                ("concordancia-regencia", "Concordância, regência e crase", "Os casos que mais aparecem em textos formais."),
            ]),
        new RoadmapSeed.Subject("literatura", "Literatura", StudyBlock.Languages,
            "Escolas literárias lidas a partir de trechos: o que o texto mostra do contexto em que foi escrito.",
            [
                ("escolas-literarias", "Escolas literárias", "Do Barroco ao Realismo: características e autores que caem."),
                ("modernismo", "Modernismo", "Semana de 22, as gerações modernistas e a ruptura com a tradição."),
                ("literatura-contemporanea", "Literatura contemporânea", "Autores atuais, literatura indígena, afro-brasileira e periférica."),
            ]),
        new RoadmapSeed.Subject("artes-corpo-tecnologia", "Artes, educação física e tecnologias da comunicação", StudyBlock.Languages,
            "A parte de Linguagens que vai além do texto verbal.",
            [
                ("artes-visuais", "Artes visuais e movimentos artísticos", "Ler obras, performances e arte urbana dentro do contexto delas."),
                ("praticas-corporais", "Práticas corporais", "Esporte, dança, lutas e saúde como fenômenos culturais."),
                ("midias-digitais", "Mídias e tecnologias digitais", "Redes sociais, fake news e linguagem da internet."),
            ]),
        new RoadmapSeed.Subject("lingua-estrangeira", "Língua estrangeira (inglês ou espanhol)", StudyBlock.Languages,
            "Cinco questões, escolhidas na inscrição. É leitura, não gramática: entender a ideia geral e o propósito do texto.",
            [
                ("leitura-estrategica", "Leitura estratégica", "Palavras cognatas, falsos cognatos e pistas do contexto."),
                ("generos-lingua-estrangeira", "Gêneros em língua estrangeira", "Tirinhas, anúncios, letras de música e notícias."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-linguagens", "Checkpoint: questões de Linguagens", StudyBlock.Languages,
            "Resolva questões de interpretação, gramática em uso e literatura com o relógio ligado: cerca de 3 minutos por questão."),

        new RoadmapSeed.Subject("historia-brasil", "História do Brasil", StudyBlock.Humanities,
            "Da colonização à Nova República, sempre ligando o passado a permanências de hoje.",
            [
                ("brasil-colonia", "Brasil Colônia", "Economia açucareira, escravidão e resistência, mineração."),
                ("brasil-imperio", "Brasil Império", "Independência, Primeiro e Segundo Reinado, abolição."),
                ("republica", "Primeira República e Era Vargas", "Coronelismo, revoltas, trabalhismo e Estado Novo."),
                ("ditadura-redemocratizacao", "Ditadura militar e redemocratização", "Golpe de 1964, AI-5, Diretas Já e a Constituição de 1988."),
            ]),
        new RoadmapSeed.Subject("historia-geral", "História geral", StudyBlock.Humanities,
            "Os grandes processos que explicam o mundo atual.",
            [
                ("antiguidade-medieval", "Antiguidade e Idade Média", "Grécia, Roma, feudalismo e a Igreja medieval."),
                ("idade-moderna", "Idade Moderna", "Grandes navegações, Reforma, absolutismo e Iluminismo."),
                ("revolucoes", "Revoluções e industrialização", "Revolução Francesa, Revolução Industrial e as ideias do século XIX."),
                ("seculo-xx", "Século XX", "Guerras mundiais, Guerra Fria, descolonização e globalização."),
            ]),
        new RoadmapSeed.Subject("geografia-fisica-ambiental", "Geografia física e questões ambientais", StudyBlock.Humanities,
            "Natureza e sociedade: clima, relevo e os impactos da ação humana.",
            [
                ("cartografia", "Cartografia", "Escala, projeções, coordenadas e leitura de mapas."),
                ("clima-biomas", "Clima e biomas brasileiros", "Tipos de clima, fenômenos climáticos e os biomas do Brasil."),
                ("relevo-hidrografia", "Relevo, solo e hidrografia", "Formação do relevo, erosão e bacias hidrográficas."),
                ("questoes-ambientais", "Questões ambientais", "Desmatamento, mudanças climáticas, acordos internacionais e sustentabilidade."),
            ]),
        new RoadmapSeed.Subject("geografia-humana", "Geografia humana e econômica", StudyBlock.Humanities,
            "População, cidades, campo e economia no Brasil e no mundo.",
            [
                ("populacao", "População e migrações", "Pirâmides etárias, transição demográfica e fluxos migratórios."),
                ("urbanizacao", "Urbanização", "Metropolização, segregação socioespacial e problemas urbanos."),
                ("espaco-agrario", "Espaço agrário", "Agronegócio, agricultura familiar, conflitos no campo e reforma agrária."),
                ("globalizacao-industria", "Globalização, indústria e energia", "Fluxos globais, blocos econômicos e matriz energética."),
            ]),
        new RoadmapSeed.Subject("filosofia-sociologia", "Filosofia e sociologia", StudyBlock.Humanities,
            "Conceitos que aparecem em trechos de autores: identificar a ideia e aplicá-la a uma situação.",
            [
                ("filosofia-antiga", "Filosofia antiga", "Pré-socráticos, Sócrates, Platão e Aristóteles."),
                ("filosofia-moderna-etica", "Filosofia moderna e ética", "Racionalismo, empirismo, contratualistas e Kant."),
                ("sociologia-classica", "Sociologia clássica", "Durkheim, Weber e Marx: fato social, ação social e classes."),
                ("cidadania-movimentos", "Cidadania e movimentos sociais", "Direitos humanos, democracia, cultura e indústria cultural."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-humanas", "Checkpoint: questões de Ciências Humanas", StudyBlock.Humanities,
            "Resolva questões de História, Geografia, Filosofia e Sociologia. Anote no caderno de erros os conceitos que confundiu."),

        new RoadmapSeed.Subject("redacao", "Redação dissertativo-argumentativa", StudyBlock.Essay,
            "Um texto de até 30 linhas sobre um tema social, avaliado em cinco competências de 200 pontos cada.",
            [
                ("competencias-redacao", "As cinco competências", "Norma culta, tema e tipo textual, argumentação, coesão e proposta de intervenção."),
                ("estrutura-texto", "Estrutura do texto", "Introdução com tese, dois parágrafos de desenvolvimento e conclusão."),
                ("repertorio", "Repertório sociocultural", "Dados, leis, obras e fatos históricos usados a favor da tese, não só citados."),
                ("proposta-intervencao", "Proposta de intervenção", "Agente, ação, meio, finalidade e detalhamento, respeitando os direitos humanos."),
                ("erros-zeram", "O que zera a redação", "Fuga ao tema, texto com menos de 7 linhas, parte desconectada e outros casos do edital."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-redacao", "Checkpoint: uma redação completa", StudyBlock.Essay,
            "Escreva uma redação sobre um tema de prova anterior em até 1 hora e confira cada competência."),

        new RoadmapSeed.Subject("biologia", "Biologia", StudyBlock.NaturalSciences,
            "A disciplina com mais questões em Natureza, quase sempre ligada a saúde e meio ambiente.",
            [
                ("ecologia", "Ecologia", "Cadeias alimentares, ciclos biogeoquímicos, relações ecológicas e impactos ambientais."),
                ("genetica", "Genética", "Leis de Mendel, heredogramas, DNA e biotecnologia."),
                ("fisiologia-humana", "Fisiologia humana e saúde", "Sistemas do corpo, vacinas, doenças e saneamento."),
                ("citologia", "Citologia", "Célula, organelas, membrana e divisão celular."),
                ("evolucao", "Evolução", "Darwin, seleção natural e especiação."),
            ]),
        new RoadmapSeed.Subject("quimica", "Química", StudyBlock.NaturalSciences,
            "Contas e conceitos aplicados ao cotidiano: combustíveis, alimentos, poluição.",
            [
                ("estequiometria", "Estequiometria", "Mol, massa molar e proporções numa reação."),
                ("solucoes", "Soluções e concentração", "Concentração comum, molaridade, diluição e mistura."),
                ("quimica-organica", "Química orgânica", "Funções orgânicas, polímeros e combustíveis."),
                ("fisico-quimica", "Termoquímica, cinética e equilíbrio", "Energia das reações, velocidade e pH."),
                ("quimica-ambiental", "Química ambiental", "Chuva ácida, efeito estufa, tratamento de água e reciclagem."),
            ]),
        new RoadmapSeed.Subject("fisica", "Física", StudyBlock.NaturalSciences,
            "Fenômenos do dia a dia: energia em casa, movimento, ondas e eletricidade.",
            [
                ("mecanica", "Mecânica", "Velocidade, aceleração, leis de Newton, trabalho e energia."),
                ("eletricidade", "Eletricidade", "Circuitos, potência, consumo de energia e conta de luz."),
                ("ondas-optica", "Ondas e óptica", "Som, luz, espelhos, lentes e fenômenos ondulatórios."),
                ("termologia", "Termologia", "Calor, temperatura, dilatação e máquinas térmicas."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-natureza", "Checkpoint: questões de Ciências da Natureza", StudyBlock.NaturalSciences,
            "Resolva questões de Biologia, Química e Física. Refaça as contas que errou sem olhar a resolução."),

        new RoadmapSeed.Subject("matematica-basica", "Matemática básica e proporcionalidade", StudyBlock.Mathematics,
            "O que mais cai em Matemática: razão, proporção e porcentagem em situações do cotidiano.",
            [
                ("razao-proporcao", "Razão, proporção e regra de três", "Grandezas direta e inversamente proporcionais, escalas."),
                ("porcentagem", "Porcentagem e juros", "Aumentos, descontos sucessivos e juros simples e compostos."),
                ("unidades-medida", "Unidades de medida", "Conversões de comprimento, área, volume e tempo."),
            ]),
        new RoadmapSeed.Subject("estatistica-probabilidade", "Estatística e probabilidade", StudyBlock.Mathematics,
            "Ler gráficos e tabelas e calcular medidas: presença garantida na prova.",
            [
                ("medidas-tendencia", "Média, mediana e moda", "Quando usar cada medida e como calcular com tabelas de frequência."),
                ("graficos-tabelas-enem", "Gráficos e tabelas", "Barras, linhas, setores e o que cada gráfico permite concluir."),
                ("probabilidade-enem", "Probabilidade", "Casos favoráveis sobre possíveis, eventos independentes e complementares."),
                ("analise-combinatoria", "Análise combinatória", "Princípio fundamental da contagem, arranjos e combinações."),
            ]),
        new RoadmapSeed.Subject("geometria", "Geometria", StudyBlock.Mathematics,
            "Áreas, volumes e formas em embalagens, terrenos e construções.",
            [
                ("geometria-plana", "Geometria plana", "Áreas e perímetros, semelhança de triângulos e Pitágoras."),
                ("geometria-espacial", "Geometria espacial", "Volumes de prismas, cilindros, cones e esferas; planificações."),
                ("trigonometria", "Trigonometria", "Seno, cosseno e tangente no triângulo retângulo."),
            ]),
        new RoadmapSeed.Subject("funcoes-algebra", "Funções e álgebra", StudyBlock.Mathematics,
            "Modelar situações com funções e ler o gráfico delas.",
            [
                ("funcao-afim-quadratica", "Funções do 1º e do 2º grau", "Gráfico, raízes, vértice e problemas de máximo e mínimo."),
                ("exponencial-logaritmo", "Exponencial e logaritmo", "Crescimento, decaimento e escalas logarítmicas."),
                ("sequencias", "Progressões aritmética e geométrica", "Termo geral e soma dos termos."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-simulado-enem", "Checkpoint final: simulado de um dia de prova", StudyBlock.Mathematics,
            "Faça um simulado completo no horário da prova (início às 13h30). Depois, revise o caderno de erros até o dia do ENEM."),
    ];

    public static IReadOnlyList<RoadmapNode> Nodes { get; } = RoadmapSeed.Build(Spine, Tracks.Enem, IdBase);
}
