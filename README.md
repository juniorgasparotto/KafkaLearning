# KafkaLearning

Esse projeto tem o objetivo de demonstrar como funciona alguns conceitos do Kafka. Por hora, temos os seguintes conceitos pre-configurados:

* Point to Point: Quando um produtor envia uma mensagem e apenas um consumidor fica responsável por ler a mensagem, mesmo que exista mais de um consumidor ouvindo o mesmo tópico.
* Publish/Subscribe: Quando um produtor envia uma mensagem e mais de um consumidor pode ler a mesma mensagem.
* Mecanismos de retentativas:
    * Modelo 1: A mensagem com erro é enviada para um tópico de retentativa com um delay de reprocessamento. Após o delay a mensagem volta para o tópico principal para uma nova tentativa. Após esgotado as tentativas a mensagem vai para o tópico final chamado DLQ.
    * Modelo 2: A mensagem com erro é enviada para um tópico de retentativa com um delay de reprocessamento. Após o delay a mensagem é reprocessada novamente no próprio tópico de retentativa. Após esgotado as tentativas a mensagem vai para o tópico final chamado DLQ.

## Requisitos

* .NET Core 2.1+ (.NET CLI)
* Kafka 2.11-2.2.0+
* NodeJs (npm, angular 7)* 
* Chrome

## Executando

* Fazer o download do Kafka: https://www.apache.org/dyn/closer.cgi?path=/kafka/2.3.0/kafka_2.11-2.3.0.tgz
* Descompactar o Kafka na `C:` (ou qualquer outra pasta)
* Executar o Zookeeper usando a configuração padrão:

```
C:/kafka_2.11-2.3.0/bin/windows/zookeeper-server-start.bat C:/kafka_2.11-2.3.0/config/zookeeper.properties
```

* Executar o broker do Kafka usando a configuração padrão:

```
C:/kafka_2.11-2.3.0/bin/windows/kafka-server-start.bat C:/kafka_2.11-2.3.0/config/server.properties
```

* Fazer o clone desse projeto em seu local de preferência
* Abrir o terminal na raiz do código clonado
* Baixar todos os módulos do angular

```
cd src/KafkaLearning.Web/ClientApp
npm install
```

* Baixar as dependências do .NET e Build

```
dotnet build
```

* Executar o projeto (o angular subirá junto)

```
dotnet run
```

* Abrir no Chrome o url: https://localhost:5001
    * Caso essa porta esteja sendo usada por outro projeto, altere-a.

* Clique no botão `Subscribe All`. O cenário default será o `Publish/Subscribe`
* Envia uma mensagem e note que a mensagem chegará nos dois ouvintes "app1" e "app2".



![change scenario](.\assets\screen.PNG)


## Trocar de cenário

Para trocar de cenário, clique no botão `Change Scenario` e selecione o cenário desejado:

![change scenario](.\assets\btn-change-scenario.PNG)

![change scenario](.\assets\modal-change-scenario.PNG)




## Adicionando um novo cenário:

Outros cenários podem ser inseridos futuramente. O código está bem simples para isso, basta seguir os passos:

## Ferramentas uteis:

* http://www.kafkatool.com/download2/kafkatool.exe: Com essa ferramenta é possível visualizar todos os tópicos de um broker, além dos dados e outras informações importantes.
