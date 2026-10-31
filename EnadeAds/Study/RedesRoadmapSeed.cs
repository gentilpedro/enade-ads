namespace EnadeAds.Study;

/// <summary>
/// Roadmap inicial da trilha ENADE Redes de Computadores: os 20 objetos de conhecimento da Portaria Inep nº 172/2026
/// (Art. 6º) mais a Formação Geral, na ordem sugerida de estudo. Ids a partir de 200.000 para não cruzar com as outras trilhas.
///
/// Como na <see cref="RoadmapSeed"/>: depois de publicado, o conteúdo muda pela tela, não por este arquivo.
/// </summary>
public static class RedesRoadmapSeed
{
    public const int IdBase = 200_000;

    private static readonly object[] Spine =
    [
        new RoadmapSeed.Subject("fundamentos-redes", "Fundamentos, componentes e topologias de redes", StudyBlock.Fundamentals,
            "O vocabulário de toda a prova: o que é cada equipamento, como ele se liga aos outros e que tipo de rede isso forma.",
            [
                ("classificacao-redes", "Classificação das redes", "PAN, LAN, MAN e WAN; redes cliente-servidor e ponto a ponto."),
                ("topologias", "Topologias física e lógica", "Barramento, anel, estrela, malha e árvore: vantagens, pontos únicos de falha e o que muda entre o físico e o lógico."),
                ("equipamentos", "Equipamentos de rede", "Placa de rede, hub, switch, roteador, access point e firewall: em que camada cada um trabalha."),
                ("dominios-colisao-broadcast", "Domínios de colisão e de broadcast", "Quantos domínios cada equipamento separa; questão clássica com diagrama."),
                ("comutacao", "Comutação de circuitos e de pacotes", "Como os dados atravessam a rede e o que isso muda em atraso e aproveitamento."),
            ]),
        new RoadmapSeed.Subject("comunicacao-dados", "Comunicação e transmissão de dados", StudyBlock.Fundamentals,
            "A camada física: sinais, meios e as contas de taxa e atraso que aparecem nas questões.",
            [
                ("sinais-modulacao", "Sinais, codificação e modulação", "Analógico x digital, codificação de linha (NRZ, Manchester) e modulação (ASK, FSK, PSK, QAM)."),
                ("meios-transmissao", "Meios de transmissão", "Par trançado, coaxial, fibra monomodo e multimodo e rádio: alcance, banda e interferência."),
                ("banda-taxa", "Largura de banda, taxa e vazão", "Nyquist e Shannon, bits x bytes e a diferença entre taxa nominal e vazão real."),
                ("atrasos", "Atrasos e desempenho", "Atraso de transmissão, propagação, fila e processamento; RTT e jitter."),
                ("multiplexacao-erros", "Multiplexação e detecção de erros", "FDM, TDM e WDM; paridade, CRC e checksum."),
            ]),
        new RoadmapSeed.Subject("modelos-referencia", "Modelos de referência", StudyBlock.Fundamentals,
            "OSI e TCP/IP lado a lado. Quase toda questão de protocolo começa por saber em que camada ele está.",
            [
                ("modelo-osi", "Modelo OSI", "As sete camadas, a função de cada uma e exemplos de protocolos e equipamentos."),
                ("modelo-tcp-ip", "Modelo TCP/IP", "As quatro (ou cinco) camadas e como elas se comparam às do OSI."),
                ("encapsulamento-pdu", "Encapsulamento e PDUs", "Mensagem, segmento, pacote, quadro e bit: o que cada camada acrescenta."),
                ("servicos-camadas", "Serviços entre camadas", "Orientado e não orientado à conexão, confiável e não confiável."),
            ]),
        new RoadmapSeed.Subject("cabeamento-estruturado", "Normas e padrões de cabeamento estruturado", StudyBlock.Fundamentals,
            "Normas, categorias de cabo e limites de distância. Rende pontos com memorização bem feita.",
            [
                ("normas-cabeamento", "Normas", "ANSI/TIA-568, ISO/IEC 11801 e NBR 14565: o que cada uma define."),
                ("subsistemas", "Subsistemas do cabeamento", "Entrada do edifício, sala de equipamentos, backbone, cabeamento horizontal e área de trabalho."),
                ("categorias-cabos", "Categorias e conectores", "Cat5e, Cat6, Cat6A e Cat8; RJ-45, padrões T568A e T568B, cabo direto e cruzado."),
                ("limites-certificacao", "Limites e certificação", "Os 90 m + 10 m do horizontal, fibra no backbone e os testes de certificação."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-fundamentos-redes", "Checkpoint: questões de fundamentos", StudyBlock.Fundamentals,
            "Antes de seguir, resolva questões de topologias, domínios de colisão, camadas do OSI, contas de taxa e atraso e cabeamento. Anote os erros no caderno de erros."),

        new RoadmapSeed.Subject("tcp-ip", "Arquitetura TCP/IP e seus protocolos", StudyBlock.NetworkTechnologies,
            "O centro da prova. Sub-redes e o comportamento do TCP aparecem em quase toda edição.",
            [
                ("ipv4-enderecamento", "Endereçamento IPv4", "Classes, máscara, notação CIDR, endereços privados e especiais."),
                ("sub-redes", "Sub-redes e VLSM", "Calcular rede, broadcast, faixa de hosts e quantidade de sub-redes sem calculadora."),
                ("ipv6", "IPv6", "Formato, abreviação, tipos de endereço, autoconfiguração e convivência com o IPv4."),
                ("arp-icmp-nat", "ARP, ICMP e NAT", "Resolução de endereço, ping e traceroute, tradução de endereços e PAT."),
                ("tcp-udp", "TCP e UDP", "Portas, three-way handshake, janela deslizante, controle de fluxo e de congestionamento."),
                ("protocolos-aplicacao", "Protocolos de aplicação", "HTTP/HTTPS, DNS, DHCP, SMTP, POP3, IMAP, FTP e SSH, com as portas de cada um."),
            ]),
        new RoadmapSeed.Subject("roteamento", "Algoritmos e protocolos de roteamento", StudyBlock.NetworkTechnologies,
            "Como o roteador escolhe o caminho. Cai como tabela de roteamento para interpretar ou rede para completar.",
            [
                ("tabela-roteamento", "Tabela de roteamento", "Rotas diretas, estáticas e padrão; prefixo mais longo e distância administrativa."),
                ("vetor-distancia", "Vetor de distância", "Bellman-Ford, RIP, contagem ao infinito, split horizon e poison reverse."),
                ("estado-enlace", "Estado de enlace", "Dijkstra, OSPF, áreas e custo dos enlaces."),
                ("bgp-sistemas-autonomos", "BGP e sistemas autônomos", "Roteamento interno x externo e como a internet se interliga."),
            ]),
        new RoadmapSeed.Subject("ieee-802", "Padrões IEEE 802.x", StudyBlock.NetworkTechnologies,
            "As redes locais por dentro: Ethernet, switches e VLANs.",
            [
                ("ethernet", "Ethernet (802.3)", "Quadro Ethernet, endereço MAC, CSMA/CD e as velocidades do Fast ao 10 Gigabit."),
                ("switching", "Comutação na camada 2", "Aprendizado de MAC, tabela CAM, full duplex e encaminhamento."),
                ("vlan-trunk", "VLANs e trunk (802.1Q)", "Segmentar a LAN, marcação de quadros e roteamento entre VLANs."),
                ("stp", "Spanning Tree (802.1D/802.1w)", "Por que laços derrubam a rede e como o STP os bloqueia."),
                ("agregacao-poe-8021x", "Agregação de enlaces, PoE e 802.1X", "LACP (802.3ad), alimentação pelo cabo e controle de acesso por porta."),
            ]),
        new RoadmapSeed.Subject("redes-sem-fio", "Redes sem fio", StudyBlock.NetworkTechnologies,
            "Wi-Fi e as outras tecnologias sem fio, com foco em padrão, alcance e segurança.",
            [
                ("padroes-80211", "Padrões 802.11", "802.11a/b/g/n/ac/ax (Wi-Fi 4, 5, 6 e 6E): faixas, canais e taxas."),
                ("csma-ca", "Acesso ao meio no Wi-Fi", "CSMA/CA, RTS/CTS e o problema do terminal oculto."),
                ("seguranca-wifi", "Segurança no Wi-Fi", "WEP, WPA, WPA2 e WPA3; modo pessoal x corporativo."),
                ("projeto-wifi", "Projeto de cobertura", "Site survey, interferência, sobreposição de canais e roaming."),
                ("outras-sem-fio", "Outras tecnologias", "Bluetooth, redes celulares (4G e 5G), LoRa e redes de sensores."),
            ]),
        new RoadmapSeed.Subject("redes-wan", "Redes de longa distância e suas tecnologias", StudyBlock.NetworkTechnologies,
            "Como ligar sites distantes: enlaces dedicados, MPLS, VPN e acesso à internet.",
            [
                ("enlaces-wan", "Enlaces WAN", "Linhas dedicadas, PPP e HDLC; Frame Relay e ATM como história."),
                ("mpls", "MPLS", "Rótulos, LSP e por que as operadoras usam."),
                ("vpn", "VPNs", "Site a site e de acesso remoto; IPsec, SSL/TLS e túneis."),
                ("acesso-banda-larga", "Acesso à internet", "DSL, cabo, fibra (FTTH, GPON), rádio e satélite."),
                ("sd-wan", "SD-WAN", "Vários links sob uma política central de tráfego."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-tecnologias-redes", "Checkpoint: questões de tecnologias de rede", StudyBlock.NetworkTechnologies,
            "Resolva questões de sub-redes, TCP, tabelas de roteamento, VLANs e Wi-Fi. Treine sub-redes até fazer sem calculadora."),

        new RoadmapSeed.Subject("sistemas-operacionais", "Administração de sistemas operacionais", StudyBlock.NetworkOperations,
            "O servidor por dentro: usuários, permissões, processos e a linha de comando do Linux e do Windows.",
            [
                ("linux-comandos", "Linux: comandos e arquivos", "Sistema de arquivos, comandos básicos, pipes e redirecionamento."),
                ("usuarios-permissoes", "Usuários, grupos e permissões", "chmod, chown, notação octal e o modelo de permissões do Windows (NTFS)."),
                ("processos-servicos", "Processos e serviços", "systemd, logs, agendamento de tarefas (cron) e inicialização."),
                ("rede-no-so", "Rede no sistema operacional", "ip, ifconfig, netstat/ss, ping, traceroute e nslookup/dig."),
                ("windows-server", "Windows Server e Active Directory", "Domínio, controlador de domínio, GPOs e unidades organizacionais."),
            ]),
        new RoadmapSeed.Subject("servicos-rede", "Serviços de redes", StudyBlock.NetworkOperations,
            "Os serviços que fazem a rede ser útil. Cai como cenário: qual serviço resolve o problema descrito.",
            [
                ("dns", "DNS", "Hierarquia, tipos de registro (A, AAAA, CNAME, MX, NS), consulta recursiva e iterativa."),
                ("dhcp", "DHCP", "DORA, escopo, reservas e relay."),
                ("web-proxy", "Servidor web e proxy", "HTTP, proxy direto e reverso, cache e balanceamento de carga."),
                ("email", "Correio eletrônico", "SMTP, POP3 e IMAP, e o caminho de uma mensagem."),
                ("arquivos-diretorio", "Arquivos e diretório", "NFS, SMB/CIFS, FTP/SFTP e LDAP."),
            ]),
        new RoadmapSeed.Subject("configuracao-dispositivos", "Configuração de dispositivos de redes", StudyBlock.NetworkOperations,
            "Ler e completar configurações de switch e roteador. A questão traz os comandos e pergunta o efeito.",
            [
                ("cli-modos", "CLI e modos de configuração", "Modos usuário, privilegiado e de configuração; salvar e restaurar a configuração."),
                ("config-switch", "Configuração de switch", "VLANs, portas de acesso e trunk, port security."),
                ("config-roteador", "Configuração de roteador", "Interfaces, rotas estáticas, rota padrão e roteamento dinâmico."),
                ("acl", "Listas de controle de acesso", "ACL padrão e estendida, ordem das regras e o deny implícito."),
                ("nat-dhcp-roteador", "NAT e DHCP no roteador", "Configurar tradução de endereços e servidor DHCP no equipamento."),
            ]),
        new RoadmapSeed.Subject("gerenciamento-redes", "Gerenciamento de redes", StudyBlock.NetworkOperations,
            "Saber o que está acontecendo na rede antes que o usuário reclame.",
            [
                ("fcaps", "Áreas funcionais (FCAPS)", "Falhas, configuração, contabilização, desempenho e segurança."),
                ("snmp", "SNMP", "Gerente, agente, MIB, OID, traps e as versões 1, 2c e 3."),
                ("monitoramento", "Monitoramento", "Zabbix, Nagios e afins; métricas, limites e alertas."),
                ("logs-fluxos", "Logs e análise de fluxo", "Syslog, NetFlow e captura de pacotes com Wireshark."),
                ("itil-sla", "Gestão de serviços", "Incidente x problema, SLA e indicadores de disponibilidade."),
            ]),
        new RoadmapSeed.Subject("virtualizacao", "Virtualização de redes, servidores, aplicativos e desktops", StudyBlock.NetworkOperations,
            "De uma máquina física para muitas: hipervisores, contêineres e nuvem.",
            [
                ("hipervisores", "Hipervisores", "Tipo 1 x tipo 2; VMware, Hyper-V, KVM e Proxmox."),
                ("conteineres", "Contêineres", "Docker, imagens e orquestração; contêiner x máquina virtual."),
                ("redes-virtuais", "Redes virtuais e SDN", "Switch virtual, VXLAN, SDN (plano de controle separado) e NFV."),
                ("vdi-aplicativos", "Desktops e aplicativos virtuais", "VDI e virtualização de aplicativos."),
                ("nuvem", "Computação em nuvem", "IaaS, PaaS e SaaS; nuvem pública, privada e híbrida."),
            ]),
        new RoadmapSeed.Subject("automacao-redes", "Programação e automação de redes", StudyBlock.NetworkOperations,
            "Scripts e ferramentas que configuram e verificam a rede sem fazer tudo à mão.",
            [
                ("scripts", "Scripts", "Python e shell script para tarefas de rede: laços, condições e leitura de arquivos."),
                ("apis-formatos", "APIs e formatos de dados", "REST, JSON, YAML e XML; NETCONF e RESTCONF."),
                ("ferramentas-automacao", "Ferramentas de automação", "Ansible, infraestrutura como código e controle de versão."),
                ("programabilidade-sdn", "Programabilidade e SDN", "Controladores, APIs northbound e southbound e OpenFlow."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-administracao-redes", "Checkpoint: questões de administração e serviços", StudyBlock.NetworkOperations,
            "Resolva questões de comandos Linux, DNS e DHCP, configurações de switch e roteador, ACL, SNMP e virtualização."),

        new RoadmapSeed.Subject("projeto-redes", "Projeto lógico e físico de redes", StudyBlock.NetworkDesign,
            "Juntar tudo num projeto. A discursiva costuma pedir uma proposta de rede justificada.",
            [
                ("requisitos", "Levantamento de requisitos", "Usuários, aplicações, tráfego, crescimento e restrições de orçamento."),
                ("projeto-top-down", "Metodologia top-down", "Das necessidades do negócio para a tecnologia, em fases."),
                ("hierarquia", "Modelo hierárquico", "Camadas de núcleo, distribuição e acesso; redundância e alta disponibilidade."),
                ("plano-enderecamento", "Plano de endereçamento e VLANs", "Dividir a rede por setor e documentar."),
                ("projeto-fisico", "Projeto físico e documentação", "Racks, salas, cabeamento, diagramas e memorial descritivo."),
            ]),
        new RoadmapSeed.Subject("seguranca-informacao", "Segurança da informação", StudyBlock.NetworkDesign,
            "Proteger dados e serviços. Cai muito como ataque descrito e a defesa adequada.",
            [
                ("pilares", "Pilares e conceitos", "Confidencialidade, integridade, disponibilidade, autenticidade e não repúdio."),
                ("criptografia", "Criptografia", "Simétrica x assimétrica, hash, assinatura digital e certificados (PKI)."),
                ("ataques", "Ataques e ameaças", "Malware, phishing, DoS e DDoS, man-in-the-middle, spoofing e engenharia social."),
                ("firewall-ids", "Firewall, IDS/IPS e DMZ", "Filtragem de pacotes, firewall com estado, detecção e prevenção de intrusão."),
                ("politicas-normas", "Políticas, normas e LGPD", "ISO/IEC 27001 e 27002, política de segurança, backup e a LGPD."),
            ]),
        new RoadmapSeed.Subject("tecnologias-emergentes", "Tecnologias emergentes", StudyBlock.NetworkDesign,
            "O que está mudando as redes agora. Cai com texto de apoio e pede a leitura crítica.",
            [
                ("iot", "Internet das coisas", "Dispositivos, protocolos leves (MQTT) e os desafios de segurança."),
                ("5g-edge", "5G e computação de borda", "Baixa latência, network slicing e processamento perto do usuário."),
                ("ia-redes", "Inteligência artificial nas redes", "Detecção de anomalias e operação automatizada."),
                ("tendencias", "Outras tendências", "Wi-Fi 7, zero trust, SASE e redes definidas por intenção."),
            ]),
        new RoadmapSeed.Subject("sustentabilidade", "Sustentabilidade e meio ambiente", StudyBlock.NetworkDesign,
            "TI verde: consumo de energia, descarte e o impacto ambiental da infraestrutura.",
            [
                ("ti-verde", "TI verde", "Eficiência energética de data centers (PUE), virtualização e consolidação de servidores."),
                ("residuos-eletronicos", "Resíduos eletrônicos", "Descarte, logística reversa e a Política Nacional de Resíduos Sólidos."),
                ("projeto-sustentavel", "Projeto sustentável", "Escolher equipamentos e soluções pensando no ciclo de vida."),
            ]),
        new RoadmapSeed.Subject("gestao-governanca", "Gestão de projetos e governança corporativa", StudyBlock.NetworkDesign,
            "Planejar, executar e prestar contas de projetos de TI dentro da organização.",
            [
                ("gestao-projetos", "Gestão de projetos", "PMBOK: escopo, cronograma, custos, riscos e partes interessadas."),
                ("metodos-ageis", "Métodos ágeis", "Scrum e Kanban em projetos de infraestrutura."),
                ("governanca-ti", "Governança de TI", "COBIT e ITIL; alinhar a TI ao negócio."),
                ("riscos-continuidade", "Riscos e continuidade", "Análise de riscos, plano de continuidade e de recuperação de desastres."),
                ("etica-legislacao", "Ética e legislação", "Marco Civil da Internet, LGPD e responsabilidade profissional."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-projeto-redes", "Checkpoint: questões de projeto, segurança e gestão", StudyBlock.NetworkDesign,
            "Resolva questões de projeto, criptografia, ataques e governança. Escreva uma resposta discursiva propondo uma rede para um cenário."),

        new RoadmapSeed.Subject("formacao-geral", "Formação geral", StudyBlock.GeneralEducation,
            "As 15 questões comuns a todos os cursos: interpretação, dados e temas da atualidade.",
            [
                ("interpretacao-texto", "Interpretação de texto", "Identificar tese, argumentos e a afirmação que o texto sustenta."),
                ("graficos-tabelas", "Gráficos e tabelas", "Tirar conclusões corretas de dados apresentados visualmente."),
                ("atualidades", "Temas da atualidade", "Tecnologia e sociedade, inteligência artificial, meio ambiente, inclusão e direitos humanos."),
                ("discursiva", "Escrita da resposta discursiva", "Estrutura problema → proposta → justificativa em cerca de 15 linhas."),
            ]),
        new RoadmapSeed.Checkpoint("checkpoint-simulado-redes", "Checkpoint final: simulado completo", StudyBlock.GeneralEducation,
            "Faça um simulado de 4 horas no mesmo horário da prova. Depois, revise só o caderno de erros até o dia do ENADE."),
    ];

    public static IReadOnlyList<RoadmapNode> Nodes { get; } = RoadmapSeed.Build(Spine, Tracks.EnadeRedes, IdBase);
}
