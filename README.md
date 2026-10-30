# ENADE ADS

Plataforma aberta para estudantes de **Tecnologia em Análise e Desenvolvimento de Sistemas** se prepararem para o ENADE.

## O que a plataforma terá

- **Roadmap de estudos** com os conteúdos da Portaria Inep nº 169/2026 e o progresso de cada aluno salvo na conta.
- **Matérias** organizadas pelos objetos de conhecimento do ENADE.
- **Banco de questões com gabarito** e comentário de cada alternativa.
- **Desafios semanais** e questões novas publicados por professores a qualquer momento.
- **Fórum de avisos** para centralizar comunicados da turma.
- **Cadastro e login** com dois perfis:
  - **Aluno:** todo cadastro entra como aluno.
  - **Professor:** quem marca "Sou professor" e usa e-mail institucional (domínio com `.edu`) fica como *professor pendente* e usa a conta como aluno até um administrador aprovar.

As regras de perfis e de acesso estão em [docs/regras-de-negocio.md](docs/regras-de-negocio.md).

## Questões das provas do Inep

O banco já vem com as questões de ADS do ENADE 2021 e 2017, em `EnadeAds/Data/Seed/inep/*.json`, importadas ao subir a aplicação
(só entram as que ainda não existem; correções feitas pelos professores na tela não são sobrescritas). As figuras ficam em
`EnadeAds/wwwroot/img/inep/`.

- **Fonte:** provas e gabaritos definitivos publicados em [download.inep.gov.br](https://download.inep.gov.br/enade/provas_e_gabaritos/).
- **Licença:** o portal do Inep publica seu conteúdo sob [Creative Commons Atribuição-SemDerivações 3.0](https://creativecommons.org/licenses/by-nd/3.0/deed.pt_BR).
  Por isso enunciados e alternativas são transcritos sem alterações e cada questão cita prova, ano e número.
  Os comentários das alternativas e os padrões de resposta das discursivas são da plataforma (CC BY-SA 4.0); o Inep não publicou padrão de resposta acessível para essas provas.
- **Anuladas:** a questão 15 de 2017 foi anulada pelo Inep e não entra.

## Stack

- Blazor Web App .NET 10 (render mode `InteractiveAuto`), a partir do template [`GentilPedro.Templates.Blazor10`](https://github.com/gentilpedro/template-dotnet-blazor10)
- Bootstrap, tema claro/escuro
- Serilog
- xUnit

## Rodando localmente

Requer o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) e o Docker (para o PostgreSQL).

```bash
docker compose up -d          # PostgreSQL local
dotnet tool restore           # dotnet-ef
dotnet run --project EnadeAds # aplica as migrations ao subir
dotnet test                   # usa SQLite em memória, não precisa do Docker
```

Em desenvolvimento, sem SMTP configurado, o link de confirmação de e-mail aparece na própria tela depois do cadastro.

## Configuração em produção

Use variáveis de ambiente (o `__` separa as seções):

| Variável | Para quê |
|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexão do PostgreSQL |
| `Admin__Emails__0`, `Admin__Emails__1`... | E-mails que recebem o perfil Admin ao se cadastrar |
| `Smtp__Host`, `Smtp__Port`, `Smtp__UserName`, `Smtp__Password`, `Smtp__FromAddress` | Envio dos e-mails de confirmação e de senha |
| `ReverseProxy__KnownNetworks__0` | Rede do proxy reverso (ex.: `172.18.0.0/16`), para ler o IP real do `X-Forwarded-For` |
| `Privacy__ControllerName`, `Privacy__ContactEmail` | Quem responde pelos dados desta instalação e o e-mail para pedidos de titulares (LGPD), mostrados na política de privacidade |
| `RateLimit__AccountPostsPerMinute` | Envios de formulários de conta por IP por minuto (padrão 10) |

Fora de desenvolvimento a aplicação não sobe sem SMTP: sem ele, a confirmação de e-mail não teria como provar que o professor é dono do endereço `.edu`.

Migrations novas: `dotnet ef migrations add <Nome> --project EnadeAds --output-dir Data/Migrations`.

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

[MIT](LICENSE)
