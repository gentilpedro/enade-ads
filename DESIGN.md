---
name: ENADE ADS
description: A preparação para o ENADE desenhada como a grade do semestre, em azul-tinta, fios finos e a cor de cada matéria.
colors:
  ink: "#14213D"
  ink-hover: "#23335A"
  ink-active: "#0D1730"
  ink-text: "#E9EDF5"
  ink-muted: "#AEB8CF"
  porcelain: "#E6EAF2"
  you: "#C23D1C"
  you-tint: "#FCECE6"
  you-on-ink: "#FF8A65"
  link: "#1D4FA3"
  ground: "#FFFFFF"
  ground-alt: "#F4F6F9"
  rule: "#D9DEE7"
  rule-strong: "#9AA3B5"
  text: "#1A2335"
  muted: "#556075"
  block-fund: "#0B7F70"
  block-fund-tint: "#E0F2EF"
  block-data: "#4646BE"
  block-data-tint: "#ECECFB"
  block-soft: "#2A7739"
  block-soft-tint: "#E4F1E6"
  block-prof: "#93570E"
  block-prof-tint: "#FAEFDD"
  block-gen: "#8A3883"
  block-gen-tint: "#F5E7F3"
typography:
  display:
    fontFamily: "Archivo, system-ui, -apple-system, Segoe UI, Roboto, sans-serif"
    fontSize: "2.25rem"
    fontWeight: 800
    lineHeight: 1
    letterSpacing: "-0.03em"
    fontFeature: "\"tnum\" 1"
  headline:
    fontFamily: "Archivo, system-ui, -apple-system, Segoe UI, Roboto, sans-serif"
    fontSize: "1.625rem"
    fontWeight: 700
    lineHeight: 1.2
    letterSpacing: "-0.012em"
  title:
    fontFamily: "Archivo, system-ui, -apple-system, Segoe UI, Roboto, sans-serif"
    fontSize: "1.0625rem"
    fontWeight: 700
    lineHeight: 1.35
    letterSpacing: "-0.012em"
  body:
    fontFamily: "Archivo, system-ui, -apple-system, Segoe UI, Roboto, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.55
  body-sm:
    fontFamily: "Archivo, system-ui, -apple-system, Segoe UI, Roboto, sans-serif"
    fontSize: "0.8125rem"
    fontWeight: 400
    lineHeight: 1.55
  module-title:
    fontFamily: "Archivo, system-ui, sans-serif"
    fontSize: "0.8125rem"
    fontWeight: 700
    letterSpacing: "0.045em"
    fontVariation: "\"wdth\" 75"
  label:
    fontFamily: "Archivo, system-ui, sans-serif"
    fontSize: "0.75rem"
    fontWeight: 700
    letterSpacing: "0.045em"
    fontVariation: "\"wdth\" 75"
  label-tag:
    fontFamily: "Archivo, system-ui, sans-serif"
    fontSize: "0.6875rem"
    fontWeight: 700
    lineHeight: 1.5
    letterSpacing: "0.05em"
    fontVariation: "\"wdth\" 75"
rounded:
  xs: "0.125rem"
  sm: "0.1875rem"
  md: "0.25rem"
  lg: "0.375rem"
spacing:
  2xs: "0.25rem"
  xs: "0.375rem"
  sm: "0.5rem"
  md: "0.75rem"
  row: "0.875rem"
  module: "1.75rem"
  gutter: "2rem"
  gutter-mobile: "1rem"
