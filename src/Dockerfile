# 1) Build image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
RUN apt-get update

# 2) Install node
# ENV NODE_VERSION v10.18.1
RUN apt-get -y install curl gnupg build-essential
RUN apt-get -qq update && apt-get -qqy --no-install-recommends install wget gnupg \
    git \
    unzip

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

# 3) Copia a pasta source
WORKDIR /src
COPY . .

# 4) Faz o restore do .net
WORKDIR /src/KafkaLearning.Web
RUN dotnet restore

# 5) Baixa os modulos node
WORKDIR /src/KafkaLearning.Web/ClientApp
RUN npm install

# 6) Faz o build da aplicação
WORKDIR /src/KafkaLearning.Web
RUN dotnet build -c Release

# 7) Faz a publicação do código
RUN dotnet publish -c Release -o /app/publish

# 8) Cria a imagem final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=publish /app/publish .

# 9) Executa a aplicação
ENTRYPOINT ["dotnet", "KafkaLearning.Web.dll"]

# EXPOSE 5000
# EXPOSE 5001