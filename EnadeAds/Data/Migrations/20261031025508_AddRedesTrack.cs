using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnadeAds.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRedesTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 2,
                column: "Order",
                value: 3);

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Basis", "ExamDate", "ExamName", "Label", "Level", "MockExamStyle", "Name", "Order", "SecondExamDate", "Slug" },
                values: new object[] { 3, "Os conteúdos de Tecnologia em Redes de Computadores da Portaria Inep nº 172/2026 e a Formação Geral, na ordem sugerida de estudo.", new DateOnly(2026, 11, 29), "ENADE", "ENADE Redes", 2, 0, "ENADE 2026 · Redes de Computadores", 2, null, "enade-redes" });

            migrationBuilder.InsertData(
                table: "RoadmapNodes",
                columns: new[] { "Id", "Block", "Kind", "Order", "ParentId", "Slug", "Summary", "Title", "TrackId" },
                values: new object[,]
                {
                    { 200100, 0, 0, 1, null, "fundamentos-redes", "O vocabulário de toda a prova: o que é cada equipamento, como ele se liga aos outros e que tipo de rede isso forma.", "Fundamentos, componentes e topologias de redes", 3 },
                    { 200200, 0, 0, 2, null, "comunicacao-dados", "A camada física: sinais, meios e as contas de taxa e atraso que aparecem nas questões.", "Comunicação e transmissão de dados", 3 },
                    { 200300, 0, 0, 3, null, "modelos-referencia", "OSI e TCP/IP lado a lado. Quase toda questão de protocolo começa por saber em que camada ele está.", "Modelos de referência", 3 },
                    { 200400, 0, 0, 4, null, "cabeamento-estruturado", "Normas, categorias de cabo e limites de distância. Rende pontos com memorização bem feita.", "Normas e padrões de cabeamento estruturado", 3 },
                    { 200500, 10, 0, 6, null, "tcp-ip", "O centro da prova. Sub-redes e o comportamento do TCP aparecem em quase toda edição.", "Arquitetura TCP/IP e seus protocolos", 3 },
                    { 200600, 10, 0, 7, null, "roteamento", "Como o roteador escolhe o caminho. Cai como tabela de roteamento para interpretar ou rede para completar.", "Algoritmos e protocolos de roteamento", 3 },
                    { 200700, 10, 0, 8, null, "ieee-802", "As redes locais por dentro: Ethernet, switches e VLANs.", "Padrões IEEE 802.x", 3 },
                    { 200800, 10, 0, 9, null, "redes-sem-fio", "Wi-Fi e as outras tecnologias sem fio, com foco em padrão, alcance e segurança.", "Redes sem fio", 3 },
                    { 200900, 10, 0, 10, null, "redes-wan", "Como ligar sites distantes: enlaces dedicados, MPLS, VPN e acesso à internet.", "Redes de longa distância e suas tecnologias", 3 },
                    { 201000, 11, 0, 12, null, "sistemas-operacionais", "O servidor por dentro: usuários, permissões, processos e a linha de comando do Linux e do Windows.", "Administração de sistemas operacionais", 3 },
                    { 201100, 11, 0, 13, null, "servicos-rede", "Os serviços que fazem a rede ser útil. Cai como cenário: qual serviço resolve o problema descrito.", "Serviços de redes", 3 },
                    { 201200, 11, 0, 14, null, "configuracao-dispositivos", "Ler e completar configurações de switch e roteador. A questão traz os comandos e pergunta o efeito.", "Configuração de dispositivos de redes", 3 },
                    { 201300, 11, 0, 15, null, "gerenciamento-redes", "Saber o que está acontecendo na rede antes que o usuário reclame.", "Gerenciamento de redes", 3 },
                    { 201400, 11, 0, 16, null, "virtualizacao", "De uma máquina física para muitas: hipervisores, contêineres e nuvem.", "Virtualização de redes, servidores, aplicativos e desktops", 3 },
                    { 201500, 11, 0, 17, null, "automacao-redes", "Scripts e ferramentas que configuram e verificam a rede sem fazer tudo à mão.", "Programação e automação de redes", 3 },
                    { 201600, 12, 0, 19, null, "projeto-redes", "Juntar tudo num projeto. A discursiva costuma pedir uma proposta de rede justificada.", "Projeto lógico e físico de redes", 3 },
                    { 201700, 12, 0, 20, null, "seguranca-informacao", "Proteger dados e serviços. Cai muito como ataque descrito e a defesa adequada.", "Segurança da informação", 3 },
                    { 201800, 12, 0, 21, null, "tecnologias-emergentes", "O que está mudando as redes agora. Cai com texto de apoio e pede a leitura crítica.", "Tecnologias emergentes", 3 },
                    { 201900, 12, 0, 22, null, "sustentabilidade", "TI verde: consumo de energia, descarte e o impacto ambiental da infraestrutura.", "Sustentabilidade e meio ambiente", 3 },
                    { 202000, 12, 0, 23, null, "gestao-governanca", "Planejar, executar e prestar contas de projetos de TI dentro da organização.", "Gestão de projetos e governança corporativa", 3 },
                    { 202100, 4, 0, 25, null, "formacao-geral", "As 15 questões comuns a todos os cursos: interpretação, dados e temas da atualidade.", "Formação geral", 3 },
                    { 210001, 0, 2, 5, null, "checkpoint-fundamentos-redes", "Antes de seguir, resolva questões de topologias, domínios de colisão, camadas do OSI, contas de taxa e atraso e cabeamento. Anote os erros no caderno de erros.", "Checkpoint: questões de fundamentos", 3 },
                    { 210002, 10, 2, 11, null, "checkpoint-tecnologias-redes", "Resolva questões de sub-redes, TCP, tabelas de roteamento, VLANs e Wi-Fi. Treine sub-redes até fazer sem calculadora.", "Checkpoint: questões de tecnologias de rede", 3 },
                    { 210003, 11, 2, 18, null, "checkpoint-administracao-redes", "Resolva questões de comandos Linux, DNS e DHCP, configurações de switch e roteador, ACL, SNMP e virtualização.", "Checkpoint: questões de administração e serviços", 3 },
                    { 210004, 12, 2, 24, null, "checkpoint-projeto-redes", "Resolva questões de projeto, criptografia, ataques e governança. Escreva uma resposta discursiva propondo uma rede para um cenário.", "Checkpoint: questões de projeto, segurança e gestão", 3 },
                    { 210005, 4, 2, 26, null, "checkpoint-simulado-redes", "Faça um simulado de 4 horas no mesmo horário da prova. Depois, revise só o caderno de erros até o dia do ENADE.", "Checkpoint final: simulado completo", 3 },
                    { 200101, 0, 1, 1, 200100, "classificacao-redes", "PAN, LAN, MAN e WAN; redes cliente-servidor e ponto a ponto.", "Classificação das redes", 3 },
                    { 200102, 0, 1, 2, 200100, "topologias", "Barramento, anel, estrela, malha e árvore: vantagens, pontos únicos de falha e o que muda entre o físico e o lógico.", "Topologias física e lógica", 3 },
                    { 200103, 0, 1, 3, 200100, "equipamentos", "Placa de rede, hub, switch, roteador, access point e firewall: em que camada cada um trabalha.", "Equipamentos de rede", 3 },
                    { 200104, 0, 1, 4, 200100, "dominios-colisao-broadcast", "Quantos domínios cada equipamento separa; questão clássica com diagrama.", "Domínios de colisão e de broadcast", 3 },
                    { 200105, 0, 1, 5, 200100, "comutacao", "Como os dados atravessam a rede e o que isso muda em atraso e aproveitamento.", "Comutação de circuitos e de pacotes", 3 },
                    { 200201, 0, 1, 1, 200200, "sinais-modulacao", "Analógico x digital, codificação de linha (NRZ, Manchester) e modulação (ASK, FSK, PSK, QAM).", "Sinais, codificação e modulação", 3 },
                    { 200202, 0, 1, 2, 200200, "meios-transmissao", "Par trançado, coaxial, fibra monomodo e multimodo e rádio: alcance, banda e interferência.", "Meios de transmissão", 3 },
                    { 200203, 0, 1, 3, 200200, "banda-taxa", "Nyquist e Shannon, bits x bytes e a diferença entre taxa nominal e vazão real.", "Largura de banda, taxa e vazão", 3 },
                    { 200204, 0, 1, 4, 200200, "atrasos", "Atraso de transmissão, propagação, fila e processamento; RTT e jitter.", "Atrasos e desempenho", 3 },
                    { 200205, 0, 1, 5, 200200, "multiplexacao-erros", "FDM, TDM e WDM; paridade, CRC e checksum.", "Multiplexação e detecção de erros", 3 },
                    { 200301, 0, 1, 1, 200300, "modelo-osi", "As sete camadas, a função de cada uma e exemplos de protocolos e equipamentos.", "Modelo OSI", 3 },
                    { 200302, 0, 1, 2, 200300, "modelo-tcp-ip", "As quatro (ou cinco) camadas e como elas se comparam às do OSI.", "Modelo TCP/IP", 3 },
                    { 200303, 0, 1, 3, 200300, "encapsulamento-pdu", "Mensagem, segmento, pacote, quadro e bit: o que cada camada acrescenta.", "Encapsulamento e PDUs", 3 },
                    { 200304, 0, 1, 4, 200300, "servicos-camadas", "Orientado e não orientado à conexão, confiável e não confiável.", "Serviços entre camadas", 3 },
                    { 200401, 0, 1, 1, 200400, "normas-cabeamento", "ANSI/TIA-568, ISO/IEC 11801 e NBR 14565: o que cada uma define.", "Normas", 3 },
                    { 200402, 0, 1, 2, 200400, "subsistemas", "Entrada do edifício, sala de equipamentos, backbone, cabeamento horizontal e área de trabalho.", "Subsistemas do cabeamento", 3 },
                    { 200403, 0, 1, 3, 200400, "categorias-cabos", "Cat5e, Cat6, Cat6A e Cat8; RJ-45, padrões T568A e T568B, cabo direto e cruzado.", "Categorias e conectores", 3 },
                    { 200404, 0, 1, 4, 200400, "limites-certificacao", "Os 90 m + 10 m do horizontal, fibra no backbone e os testes de certificação.", "Limites e certificação", 3 },
                    { 200501, 10, 1, 1, 200500, "ipv4-enderecamento", "Classes, máscara, notação CIDR, endereços privados e especiais.", "Endereçamento IPv4", 3 },
                    { 200502, 10, 1, 2, 200500, "sub-redes", "Calcular rede, broadcast, faixa de hosts e quantidade de sub-redes sem calculadora.", "Sub-redes e VLSM", 3 },
                    { 200503, 10, 1, 3, 200500, "ipv6", "Formato, abreviação, tipos de endereço, autoconfiguração e convivência com o IPv4.", "IPv6", 3 },
                    { 200504, 10, 1, 4, 200500, "arp-icmp-nat", "Resolução de endereço, ping e traceroute, tradução de endereços e PAT.", "ARP, ICMP e NAT", 3 },
                    { 200505, 10, 1, 5, 200500, "tcp-udp", "Portas, three-way handshake, janela deslizante, controle de fluxo e de congestionamento.", "TCP e UDP", 3 },
                    { 200506, 10, 1, 6, 200500, "protocolos-aplicacao", "HTTP/HTTPS, DNS, DHCP, SMTP, POP3, IMAP, FTP e SSH, com as portas de cada um.", "Protocolos de aplicação", 3 },
                    { 200601, 10, 1, 1, 200600, "tabela-roteamento", "Rotas diretas, estáticas e padrão; prefixo mais longo e distância administrativa.", "Tabela de roteamento", 3 },
                    { 200602, 10, 1, 2, 200600, "vetor-distancia", "Bellman-Ford, RIP, contagem ao infinito, split horizon e poison reverse.", "Vetor de distância", 3 },
                    { 200603, 10, 1, 3, 200600, "estado-enlace", "Dijkstra, OSPF, áreas e custo dos enlaces.", "Estado de enlace", 3 },
                    { 200604, 10, 1, 4, 200600, "bgp-sistemas-autonomos", "Roteamento interno x externo e como a internet se interliga.", "BGP e sistemas autônomos", 3 },
                    { 200701, 10, 1, 1, 200700, "ethernet", "Quadro Ethernet, endereço MAC, CSMA/CD e as velocidades do Fast ao 10 Gigabit.", "Ethernet (802.3)", 3 },
                    { 200702, 10, 1, 2, 200700, "switching", "Aprendizado de MAC, tabela CAM, full duplex e encaminhamento.", "Comutação na camada 2", 3 },
                    { 200703, 10, 1, 3, 200700, "vlan-trunk", "Segmentar a LAN, marcação de quadros e roteamento entre VLANs.", "VLANs e trunk (802.1Q)", 3 },
                    { 200704, 10, 1, 4, 200700, "stp", "Por que laços derrubam a rede e como o STP os bloqueia.", "Spanning Tree (802.1D/802.1w)", 3 },
                    { 200705, 10, 1, 5, 200700, "agregacao-poe-8021x", "LACP (802.3ad), alimentação pelo cabo e controle de acesso por porta.", "Agregação de enlaces, PoE e 802.1X", 3 },
                    { 200801, 10, 1, 1, 200800, "padroes-80211", "802.11a/b/g/n/ac/ax (Wi-Fi 4, 5, 6 e 6E): faixas, canais e taxas.", "Padrões 802.11", 3 },
                    { 200802, 10, 1, 2, 200800, "csma-ca", "CSMA/CA, RTS/CTS e o problema do terminal oculto.", "Acesso ao meio no Wi-Fi", 3 },
                    { 200803, 10, 1, 3, 200800, "seguranca-wifi", "WEP, WPA, WPA2 e WPA3; modo pessoal x corporativo.", "Segurança no Wi-Fi", 3 },
                    { 200804, 10, 1, 4, 200800, "projeto-wifi", "Site survey, interferência, sobreposição de canais e roaming.", "Projeto de cobertura", 3 },
                    { 200805, 10, 1, 5, 200800, "outras-sem-fio", "Bluetooth, redes celulares (4G e 5G), LoRa e redes de sensores.", "Outras tecnologias", 3 },
                    { 200901, 10, 1, 1, 200900, "enlaces-wan", "Linhas dedicadas, PPP e HDLC; Frame Relay e ATM como história.", "Enlaces WAN", 3 },
                    { 200902, 10, 1, 2, 200900, "mpls", "Rótulos, LSP e por que as operadoras usam.", "MPLS", 3 },
                    { 200903, 10, 1, 3, 200900, "vpn", "Site a site e de acesso remoto; IPsec, SSL/TLS e túneis.", "VPNs", 3 },
                    { 200904, 10, 1, 4, 200900, "acesso-banda-larga", "DSL, cabo, fibra (FTTH, GPON), rádio e satélite.", "Acesso à internet", 3 },
                    { 200905, 10, 1, 5, 200900, "sd-wan", "Vários links sob uma política central de tráfego.", "SD-WAN", 3 },
                    { 201001, 11, 1, 1, 201000, "linux-comandos", "Sistema de arquivos, comandos básicos, pipes e redirecionamento.", "Linux: comandos e arquivos", 3 },
                    { 201002, 11, 1, 2, 201000, "usuarios-permissoes", "chmod, chown, notação octal e o modelo de permissões do Windows (NTFS).", "Usuários, grupos e permissões", 3 },
                    { 201003, 11, 1, 3, 201000, "processos-servicos", "systemd, logs, agendamento de tarefas (cron) e inicialização.", "Processos e serviços", 3 },
                    { 201004, 11, 1, 4, 201000, "rede-no-so", "ip, ifconfig, netstat/ss, ping, traceroute e nslookup/dig.", "Rede no sistema operacional", 3 },
                    { 201005, 11, 1, 5, 201000, "windows-server", "Domínio, controlador de domínio, GPOs e unidades organizacionais.", "Windows Server e Active Directory", 3 },
                    { 201101, 11, 1, 1, 201100, "dns", "Hierarquia, tipos de registro (A, AAAA, CNAME, MX, NS), consulta recursiva e iterativa.", "DNS", 3 },
                    { 201102, 11, 1, 2, 201100, "dhcp", "DORA, escopo, reservas e relay.", "DHCP", 3 },
                    { 201103, 11, 1, 3, 201100, "web-proxy", "HTTP, proxy direto e reverso, cache e balanceamento de carga.", "Servidor web e proxy", 3 },
                    { 201104, 11, 1, 4, 201100, "email", "SMTP, POP3 e IMAP, e o caminho de uma mensagem.", "Correio eletrônico", 3 },
                    { 201105, 11, 1, 5, 201100, "arquivos-diretorio", "NFS, SMB/CIFS, FTP/SFTP e LDAP.", "Arquivos e diretório", 3 },
                    { 201201, 11, 1, 1, 201200, "cli-modos", "Modos usuário, privilegiado e de configuração; salvar e restaurar a configuração.", "CLI e modos de configuração", 3 },
                    { 201202, 11, 1, 2, 201200, "config-switch", "VLANs, portas de acesso e trunk, port security.", "Configuração de switch", 3 },
                    { 201203, 11, 1, 3, 201200, "config-roteador", "Interfaces, rotas estáticas, rota padrão e roteamento dinâmico.", "Configuração de roteador", 3 },
                    { 201204, 11, 1, 4, 201200, "acl", "ACL padrão e estendida, ordem das regras e o deny implícito.", "Listas de controle de acesso", 3 },
                    { 201205, 11, 1, 5, 201200, "nat-dhcp-roteador", "Configurar tradução de endereços e servidor DHCP no equipamento.", "NAT e DHCP no roteador", 3 },
                    { 201301, 11, 1, 1, 201300, "fcaps", "Falhas, configuração, contabilização, desempenho e segurança.", "Áreas funcionais (FCAPS)", 3 },
                    { 201302, 11, 1, 2, 201300, "snmp", "Gerente, agente, MIB, OID, traps e as versões 1, 2c e 3.", "SNMP", 3 },
                    { 201303, 11, 1, 3, 201300, "monitoramento", "Zabbix, Nagios e afins; métricas, limites e alertas.", "Monitoramento", 3 },
                    { 201304, 11, 1, 4, 201300, "logs-fluxos", "Syslog, NetFlow e captura de pacotes com Wireshark.", "Logs e análise de fluxo", 3 },
                    { 201305, 11, 1, 5, 201300, "itil-sla", "Incidente x problema, SLA e indicadores de disponibilidade.", "Gestão de serviços", 3 },
                    { 201401, 11, 1, 1, 201400, "hipervisores", "Tipo 1 x tipo 2; VMware, Hyper-V, KVM e Proxmox.", "Hipervisores", 3 },
                    { 201402, 11, 1, 2, 201400, "conteineres", "Docker, imagens e orquestração; contêiner x máquina virtual.", "Contêineres", 3 },
                    { 201403, 11, 1, 3, 201400, "redes-virtuais", "Switch virtual, VXLAN, SDN (plano de controle separado) e NFV.", "Redes virtuais e SDN", 3 },
                    { 201404, 11, 1, 4, 201400, "vdi-aplicativos", "VDI e virtualização de aplicativos.", "Desktops e aplicativos virtuais", 3 },
                    { 201405, 11, 1, 5, 201400, "nuvem", "IaaS, PaaS e SaaS; nuvem pública, privada e híbrida.", "Computação em nuvem", 3 },
                    { 201501, 11, 1, 1, 201500, "scripts", "Python e shell script para tarefas de rede: laços, condições e leitura de arquivos.", "Scripts", 3 },
                    { 201502, 11, 1, 2, 201500, "apis-formatos", "REST, JSON, YAML e XML; NETCONF e RESTCONF.", "APIs e formatos de dados", 3 },
                    { 201503, 11, 1, 3, 201500, "ferramentas-automacao", "Ansible, infraestrutura como código e controle de versão.", "Ferramentas de automação", 3 },
                    { 201504, 11, 1, 4, 201500, "programabilidade-sdn", "Controladores, APIs northbound e southbound e OpenFlow.", "Programabilidade e SDN", 3 },
                    { 201601, 12, 1, 1, 201600, "requisitos", "Usuários, aplicações, tráfego, crescimento e restrições de orçamento.", "Levantamento de requisitos", 3 },
                    { 201602, 12, 1, 2, 201600, "projeto-top-down", "Das necessidades do negócio para a tecnologia, em fases.", "Metodologia top-down", 3 },
                    { 201603, 12, 1, 3, 201600, "hierarquia", "Camadas de núcleo, distribuição e acesso; redundância e alta disponibilidade.", "Modelo hierárquico", 3 },
                    { 201604, 12, 1, 4, 201600, "plano-enderecamento", "Dividir a rede por setor e documentar.", "Plano de endereçamento e VLANs", 3 },
                    { 201605, 12, 1, 5, 201600, "projeto-fisico", "Racks, salas, cabeamento, diagramas e memorial descritivo.", "Projeto físico e documentação", 3 },
                    { 201701, 12, 1, 1, 201700, "pilares", "Confidencialidade, integridade, disponibilidade, autenticidade e não repúdio.", "Pilares e conceitos", 3 },
                    { 201702, 12, 1, 2, 201700, "criptografia", "Simétrica x assimétrica, hash, assinatura digital e certificados (PKI).", "Criptografia", 3 },
                    { 201703, 12, 1, 3, 201700, "ataques", "Malware, phishing, DoS e DDoS, man-in-the-middle, spoofing e engenharia social.", "Ataques e ameaças", 3 },
                    { 201704, 12, 1, 4, 201700, "firewall-ids", "Filtragem de pacotes, firewall com estado, detecção e prevenção de intrusão.", "Firewall, IDS/IPS e DMZ", 3 },
                    { 201705, 12, 1, 5, 201700, "politicas-normas", "ISO/IEC 27001 e 27002, política de segurança, backup e a LGPD.", "Políticas, normas e LGPD", 3 },
                    { 201801, 12, 1, 1, 201800, "iot", "Dispositivos, protocolos leves (MQTT) e os desafios de segurança.", "Internet das coisas", 3 },
                    { 201802, 12, 1, 2, 201800, "5g-edge", "Baixa latência, network slicing e processamento perto do usuário.", "5G e computação de borda", 3 },
                    { 201803, 12, 1, 3, 201800, "ia-redes", "Detecção de anomalias e operação automatizada.", "Inteligência artificial nas redes", 3 },
                    { 201804, 12, 1, 4, 201800, "tendencias", "Wi-Fi 7, zero trust, SASE e redes definidas por intenção.", "Outras tendências", 3 },
                    { 201901, 12, 1, 1, 201900, "ti-verde", "Eficiência energética de data centers (PUE), virtualização e consolidação de servidores.", "TI verde", 3 },
                    { 201902, 12, 1, 2, 201900, "residuos-eletronicos", "Descarte, logística reversa e a Política Nacional de Resíduos Sólidos.", "Resíduos eletrônicos", 3 },
                    { 201903, 12, 1, 3, 201900, "projeto-sustentavel", "Escolher equipamentos e soluções pensando no ciclo de vida.", "Projeto sustentável", 3 },
                    { 202001, 12, 1, 1, 202000, "gestao-projetos", "PMBOK: escopo, cronograma, custos, riscos e partes interessadas.", "Gestão de projetos", 3 },
                    { 202002, 12, 1, 2, 202000, "metodos-ageis", "Scrum e Kanban em projetos de infraestrutura.", "Métodos ágeis", 3 },
                    { 202003, 12, 1, 3, 202000, "governanca-ti", "COBIT e ITIL; alinhar a TI ao negócio.", "Governança de TI", 3 },
                    { 202004, 12, 1, 4, 202000, "riscos-continuidade", "Análise de riscos, plano de continuidade e de recuperação de desastres.", "Riscos e continuidade", 3 },
                    { 202005, 12, 1, 5, 202000, "etica-legislacao", "Marco Civil da Internet, LGPD e responsabilidade profissional.", "Ética e legislação", 3 },
                    { 202101, 4, 1, 1, 202100, "interpretacao-texto", "Identificar tese, argumentos e a afirmação que o texto sustenta.", "Interpretação de texto", 3 },
                    { 202102, 4, 1, 2, 202100, "graficos-tabelas", "Tirar conclusões corretas de dados apresentados visualmente.", "Gráficos e tabelas", 3 },
                    { 202103, 4, 1, 3, 202100, "atualidades", "Tecnologia e sociedade, inteligência artificial, meio ambiente, inclusão e direitos humanos.", "Temas da atualidade", 3 },
                    { 202104, 4, 1, 4, 202100, "discursiva", "Estrutura problema → proposta → justificativa em cerca de 15 linhas.", "Escrita da resposta discursiva", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200101);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200102);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200103);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200104);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200105);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200201);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200202);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200203);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200204);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200205);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200301);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200302);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200303);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200304);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200401);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200402);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200403);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200404);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200501);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200502);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200503);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200504);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200505);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200506);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200601);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200602);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200603);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200604);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200701);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200702);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200703);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200704);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200705);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200801);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200802);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200803);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200804);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200805);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200901);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200902);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200903);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200904);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200905);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201001);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201002);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201003);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201004);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201005);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201101);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201102);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201103);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201104);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201105);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201201);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201202);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201203);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201204);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201205);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201301);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201302);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201303);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201304);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201305);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201401);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201402);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201403);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201404);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201405);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201501);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201502);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201503);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201504);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201601);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201602);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201603);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201604);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201605);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201701);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201702);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201703);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201704);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201705);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201801);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201802);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201803);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201804);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201901);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201902);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201903);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202001);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202002);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202003);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202004);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202005);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202101);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202102);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202103);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202104);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 210001);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 210002);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 210003);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 210004);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 210005);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200100);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200200);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200300);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200400);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200500);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200600);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200700);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200800);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 200900);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201000);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201100);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201200);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201300);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201400);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201500);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201600);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201700);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201800);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 201900);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202000);

            migrationBuilder.DeleteData(
                table: "RoadmapNodes",
                keyColumn: "Id",
                keyValue: 202100);

            migrationBuilder.DeleteData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 2,
                column: "Order",
                value: 2);
        }
    }
}