components:
  button-primary:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.ground}"
    rounded: "{rounded.md}"
    padding: "0.375rem 0.75rem"
  button-primary-hover:
    backgroundColor: "{colors.ink-hover}"
    textColor: "{colors.ground}"
  button-primary-active:
    backgroundColor: "{colors.ink-active}"
    textColor: "{colors.ground}"
  button-outline:
    backgroundColor: "{colors.ground}"
    textColor: "{colors.ink}"
    rounded: "{rounded.md}"
    padding: "0.375rem 0.75rem"
  button-outline-hover:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.ground}"
  header-cta:
    backgroundColor: "{colors.ground}"
    textColor: "{colors.ink}"
    rounded: "{rounded.md}"
    padding: "0.375rem 0.875rem"
  site-header:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.ink-text}"
    height: "3.5rem"
  filter-chip:
    backgroundColor: "{colors.ground}"
    textColor: "{colors.muted}"
    typography: "{typography.label}"
    rounded: "{rounded.sm}"
    padding: "0.1875rem 0.5rem"
  filter-chip-active:
    backgroundColor: "{colors.text}"
    textColor: "{colors.ground}"
  tag-official:
    backgroundColor: "{colors.text}"
    textColor: "{colors.ground}"
    typography: "{typography.label-tag}"
    rounded: "{rounded.xs}"
    padding: "0.0625rem 0.375rem"
  tag-doubt:
    backgroundColor: "{colors.block-data-tint}"
    textColor: "{colors.block-data}"
    typography: "{typography.label-tag}"
    rounded: "{rounded.xs}"
    padding: "0.0625rem 0.375rem"
  tag-tip:
    backgroundColor: "{colors.block-fund-tint}"
    textColor: "{colors.block-fund}"
    typography: "{typography.label-tag}"
    rounded: "{rounded.xs}"
    padding: "0.0625rem 0.375rem"
  tag-discussion:
    backgroundColor: "{colors.block-prof-tint}"
    textColor: "{colors.block-prof}"
    typography: "{typography.label-tag}"
    rounded: "{rounded.xs}"
    padding: "0.0625rem 0.375rem"
  badge-teacher:
    textColor: "{colors.text}"
    typography: "{typography.label-tag}"
    rounded: "{rounded.xs}"
    padding: "0 0.3125rem"
  week-current:
    backgroundColor: "{colors.you-tint}"
    textColor: "{colors.you}"
    padding: "0.75rem 0.75rem 0.875rem"
  slot:
    backgroundColor: "{colors.ground}"
    textColor: "{colors.text}"
    rounded: "{rounded.sm}"
    padding: "0.375rem 0.5rem"
  slot-missed:
    backgroundColor: "{colors.ground-alt}"
    textColor: "{colors.muted}"
  slot-exam:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.ground}"
    rounded: "{rounded.sm}"
    padding: "0.5rem"
  next-step:
    backgroundColor: "{colors.you-tint}"
    textColor: "{colors.text}"
    padding: "0.75rem 0.875rem"
  meter:
    backgroundColor: "{colors.ground-alt}"
    height: "0.375rem"
  avatar:
    backgroundColor: "{colors.you-on-ink}"
    textColor: "{colors.ink}"
    size: "2rem"
  tab-bar-item-active:
    backgroundColor: "{colors.ground}"
    textColor: "{colors.text}"
---

# Design System: ENADE ADS

## Overview

**Creative North Star: "A Grade do Semestre"**

O app é desenhado como a grade horária impressa de uma faculdade: um cabeçalho azul-tinta, um chão claro dividido por fios finos e colunas por semana até a prova. Cada matéria tem sua cor, e essa cor só aparece onde há matéria ou categoria. Uma única cor quente, o coral, é reservada para "você": sua semana, seu voto, seu próximo passo. O resto é neutro, denso e legível, para o aluno que abre no celular entre aulas e para quem estuda horas no computador.

A densidade é de documento de trabalho, não de app de comunidade genérico. Em vez de cartões flutuantes, módulos com um fio grosso em cima e uma aba de título estreita; em vez de listas em cartões, linhas separadas por fios de 1px, como as linhas da grade. Estado se lê pela forma da linha (sólida entregue, tracejada pendente), não só pela cor. Todo item tem um código legível em letra estreita: "Sem 44", "2021·Q12".

Archivo é a única família, e o eixo de largura faz o trabalho de uma segunda fonte: largura normal para ler, estreita (75%) em caixa alta para rótulos, códigos e cabeçalhos de coluna. Claro e escuro são obrigatórios e trocados por `data-bs-theme` no `<html>`; a casca azul-tinta continua nos dois temas. A base é Bootstrap 5.3, restilizado só pelas variáveis, sem forkar componentes. A página do Roadmap é uma exceção local deliberada (ver Colors).

