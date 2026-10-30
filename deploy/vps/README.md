# Deploy na VPS

- **URL:** https://rumo.aamora.com.br (HTTP redireciona para HTTPS)
- **App:** `rumo`, em `/opt/rumo` (`docker-compose.yml`, `deploy.conf`, `.env`, `secrets.env`, `.version`, `backups/`)
- **Porta:** `127.0.0.1:5106` → container na 8080; sub-rede `172.30.5.0/24` (gateway `172.30.5.1`, o único proxy em que o app confia)
- **Banco:** PostgreSQL 18 no container `db`, volume `rumo_pgdata`. As migrations rodam quando o app sobe.
- **Volumes no backup diário:** banco e `dpkeys` (chaves de login e formulários; sem elas, cada deploy tira todo mundo da conta)
- **Nginx do host:** `/etc/nginx/conf.d/rumo.conf` (cópia de `nginx.conf`), com timeout de 1h para o SignalR do Blazor
- **Deploy:** publicar uma tag `vX.Y.Z` dispara o job `deploy-vps` do `.github/workflows/ci-cd.yml`. Se `/health` não responder, o `app-deploy` volta para a versão anterior sozinho.

## Segredos (GitHub → Settings → Secrets and variables → Actions)

`SMTP__HOST`, `SMTP__USERNAME`, `SMTP__PASSWORD`, `SMTP__FROMADDRESS`, `ADMIN__EMAILS__0`, `PRIVACY__CONTROLLERNAME`, `PRIVACY__CONTACTEMAIL`. Segredo não cadastrado fica de fora do `secrets.env`. Sem os `SMTP__*` o app sobe, mas não envia e-mail, então ninguém confirma conta nova até eles existirem; depois de cadastrar, rode o deploy de novo (nova tag ou *Re-run* do último).
`VPS_HOST`, `VPS_SSH_KEY` e `VPS_KNOWN_HOSTS` são gravados pelo `chave_deploy.sh rumo` da skill deploy-vps.

## Operação

```bash
ssh vps 'app-deploy rumo status'      # versão no ar e imagens guardadas
ssh vps 'app-deploy rumo up 1.0.0'    # rollback para uma imagem guardada
ssh vps 'app-backup rumo'             # backup na hora
```

Mudou `docker-compose.yml`, `deploy.conf` ou `nginx.conf`? Rode de novo o `preparar_app.sh rumo` da skill deploy-vps: os arquivos desta pasta não vão sozinhos para o servidor.
