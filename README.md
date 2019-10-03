# KafkaLearning

Esse projeto tem o objetivo de demonstrar como funciona alguns conceitos do Kafka. Por hora, temos os seguintes conceitos pre-configurados:

* `Point to Point`: Quando um produtor envia uma mensagem e apenas um consumidor fica responsável por ler a mensagem, mesmo que exista mais de um consumidor ouvindo o mesmo tópico.
* `Publish/Subscribe`: Quando um produtor envia uma mensagem e mais de um consumidor pode ler a mesma mensagem.
* `Retry`:
    * Modelo 1: A mensagem com erro é enviada para um tópico de retentativa com um delay de reprocessamento. Após o delay a mensagem volta para o tópico principal para uma nova tentativa. Após esgotado as tentativas a mensagem vai para o tópico final chamado DLQ.
    * Modelo 2: A mensagem com erro é enviada para um tópico de retentativa com um delay de reprocessamento. Após o delay a mensagem é reprocessada novamente no próprio tópico de retentativa. Após esgotado as tentativas a mensagem vai para o tópico final chamado DLQ.

## Requisitos

* .NET Core 2.1+ (.NET CLI)
* Kafka 2.11-2.2.0+
* NodeJs (npm, angular 7)
* Chrome (Com suporte a WebSocket/SignalR)

## Executando

* Fazer o download do Kafka: https://www.apache.org/dyn/closer.cgi?path=/kafka/2.3.0/kafka_2.11-2.3.0.tgz
* Descompactar o Kafka na pasta `C:\` (ou qualquer outra pasta)
* Executar o Zookeeper usando a configuração padrão:

```
C:/kafka_2.11-2.3.0/bin/windows/zookeeper-server-start.bat C:/kafka_2.11-2.3.0/config/zookeeper.properties
```

* Executar o broker do Kafka usando a configuração padrão:

```
C:/kafka_2.11-2.3.0/bin/windows/kafka-server-start.bat C:/kafka_2.11-2.3.0/config/server.properties
```

* Fazer o clone desse projeto em seu local de preferência

```
git clone https://github.com/juniorgasparotto/KafkaLearning.git
```

* Abrir o terminal na raiz do código clonado
* Baixar todos os módulos do angular

```
cd src/KafkaLearning.Web/ClientApp
npm install
```

* Baixar as dependências do .NET e Build

```
cd src/KafkaLearning.Web
dotnet build
```

* Executar o projeto (o angular subirá junto)

```
cd src/KafkaLearning.Web
dotnet run
```

* Abrir no Chrome o url: https://localhost:5001. Você deve visualizar a seguinte tela:
    * Caso essa porta esteja sendo usada por outro projeto, altere-a.

![change scenario](./assets/screen.PNG)

* Clique no botão `Subscribe All`. O cenário padrão será o `Publish/Subscribe`
* Envia uma mensagem clicando no botão `Send` e note que a mensagem chegará nos dois ouvintes `app1` e `app2`.

## Trocar de cenário

Para trocar de cenário, clique no botão `Change Scenario` e selecione o cenário desejado:

![change scenario](./assets/btn-change-scenario.PNG)

![change scenario](./assets/modal-change-scenario.PNG)

## Adicionando um novo cenário:

Outros cenários podem ser inseridos futuramente. O código está bem simples para isso, basta seguir os passos:

* Crie um componente angular na pasta `src/KafkaLearning.Web/ClientApp/src/app/scenarios`: 

```
ng g c ScenarioMyCustomTest
```

* Copie o código do componente existente `scenario-point-to-point/scenario/point-to-point.component.ts` e cole no arquivo `scenario-my-custom-test.component.ts` do novo componente, mantendo apenas o nome da classe do novo cenário.

* Altere o nome da pasta e o título do novo componente 

```typescript
  public static FOLDER: string = "scenario-my-custom-test";
  public static TITLE: string = "My custom test";
  public static TITLE_PT_BR: string = "Meu cenário customizado";