**Key Characteristics:**
- Casca azul-tinta de 56px nos dois temas; chão branco (claro) ou azul-noite (escuro).
- Fios no lugar de cartões: 2px no topo do módulo, 1px entre linhas.
- Cinco cores de matéria, cada uma com seu tom tingido; coral só para "você".
- Uma família, dois corpos: Archivo normal e Archivo estreita em caixa alta.
- Estado pela forma da borda: sólida, tracejada, apagada com risco.
- Código legível em todo item, em numerais tabulares.

## Colors

Neutros frios de azul-tinta carregam a estrutura; as cores saturadas são semânticas: matéria ou "você", nunca decoração. Os valores do frontmatter são do tema claro; os do tema escuro estão no sidecar (`colorMeta.*.dark`) e em `app.css` no bloco `[data-bs-theme="dark"]`.

### Primary
- **Azul-Tinta** (ink): a casca do app (cabeçalho fixo), o botão primário e a página ativa da paginação no tema claro, e o bloco da prova na faixa do semestre. No escuro escurece para quase preto-azulado e deixa de ser cor de botão. Hover e pressionado usam **Tinta Clara** (ink-hover) e **Tinta Funda** (ink-active).
- **Porcelana** (porcelain): o botão primário, a paginação ativa e o bloco da prova no tema escuro, com texto quase preto. Garante que o botão nunca se confunda nem com a casca nem com o chão.
- **Texto da Casca** (ink-text) e **Casca Apagada** (ink-muted): texto e itens de navegação inativos sobre o azul-tinta.

### Secondary
- **Coral Você** (you): exclusivo do que é do usuário. Barra de 4px no topo da semana atual e o rótulo "Esta semana", fio de 2px do próximo passo, seta de voto dado, cor da seleção de texto (a 22%). **Coral Tingido** (you-tint) é o fundo da semana atual e do próximo passo. No escuro o coral clareia para o mesmo tom de **Coral sobre Tinta** (you-on-ink), que também é o fundo do avatar no cabeçalho.

### Tertiary
As cores de bloco do roadmap. Cada matéria herda a cor do seu bloco em todo o app (`StudyBlock.ColorClass()` em `RoadmapNode.cs` mapeia para `.block-*`, que define `--block` e `--block-tint`). Cada uma tem um tom forte para texto e borda e um tingido para fundo.
- **Verde-Azulado Fundamentos** (block-fund / block-fund-tint): Fundamentos; também a etiqueta "Dicas de estudo".
- **Índigo Dados** (block-data / block-data-tint): Dados e infraestrutura; também a etiqueta "Dúvidas".
- **Verde Engenharia** (block-soft / block-soft-tint): Engenharia de software.
- **Âmbar Profissional** (block-prof / block-prof-tint): Contexto profissional; também a etiqueta "Discussão".
- **Ameixa Formação Geral** (block-gen / block-gen-tint): Formação geral e o padrão para qualquer bloco não mapeado.

### Neutral
- **Chão** (ground): fundo da página e dos blocos da faixa.
- **Chão Alternado** (ground-alt): linha fixada do feed, hover de linha de questão, trilho do medidor, listras diagonais da semana omitida, blocos perdidos.
- **Fio** (rule): todas as divisões de 1px (linhas do feed e das questões, colunas da semana, bordas de chip e campo).
- **Fio Forte** (rule-strong): borda tracejada do bloco pendente sem matéria e o botão desabilitado.
- **Texto** (text): corpo, títulos e o fio de 2px do módulo; também o fundo do chip ativo e da etiqueta oficial (com texto no chão), invertendo nos dois temas.
- **Texto Apagado** (muted): metadados, abas inativas, resumos do feed.
- **Link** (link): links de texto e o anel de foco (2px, deslocado 2px).

### Named Rules
**The Coral Is You Rule.** Coral marca só o que pertence ao usuário: a semana atual, o voto dado, o próximo passo, o avatar. Nunca em botão genérico, ícone decorativo ou destaque de marketing.

**The Color Means Subject Rule.** Cor de bloco só aparece onde há uma matéria ou uma categoria do feed: bloco da semana, etiqueta, medidor, linha de questão. Sem matéria, o elemento fica neutro (tinta, texto ou fio).

