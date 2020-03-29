<a href="https://github.com/juniorgasparotto/KafkaLearning" rel="some text">![change scenario](https://raw.githubusercontent.com/juniorgasparotto/KafkaLearning/master/assets/en-us.png)</a>
<a href="https://github.com/juniorgasparotto/KafkaLearning/blob/master/README-PT-BR.md" rel="some text">![change scenario](https://raw.githubusercontent.com/juniorgasparotto/KafkaLearning/master/assets/pt-br.png)</a>

# KafkaLearning

This project aims to demonstrate how some Kafka concepts work. For now, we have the following preconfigured concepts:

* `Point to Point`: When a producer sends a message and only one consumer is responsible for reading the message, even if there is more than one consumer listening to the same topic.
* `Publish/Subscribe`: When a producer sends a message and more than one consumer can read the same message.
* `Retry`:
    * Model 1: The error message is sent to a retry topic with a reprocessing delay. After the delay the message returns to the main topic for a retry. After the attempts are exhausted the message goes to the final topic called DLQ.
    * Model 2: The error message is sent to a retry topic with a reprocessing delay. After the delay the message is reprocessed again in the retry topic itself. After the attempts are exhausted the message goes to the final topic called DLQ.

## Requirements

* .NET Core 2.1+ (.NET CLI)
* Kafka 2.11-2.2.0+
* NodeJs (npm, angular 7)
* Chrome (With WebSocket / SignalR Support)

## Running with docker

To run using docker and without installing anything on your host machine.

```
curl -sSL https://raw.githubusercontent.com/juniorgasparotto/KafkaLearning/master/src/docker-compose.yml > docker-compose.yml
docker-compose up -d
```

### Build a new image

Steps to build a new image:

```
cd /src
docker build . --tag juniorgasparotto/kafka-learning:1.0
docker run -e Kafka__CertificatePath="" -e "Kafka__Producers__TopicSample__BootstrapServers=localhost:9092" -e "Kafka__Consumers__TopicSample__BootstrapServers=localhost:9092" -e ASPNETCORE_URLS="http://0.0.0.0:5000" -ti -p 5000:5000 kafka-learning:1.0
docker tag kafka-learning:1.0 juniorgasparotto/kafka-learning:1.0
docker push juniorgasparotto/kafka-learning:1.0
```

## Running as developer

* Download Kafka: https://www.apache.org/dyn/closer.cgi?path=/kafka/2.3.0/kafka_2.11-2.3.0.tgz
* Unzip Kafka in the `C:\` folder (or any other folder)
* Run Zookeeper using the default setting:

```
C:/kafka_2.11-2.3.0/
./bin/windows/zookeeper-server-start.bat ./config/zookeeper.properties
```

* Run Kafka broker using the default setting:

```
C:/kafka_2.11-2.3.0/
./bin/windows/kafka-server-start.bat ./config/server.properties
```

* Clone this project to your preferred location

```
git clone https://github.com/juniorgasparotto/KafkaLearning.git
```

* Open terminal at root of cloned code
* Download all angular modules

```
cd src/KafkaLearning.Web/ClientApp
npm install
```

* Download .NET and Build Dependencies

```
cd src/KafkaLearning.Web
dotnet build
```

* Run the project (the angle will go up together)

```
cd src/KafkaLearning.Web
dotnet run
```

* Open in Chrome the url: https://localhost:5001. You should see the following screen:
    * If this port is being used by another project, change it.

![change scenario](./assets/screen.PNG)

* Click on the `Subscribe All` button. The default scenario will be `Publish / Subscribe`
* Send a message by clicking the `Send` button and note that the message will arrive at both the` app1` and `app2` listeners.

## Change default settings (example: Kafka's default URL)

* Environment `development`:
  * .NET: If you don't need to use a certificate (usually localhost: 9092 doesn't need to): Open the file `\src\KafkaLearning.Web\appsettings.json` and remove the certificate configuration in the property:` Kafka -> CertificatePath: null`.
  * .NET: If certificate use is required: replace the certificate file in the following folder with your certificate: `\src\KafkaLearning.Web\Certificates\ca.crt`. If you change the file name of the certificate, change the path in the `CertificatePath` property and ensure that this new file is being copied to the` bin` folder in the build process: `\src\KafkaLearning.Web\KafkaLearning.Web.csproj`.
  * Angular: Open the file `\src\KafkaLearning.Web\ClientApp\src\environments\environment.ts` and change the default URL or any other information.
* Environment `production`:
  * .NET: Same procedure as in the DEV environment, however, use the file `appsettings.production.json`.
  * Open the file `\src\KafkaLearning.Web\ClientApp\src\environments\environment.prod.ts` and exchange standard URL or any other information.

## Change scenery

To switch scenarios, click on the `Change Scenario` button and select the desired scenario:

![change scenario](./assets/btn-change-scenario.PNG)

![change scenario](./assets/modal-change-scenario.PNG)

## Adding a new scenario:

Other scenarios may be inserted in the future. The code is very simple for that, just follow the steps:

* Create an angular component in the folder `src/KafkaLearning.Web/ClientApp/src/app/scenarios`: 

```
ng g c ScenarioMyCustomTest
```

* Copy existing component code `scenario-point-to-point/scenario/point-to-point.component.ts` and paste in the file `scenario-my-custom-test.component.ts` of the new component, keeping only the class name of the new scenario.

* Change folder name and new component title

```typescript
  public static FOLDER: string = "scenario-my-custom-test";
  public static TITLE: string = "My custom test";
  public static TITLE_PT_BR: string = "Meu cenário customizado";
```

* Open the template file and add the listeners with your desired setting.
    * Keep the parent `<div class="subscribers">` so as not to break the layout.

```html
<div class="subscribers">
  <app-listener appName="app1" groupId="g1" topic="Chat" [simulateError]="false"></app-listener>
  <app-listener appName="app2" groupId="g1" topic="Chat" [simulateError]="false"></app-listener>
</div>
```

* Create a file named `description.html` in the root of the new component. If you want to keep two languages, create the `description-pt-br.html` file. This file is pure HTML and must contain the description of the scenario.

* Locate the file `src/KafkaLearning.Web/ClientApp/src/app/modal-scenarios/modal-scenarios.component.ts` to add the scenario in the modal of choice.

* Add new scenario at end of `TABS` array

```typescript
private static TABS: any[] = [
    { name: 'ScenarioPointToPointComponent', component: ScenarioPointToPointComponent, active: false },
    { name: 'ScenarioPublishSubscribeComponent', component: ScenarioPublishSubscribeComponent, active: false },
    { name: 'ScenarioRetryMainTopicComponent', component: ScenarioRetryMainTopicComponent, active: false },
    { name: 'ScenarioRetryNextTopicComponent', component: ScenarioRetryNextTopicComponent, active: false },
    
    // new scenario
    { name: 'ScenarioMyCustomTest', component: ScenarioMyCustomTest, active: false },
  ];
```

* Build the angular with `ng build` and rerun the project.

* There, your new component should appear in modal and can now be used.

## Listener Settings

The `app-listener` component/listener settings have a direct relationship to the` Kafka` settings and other `retry` settings we have created, they are:

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

* `appName`: Simulation App Name
* `topic`: Kafka Topic Name
* `groupId`: Kafka Consumer Group Name
* `simulateError`: Indicates whether or not the listener should issue an error when reading a message.
    * Tip: Error scenarios should always be considered in your `Handlers`, as well as the retry strategy if you need to.
* `retryStrategy`: Defines what this listener / consumer retry strategy will be
    * `retry`: When set to this value and a consumer error occurs, the code will create or update the `retry.count` header by incrementing` + 1` in the value and forwarding the message to the topic that was set in the `retryTopic` setting.
    * `redirect`: When set to this value the code will do nothing with the message, it will only redirect the message to the topic that was set in the `retryTopic` setting.
* `retryTopic`: Destination topic on error or when setting `retryStrategy` equals` redirect`.
* `delay`: When set, the consumer will create a delay in reading the message, which is useful for creating exponential retries.

NOTE: Maybe using the `redirect` value in the` retryStrategy` setting does not make sense, check if it would be better to create something like: `handler=none|redirect` and` handle-args=REDIRECT_TOPIC_NAME`.

## Installing on OpenShift (OKD)

Before installing KafkaLearning, it is necessary to have a Kafka service that can be accessible from within the cluster. Or use the `Strimzi` project to install a Kafka cluster within your cluster:

https://strimzi.io/quickstarts/okd/

**Useful links:**

* https://github.com/redhat-developer/s2i-dotnetcore
* https://docs.openshift.com/container-platform/3.7/dev_guide/application_lifecycle/new_app.html

**Installation step by step:**

* Create the openshift secret to access the RedHat registry. You need an active RedHat registry: https://access.redhat.com/

```
oc create secret docker-registry redhat-registry \
    --docker-server=registry.redhat.io \
    --docker-username=<user> \
    --docker-password=<pwd> \
    --docker-email=<email> \
    -n openshift
```

* Install ImageStream

```
oc create -f https://raw.githubusercontent.com/redhat-developer/s2i-dotnetcore/master/dotnet_imagestreams.json
```

* Or update

```
oc replace -f https://raw.githubusercontent.com/redhat-developer/s2i-dotnetcore/master/dotnet_imagestreams.json
```

* Create a project to contain KafkaLearning

```
oc new-project project-kafka
```

* Or just select a project in which you want it to contain it

```
oc project [project-name]
```

* Create the Kafka Learning application

```
oc new-app 'dotnet:3.1~https://github.com/juniorgasparotto/KafkaLearning.git' \
--name=kafka-learning \
--context-dir src \
--build-env DOTNET_STARTUP_PROJECT=KafkaLearning.Web/KafkaLearning.Web.csproj \
--build-env DOTNET_CONFIGURATION=Release
```

* Follow the image build log

```
oc logs -f bc/kafka-learning
```

* Follow the image deployment log

```
oc logs -f dc/kafka-learning
```

* Exposes a route to gain access from outside the cluster

```
oc expose svc/kafka-learning
```

* Obtain the route address and check outside the cluster that everything is working (Requested Host: <rout>)

```
oc describe route kafka-learning
```

* To see all the objects created, use:

```
oc get all -l app=kafka-learning
```

* If you want to remove Kafka-Learning and all its objects, do:

```
oc delete all -l app=kafka-learning
```

* It may be that in the first "Subscribe" you receive errors, this is because all .NET dependencies have not yet risen completely.

## Improvements

* Create an abstract class for all scenarios so you don't have to copy the same code every time you create a new scenario
* Send producer data into scenario
* Rename the classes from `ConsumerClient` to` Listener` and simplify mechanism.
* Leave the Kafka URL to be overwritten by an environment variable.

## Useful tools:

* http://www.kafkatool.com/download2/kafkatool.exe: With this tool you can view all topics of a broker, as well as data and other important information.