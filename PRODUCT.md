# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

- **Alunos de Análise e Desenvolvimento de Sistemas (ADS)** que vão fazer o ENADE. Usam tanto no celular (feed, avisos, uma questão rápida entre aulas ou no ônibus) quanto no computador (sessões longas de questões, simulados e roadmap). Os dois contextos pesam igual.
- **Professores** do curso: publicam avisos oficiais, questões, desafios semanais e moderam a comunidade. Entram como aluno e viram professor depois da aprovação de um administrador (e-mail institucional `.edu`).
- **Administradores**: aprovam professores, gerenciam contas e definem a data da prova.
- **Visitantes sem conta** leem tudo (roadmap, matérias, questões, feed); responder, votar, publicar e comentar exigem login.

## Product Purpose

Plataforma aberta para a turma de ADS se preparar para o ENADE: estudar pelo roadmap das matérias da prova, resolver questões no formato do exame (incluindo as provas reais do Inep de 2017 e 2021) com gabarito comentado, fazer desafios semanais e simulados cronometrados, acompanhar o próprio desempenho e conversar na comunidade. Sucesso é o aluno chegar à prova sabendo onde está fraco e tendo praticado no formato real.

## Positioning

É da turma, não de uma empresa: código aberto (MIT), hospedado pelo próprio curso, com professores reais publicando avisos e questões e alunos tirando dúvidas entre si. Junta o material oficial do Inep, o roadmap do curso e a conversa da turma num lugar só.

## Operating Context

- A prova tem data fixa (ENADE 2026: 29/11/2026), configurável pelo admin; a contagem regressiva é parte da rotina.
- Formato da prova de 2026 (Portaria Inep 169/2026): formação geral e componente específico, objetivas e discursiva, 4 horas.
- Rotinas: desafio semanal publicado pelo professor; simulado cronometrado; avisos do Inep (portarias, editais, datas).
- Feed da comunidade com votos, abas Em alta / Novos / Avisos oficiais, moderação por professores.

## Capabilities and Constraints

- Blazor Web App .NET 10, páginas em SSR estático com formulários (pouca interatividade no cliente), PostgreSQL. Bootstrap é a base de CSS do template e pode ser restilizado.
- Tema claro e escuro com botão de alternar: obrigatório.
- Funciona sem JavaScript para ler e enviar formulários.
- Seções do app: Início (feed), Roadmap, Matérias, Questões, Desafios, Simulados, Meu desempenho, Moderação, Admin (professores pendentes, contas, data da prova), Conta, Termos e Privacidade.

## Brand Commitments

- Nenhuma identidade visual definida pelo usuário além do tema claro/escuro. Nome atual: "ENADE ADS" (pode evoluir).
- Textos em português do Brasil.

## Evidence on Hand

- 79 questões reais do Inep (2017 e 2021) com figuras em `EnadeAds/wwwroot/img/inep/`, mais questões de exemplo.
- Roadmap das matérias do ENADE de ADS (seed em `EnadeAds/Study/RoadmapSeed.cs`).
- Sem depoimentos, números de uso, parceiros ou logotipos de instituições: não inventar.
- Não tem ligação oficial com o Inep; não usar marca do Inep ou do governo.

## Product Principles

1. Ler é livre; agir pede conta.
2. O formato real da prova é o padrão: praticar como vai ser cobrado.
3. A turma fala, o professor confirma: avisos oficiais se distinguem da conversa.
4. Mostrar ao aluno onde ele está fraco vale mais que mostrar quanto ele fez.

## Accessibility & Inclusion

- Contraste AA nos dois temas, navegação por teclado com foco visível, formulários com rótulos e mensagens de erro em texto.