**The Ink Shell Rule.** O cabeçalho é azul-tinta nos dois temas. O botão primário é tinta no claro e porcelana no escuro; o tema escuro nunca usa o azul do Bootstrap como fundo de botão.

**The Roadmap Exception Rule.** A página do Roadmap mantém a paleta do roadmap.sh (matéria em amarelo `#fdff00`, assunto em bege `#ffe599`, espinha azul `#2b78e4`, checkpoint índigo `#4136d4`, caixas com borda preta de 2px e sombra dura de 3px no hover) por pedido explícito do usuário. Esses valores vivem só em `Roadmap.razor.css` sob `--rm-*` e não se estendem a nenhuma outra página.

## Typography

**Display Font:** Archivo (com system-ui, -apple-system, Segoe UI, Roboto, sans-serif)
**Body Font:** Archivo (mesma família)
**Label/Mono Font:** Archivo estreita, eixo `wdth` a 75% via `font-stretch: var(--narrow)`

**Character:** Uma grotesca de trabalho, hospedada no próprio site (a CSP só aceita fonte de `'self'`), variável de 100 a 900 e de 62% a 125% de largura. A versão estreita em caixa alta tem o tom de cabeçalho de coluna de grade impressa; a normal lê confortável em textos longos.

### Hierarchy
- **Display** (800, 2.25rem, 1, -0.03em, numerais tabulares): o número de dias até a prova na faixa do semestre (1.75rem no celular, em linha com o texto). O Roadmap usa a mesma voz a 2.75rem.
- **Headline** (700, 1.625rem, -0.012em, `text-wrap: balance`): título de página no feed. Demais títulos seguem a escala do Bootstrap com peso 700 e o mesmo tracking.
- **Title** (700, 1.0625rem, 1.35): título de post no feed e de item de lista.
- **Body** (400, 1rem, 1.55, `text-wrap: pretty`): texto corrido. Enunciados e alternativas limitados a 75ch, resumos do feed a 72ch, texto de questão na lista a 66ch, páginas legais a 70ch.
- **Body pequeno** (400, 0.8125–0.875rem): metadados, datas, lateral, rodapé.
- **Título de módulo** (700, 0.8125rem, estreita, 0.045em, caixa alta): a aba de título do módulo. É o título real da seção (um `h2`), não um sobretítulo.
- **Label** (700, 0.75rem, estreita, 0.045em, caixa alta): códigos de semana e de questão, chips de filtro, contagem de comentários, grupos do menu da conta.
- **Etiqueta** (700, 0.6875rem, estreita, 0.05em, caixa alta): etiquetas de categoria e de matéria, selo de professor.

### Named Rules
**The One Family Rule.** Archivo é a única família. Hierarquia vem de peso, tamanho e largura; nunca carregar uma segunda fonte nem usar fonte do sistema para títulos.

**The Readable Code Rule.** Todo item que se repete numa lista leva um código curto em Label estreita e numerais tabulares: "Sem 44", "2021·Q12", "nº 13". O código é texto real que identifica o item, não ornamento.

## Layout

Coluna centralizada de até 82rem (`--page-max`) com calha de 2rem, que cai para 1rem abaixo de 768px. O cabeçalho tem 3.5rem e fica fixo no topo; o conteúdo começa 1.5rem abaixo (1rem no celular).

No Início, a faixa do semestre ocupa a largura toda: uma coluna fixa de 11rem com a contagem e, ao lado, uma coluna por semana (mínimo 9rem), separadas por fios verticais de 1px, com fio de 2px em cima e 1px embaixo. Abaixo, grade Bootstrap 8/4 com calha `g-5`: feed à esquerda, lateral "Sua semana" à direita.

Abaixo de 992px a cabeça da faixa vira uma linha (contagem e data lado a lado) e as semanas rolam na horizontal com snap, cerca de duas por tela. Abaixo de 768px a navegação principal sai do cabeçalho e vira uma barra de cinco abas fixa embaixo, respeitando `safe-area-inset-bottom`; o rodapé ganha 4rem de folga para não ficar coberto.

O ritmo vertical é curto e regular: 0.25, 0.375, 0.5, 0.75 e 0.875rem dentro dos itens (0.875rem é o respiro de toda linha de lista), 1.75rem entre módulos, 2rem depois da faixa.

