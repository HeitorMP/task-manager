# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Instalar o OpenSSL no contêiner (se não estiver presente)
RUN apt-get update && apt-get install -y openssl

WORKDIR /app

# Copiar o projeto e restaurar as dependências
COPY ./TaskApi/*.csproj ./
RUN dotnet restore

# Copiar o restante do código
COPY ./TaskApi/. ./

# Publicar o projeto
RUN dotnet publish -c Release -o /app/publish

# Etapa de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Copiar os arquivos publicados
WORKDIR /app
COPY --from=build /app/publish .

# Criar um script de inicialização para gerar e definir a chave secreta
COPY ./tools/init.sh /app/init.sh
RUN chmod +x /app/init.sh

# Expor a porta
EXPOSE 5170

# Definir o comando de inicialização para rodar o script e depois o aplicativo
ENTRYPOINT ["/app/init.sh"]

