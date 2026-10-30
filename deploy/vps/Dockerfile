# Imagem de produção da VPS (Rumo, Blazor .NET 10). Contexto = raiz do repositório:
#   docker build -f deploy/vps/Dockerfile --build-arg VERSAO=1.2.3 -t rumo:1.2.3 .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG VERSAO=0.0.0
WORKDIR /src

# Restore separado do código: muda pouco e aproveita o cache das camadas.
COPY nuget.config EnadeAds.sln ./
COPY EnadeAds/EnadeAds.csproj EnadeAds/
COPY EnadeAds.Client/EnadeAds.Client.csproj EnadeAds.Client/
RUN dotnet restore EnadeAds/EnadeAds.csproj

COPY EnadeAds/ EnadeAds/
COPY EnadeAds.Client/ EnadeAds.Client/
# O projeto web é o principal; o .Client (WebAssembly) vai junto pela referência.
RUN dotnet publish EnadeAds/EnadeAds.csproj -c Release -o /app --no-restore -p:Version=$VERSAO

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# O usuário não-root ($APP_UID = 1654) escreve em dois lugares:
# - logs/ do Serilog (arquivo diário, além do console);
# - /keys, as chaves de Data Protection (DataProtection:KeysPath no compose), que ficam num volume
#   para o login e os formulários sobreviverem a cada deploy.
RUN mkdir -p /app/logs /keys && chown $APP_UID:$APP_UID /app/logs /keys
USER $APP_UID
# A imagem base já escuta na 8080 (ASPNETCORE_HTTP_PORTS).
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnadeAds.dll"]