## Elevation & Depth

O sistema é plano. Profundidade vem de fios, de tons de chão (ground, ground-alt) e de tintas de cor, não de sombra. Marcadores de topo usam sombra interna de 2px, que funciona como um fio e não como elevação.

### Shadow Vocabulary
- **Fio interno de topo** (`box-shadow: inset 0 2px 0 var(--text)`): post fixado no feed. Com `var(--you)`, o próximo passo.
- **Contorno do medidor** (`box-shadow: inset 0 0 0 1px var(--rule)`): trilho da barra de acerto por matéria.
- **Anel de foco de campo** (`box-shadow: 0 0 0 0.2rem rgba(var(--bs-link-color-rgb), 0.3)`): foco de botão, campo, select, checkbox e paginação.
- **Painel suspenso** (`box-shadow: 0 0.5rem 1.5rem rgba(11, 18, 34, 0.18)`): o menu da conta, o único elemento que flutua sobre o conteúdo.

### Named Rules
**The Rules Not Cards Rule.** Agrupamento se faz com fio, não com cartão: módulo com 2px de texto no topo, listas com 1px de fio entre linhas. Sombra projetada só em painel que se sobrepõe ao conteúdo (menu suspenso).

## Shapes

Cantos quase retos, de papel impresso. Etiquetas a 0.125rem, blocos da faixa, chips e botões de voto a 0.1875rem, botões e campos a 0.25rem, painel suspenso e blocos de código a 0.375rem. O único círculo é o avatar. As bordas carregam significado: tracejada de 1px é pendente, sólida é entregue, sólida em cinza com título riscado é perdido. A semana omitida é preenchida com listras diagonais de 6px em ground-alt. Ícones são traço SVG de 1.75px (1.6px na barra de abas) com pontas arredondadas.

### Named Rules
**The State By Line Rule.** O estado de um bloco se lê pela borda antes da cor: tracejada pendente, sólida entregue, apagada e riscada perdida. Cor sozinha nunca carrega estado.

## Components

### Buttons
Firmes e sem enfeite, peso 600.
- **Shape:** cantos levemente arredondados ({rounded.md}).
- **Primary:** azul-tinta com texto branco no claro; porcelana com texto quase preto no escuro. Hover clareia a tinta (ink-hover), pressionado escurece (ink-active); no escuro o hover vai para branco puro.
- **Outline:** borda e texto em tinta, enche de tinta no hover. No escuro, borda `#8B97B0` e texto porcelana, enche de porcelana no hover.
- **Header CTA:** "Criar conta" no cabeçalho, branco com texto tinta, peso 700.
- **Focus:** anel de 0.2rem na cor do link a 30%, borda na cor do link.
- **Ícone no cabeçalho:** 2.25rem, sem fundo, hover em branco a 10%; foco com contorno branco interno.

### Chips
- **Filtros do feed:** Label estreita em caixa alta, borda de 1px em fio, texto apagado. Hover escurece borda e texto; o ativo (`aria-current="page"`) inverte: fundo em texto, letra no chão.
- **Etiquetas de categoria:** fundo tingido e texto forte do bloco (Dúvidas índigo, Dicas verde-azulado, Discussão âmbar). Avisos oficiais invertem em texto sobre chão.
- **Etiqueta de matéria:** mesma forma, com a cor do bloco da matéria.
- **Selo de professor / Discursiva:** só contorno de 1px na cor do texto, sem fundo.

### Cards / Containers
- **Módulo:** sem cartão. Fio de 2px na cor do texto no topo, 0.625rem de respiro, título em aba estreita, 1.75rem até o próximo módulo.
- **Próximo passo:** o único bloco com fundo, em coral tingido, com fio interno de 2px coral no topo e botão primário pequeno.
- **Blocos de código no enunciado:** fundo ground-alt, fio de 1px, {rounded.lg}, rolagem horizontal.

### Inputs / Fields
- **Style:** campos do Bootstrap com borda em fio e {rounded.md}.
- **Focus:** borda na cor do link e o mesmo anel de 0.2rem dos botões.

