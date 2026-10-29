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

Requer o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet restore
dotnet run --project EnadeAds
dotnet test
```

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md).

## Licença

[MIT](LICENSE)
