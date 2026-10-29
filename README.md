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

Fora de desenvolvimento a aplicação não sobe sem SMTP: sem ele, a confirmação de e-mail não teria como provar que o professor é dono do endereço `.edu`.

Migrations novas: `dotnet ef migrations add <Nome> --project EnadeAds --output-dir Data/Migrations`.

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

[MIT](LICENSE)