```

* Abra o arquivo de template e adicione os ouvintes com a sua configuração desejada. 
    * Mantenha a pai `<div class="subscribers">` para não quebrar o layout. 

```html
<div class="subscribers">
  <app-listener appName="app1" groupId="g1" topic="Chat" [simulateError]="false"></app-listener>
  <app-listener appName="app2" groupId="g1" topic="Chat" [simulateError]="false"></app-listener>
</div>
```

* Crie um arquivo chamado `description.html` na raiz do novo componente. Caso queira manter dois idiomas, crie o arquivo `description-pt-br.html`. Esse arquivo é HTML puro e deve conter a descrição do cenário.

* Localize o arquivo `src/KafkaLearning.Web/ClientApp/src/app/modal-scenarios/modal-scenarios.component.ts` para adicionar o cenário na modal de escolha. 

* Adicione o novo cenário no final do array `TABS`

```typescript
private static TABS: any[] = [
    { component: ScenarioPointToPointComponent, active: false },
    { component: ScenarioPublishSubscribeComponent, active: false },
    { component: ScenarioRetryMainTopicComponent, active: false },
    { component: ScenarioRetryNextTopicComponent, active: false },
    
    { component: ScenarioMyCustomTest, active: false },
    //  ^^^^^

  ];
```

* Faça o build do angular com `ng build` e execute novamente o projeto.

* Pronto, seu novo componente deve aparecer na modal e já pode ser utilizado.

## Configurações do ouvinte

As configurações do componente/ouvinte `app-listener` tem uma relação direta com as configurações do `Kafka` e outras configurações de `retry` que criamos, são elas:

```html
<app-listener 
    appName="APP_NAME" 
    topic="TOPIC_NAME" 
    groupId="GROUP_ID" 
    simulateError="true|false"
    retryStrategy="retry|redirect" 
    retryTopic="RETRY_TOPIC_NAME" 
    delay="DELAY_IN_MILLISECONDS" 
    ></app-listener>
```

* `appName`: Nome da aplicação de simulação
* `topic`: Nome do tópico do `Kafka`
* `groupId`: Nome do grupo de consumo do `Kafka`
* `simulateError`: Indica se o ouvinte deve emitir um erro ou não quando ele ler uma mensagem. 
    * Dica: Cenários de erros devem sempre ser considerados em seus `Handlers`, assim como a estratégia de retry, caso necessite. 
* `retryStrategy`: Define qual será a estratégia de retry desse ouvinte/consumidor
    * `retry`: Quando definido com este valor e ocorrer um erro no consumidor, o código vai criar ou atualizar o cabeçalho `retry.count` incrementando `+1` no valor e encaminhar a mensagem para o tópico que foi definido na configuração `retryTopic`.
    * `redirect`: Quando definido com este valor o código não fará nada com a mensagem, apenas fará o redirecionamento da mensagem para o tópico que foi definido na configuração `retryTopic`.
* `retryTopic`: Tópico de destino em caso de erro ou quando a configuração `retryStrategy` for igual a `redirect`.
* `delay`: Quando definido, o consumidor criará um atraso na leitura da mensagem, isso é útil para criar retentativas exponenciais. 

OBS: Talvez o uso do valor `redirect` na configuração `retryStrategy` não faça sentido, verificar se não seria melhor criar algo como: `handler=none|redirect` e `handle-args=REDIRECT_TOPIC_NAME`.

## Melhorias

* Criar uma classe abstrata para todos os cenários para não precisar copiar o mesmo código toda vez que cria um novo cenário
* Enviar os dados do produtor para dentro do cenário
* Renomear as classes de `ConsumerClient` para `Listener` e simplificar mecanismo.

## Ferramentas uteis:

* http://www.kafkatool.com/download2/kafkatool.exe: Com essa ferramenta é possível visualizar todos os tópicos de um broker, além dos dados e outras informações importantes.
