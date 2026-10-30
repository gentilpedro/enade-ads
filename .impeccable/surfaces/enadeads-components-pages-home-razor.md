---
version: 1
slug: "enadeads-components-pages-home-razor"
primary_target: "EnadeAds/Components/Pages/Home.razor"
related_targets: ["EnadeAds/Components/Layout/MainLayout.razor"]
---

# Casca do app e Início (feed)

Escopo: a casca de todas as páginas (navegação, cabeçalho, rodapé, tokens) e a página inicial com o feed da comunidade. Modo: Operate. Usuário: aluno de ADS no celular (pausas curtas) e no computador (sessões longas), professores publicando avisos. Tarefa: ver o que há de novo, saber quanto falta para a prova e o que estudar a seguir, publicar, votar, comentar.

## Direction contract

THESIS: A preparação como a grade do semestre da faculdade: cada semana até a prova é uma coluna, cada matéria tem sua cor. Recusa a casca de template com barra lateral fixa e cartões flutuantes de app de comunidade genérico.

OWN-WORLD: Casca azul-tinta (#14213D) com texto claro; chão branco com fios finos (#D9DEE7) dividindo módulos, que levam uma pequena aba de título em Archivo estreita, caixa alta. Archivo (eixo de largura) é a única família: normal para texto, estreita para rótulos, códigos e cabeçalhos de coluna. Cor de matéria (verde-azulado, índigo, âmbar, verde) só onde há matéria ou categoria; coral (#C8401F) reservado para "você": sua semana, seu voto, seu próximo passo. Estado pela forma da linha: sólida feito, tracejada pendente. Todo item com código legível (Sem 44, 2021·Q12).

STORY: O aluno abre e vê, sem rolar, quanto falta para a prova e o que cai em cada semana; lê o que a turma e os professores publicaram; sabe qual matéria atacar agora e vai direto às questões dela.

FIRST VIEWPORT: Barra superior azul-tinta de 56px: marca à esquerda, seções no meio, tema e conta à direita. Logo abaixo, a faixa do semestre na largura toda: rótulo "Até a prova / N dias" e uma coluna por semana até 29/11, semana atual com barra coral e fundo tingido, prova como bloco azul-tinta no fim, desafios dentro das semanas. Abaixo, duas colunas: feed (título Comunidade, botão Criar post, abas Em alta/Novos/Avisos oficiais, linhas com voto, etiqueta, título, resumo) e lateral "Sua semana" com barras por matéria e o próximo passo. No celular: faixa rolável na horizontal, feed em coluna, abas fixas embaixo.

FORM: Grade horária e calendário acadêmico, candidato 3 de 7 na lista ordenada; seed:f0c5a990456f5d3e

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance

## Raises

- Do mapa de orientação: uma cor reservada só para você, nunca decorativa.
- Da edição de referência: enunciado em medida fixa, gabarito comentado na margem no computador.
- Da web japonesa densa: módulos com fios finos e aba de título, sem cartões flutuantes.
- Do catálogo de evento: todo item com código legível.
- Do trilho espectral: estado também pela forma da linha.

## Assinatura

A faixa do semestre: o calendário até a prova desenhado como a grade horária, com a semana atual marcada em coral e cada desafio na sua semana (sólido se feito, tracejado se pendente).