### Navigation
- **Cabeçalho:** marca "ENADE·ADS" em 800, ponto em casca apagada. Seções em 600, 0.9375rem, casca apagada; hover e ativa em branco, ativa com fio branco de 3px embaixo encostado no fim da casca. Transição de cor e borda em 150ms ease-out.
- **Abas de conteúdo (Em alta, Novos, Avisos oficiais):** `nav-underline` do Bootstrap, inativas em texto apagado, ativa em texto com sublinhado na cor do texto.
- **Barra de abas do celular:** cinco itens com ícone de traço (1.375rem) e rótulo de 0.6875rem; ativo com fio de 3px na cor do texto em cima.
- **Menu da conta:** `<details>` sem JavaScript; avatar coral sobre tinta, painel no chão com fio, {rounded.lg} e a única sombra projetada.
- **Paginação:** link na cor do link; página ativa em tinta (claro) ou porcelana (escuro).

### Faixa do semestre (assinatura)
O calendário até a prova desenhado como grade horária. Cabeça com a contagem em Display e a data da prova; uma coluna por semana com código em Label ("Sem 45 · 02/11–08/11"). A semana atual leva fundo coral tingido, barra coral de 4px no topo e o rótulo "Esta semana" em coral. Desafios são blocos com a cor da matéria e a borda dizendo o estado; a prova é um bloco sólido em tinta (porcelana no escuro). Semanas omitidas viram uma coluna listrada "+N semanas". No celular rola de lado com snap.

### Linhas de lista (feed e questões)
Linhas de largura total separadas por fio de 1px, respiro de 0.875rem. No feed: voto à esquerda (2.5rem), à direita metadados (etiqueta, autor, selo, tempo), título, resumo em até três linhas e contagem de comentários em Label. Fixado ganha ground-alt e fio interno de 2px. Nas questões: código, etiqueta da matéria, fonte e resultado; a linha inteira é clicável e ganha ground-alt no hover.

### Voto
Setas preenchidas sem moldura, 2rem por 1.75rem, texto apagado. Hover em ground-alt. Voto a favor dado fica coral; voto contra dado fica na cor do texto. Pontuação em 700 tabular.

### Medidor por matéria
Barra de 0.375rem, trilho em ground-alt com contorno interno de fio, preenchimento na cor do bloco, porcentagem em 700 tabular alinhada à direita.

## Do's and Don'ts

### Do:
- **Do** usar os tokens (`--ink`, `--ground`, `--rule`, `--text`, `--muted`, `--link`, `--you`, `--block-*`) e as variáveis do Bootstrap mapeadas a eles; nunca hex solto em componente novo.
- **Do** definir todo valor novo de cor nos dois temas, em `[data-bs-theme="light"]` e `[data-bs-theme="dark"]`.
- **Do** dar a uma matéria a cor do bloco dela via `StudyBlock.ColorClass()` e `.block-*`, consumindo `--block` e `--block-tint`.
- **Do** agrupar com módulo (fio de 2px e título estreito) e separar linhas com fio de 1px.
- **Do** marcar estado pela borda (tracejada pendente, sólida feito) e acompanhar com texto ("Pendente", "Entregue").
- **Do** usar o componente `Icon` (traço SVG de 1.75px, `currentColor`) para ícones novos.
- **Do** limitar texto corrido a 66–75ch e usar numerais tabulares em contagens, porcentagens e códigos.

### Don't:
- **Don't** usar coral fora do que pertence ao usuário (semana atual, voto dado, próximo passo, avatar).
- **Don't** pôr cor de bloco em elemento sem matéria ou categoria.
- **Don't** criar cartões flutuantes com sombra para conteúdo; sombra projetada só em painel suspenso.
- **Don't** usar o azul padrão do Bootstrap (`#0d6efd`) nem o ciano de `alert-info` sem restilizar pelos tokens.
- **Don't** carregar outra família de fonte, nem Google Fonts ou fonte de terceiros (a CSP só aceita `'self'`).
- **Don't** usar caracteres como ✓, ↷ ou emoji no lugar de ícone.
- **Don't** levar a paleta amarela, as bordas pretas ou a sombra dura do Roadmap para outras páginas.
