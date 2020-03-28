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

# cd /d/Junior/Projetos/KafkaLearning/src; docker build . --tag kafka-learning:1.0
# winpty docker run -e ASPNETCORE_URLS="http://0.0.0.0:5000" -ti -p 5000:5000 kafka-learning:1.0
# winpty docker run --net glauberj_default -e Kafka__CertificatePath="" -e "Kafka__Producers__TopicSample__BootstrapServers=glauberj_kafka_1:9092" -e "Kafka__Consumers__TopicSample__BootstrapServers=glauberj_kafka_1:9092" -e ASPNETCORE_URLS="http://0.0.0.0:5000" -ti -p 5000:5000 kafka-learning:1.0
# cd D:\Junior\Projetos\KafkaLearning\kafka_2.12-2.2.0
# ./bin/windows/zookeeper-server-start.bat ./config/zookeeper.properties
# ./bin/windows/kafka-server-start.bat ./config/server.properties

# docker tag kafka-learning:1.0 juniorgasparotto/kafka-learning:latest
# docker push juniorgasparotto/kafka-learning:latest
# export ASPNETCORE_ENVIRONMENT=Development
# "Kafka__Consumers__TopicSample__BootstrapServers": "TESTE 1",
# "Kafka__Producers__TopicSample__BootstrapServers": "TESTE 2",