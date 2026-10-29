# Como contribuir

Contribuições de estudantes e professores são bem-vindas.

## Fluxo

1. Abra (ou escolha) uma **issue** descrevendo o problema ou a melhoria.
2. Crie uma branch a partir de `main`:
   - `feature/<numero>-<descricao>` para funcionalidades
   - `fix/<numero>-<descricao>` para correções
3. Faça commits pequenos no padrão [Conventional Commits](https://www.conventionalcommits.org/pt-br/), em português e no imperativo (`feat: adiciona banco de questões`).
4. Rode `dotnet build` e `dotnet test` antes de abrir o PR.
5. Abra um **Pull Request** para `main` começando com `Closes #<numero>`.

## Regras da branch `main`

- Ninguém faz push direto na `main`, nem os administradores.
- Todo merge exige Pull Request com aprovação de um mantenedor (`CODEOWNERS`) e o CI verde.
- Os commits da `main` precisam ser **assinados** (GPG ou SSH). Veja como configurar em [Signing commits](https://docs.github.com/pt/authentication/managing-commit-signature-verification/signing-commits).
- Force-push e exclusão da `main` estão bloqueados.

## Questões do ENADE

Ao contribuir com questões, informe a fonte (prova e ano do Inep, ou autoria própria). Não envie material protegido por direitos autorais de terceiros sem permissão.
