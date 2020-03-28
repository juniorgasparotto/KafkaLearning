import { Component, Inject, OnInit, Input, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-listener',
  templateUrl: './listener.component.html',
  styleUrls: ['./listener.component.css'],
  host: {
    '[class.error]': 'lastError',
  },
})
export class ListenerComponent implements OnInit, AfterViewChecked {
  public message: string;
  public consumerDefaultJson: string;
  public consumerDefault: ConsumerSettigns;
  public messages: EventMessage[];
  private hubConnection: signalR.HubConnection;
  public hasSubcribe: boolean;
  public loading: boolean;
  public lastError: any;
  public appInfo: AppInfo;

  @ViewChild('scrollBottom')
  private myScrollContainer: ElementRef;

  @Input()
  public appName: string;

  @Input()
  public groupId: string;

  @Input()
  public topic: string;

  @Input()
  public retryTopic: string;

  @Input()
  public retryStrategy: string;

  @Input()
  public delay: number;

  @Input()
  public simulateError: boolean;

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.messages = new Array<EventMessage>();
  }

  ngOnInit() {
    this.setSettings();
  }

  public setSettings() {
    this.http.get<AppInfo[]>(this.baseUrl + `api/Server/GetSubscribers?appName=${this.appName}`).subscribe(
      result => {
        if (result.length > 0) {
          var appInfo = result[0];
          this.simulateError = appInfo.simulateError;
          this.consumerDefault = appInfo.settings;
          this.consumerDefaultJson = JSON.stringify(this.consumerDefault, null, 2);
          this.subscribe();
        }
        else {
          this.setSettingsDefault();
        }
      },
      error => {
        console.error(error);
        this.lastError = error;

        this.setSettingsDefault();
      }
    );
  }

  private setSettingsDefault() {
    this.http.get<any>(this.baseUrl + `api/Server/GetConfigs`).subscribe(
      result => {
        this.consumerDefault = {
          bootstrapServers: result.consumers.topicSample.bootstrapServers,
          groupId: this.groupId,
          enableAutoCommit: environment.kafka.consumerDefault.enableAutoCommit,
          enablePartitionEof: environment.kafka.consumerDefault.enablePartitionEof,
          topic: this.topic,
          retryTopic: this.retryTopic,
          retryStrategy: this.retryStrategy,
          autoOffSetReset: environment.kafka.consumerDefault.autoOffSetReset * 1,
          delay: this.delay * 1
        };

        if (!this.retryTopic)
          delete this.consumerDefault.retryTopic;

        if (!this.delay)
          delete this.consumerDefault.delay;

        this.consumerDefaultJson = JSON.stringify(this.consumerDefault, null, 2);
      },
      error => {
        console.error(error);
        this.lastError = error;
      }
    );
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  public subscribe() {
    this.loading = true;
    this.lastError = null;
    this.consumerDefault = JSON.parse(this.consumerDefaultJson);

    this.http.post<AppInfo>(this.baseUrl + `api/Server/Subscribe?appName=${this.appName}&simulateError=${this.simulateError}`, this.consumerDefault).subscribe(
      result => {
        this.appInfo = result;
        this.listen();
      },
      error => {
        console.error(error);
        this.loading = false;
        this.lastError = error;
      }
    );
  }

  public unSubscribe() {
    this.loading = true;
    this.lastError = null;

    this.http.get(this.baseUrl + `api/Server/UnSubscribe?appName=${this.appName}`).subscribe(
      result => {
        this.hasSubcribe = false;
        try {
          this.hubConnection
            .stop()
            .catch(error => {
              console.log(error);
              this.lastError = error;
            });
        }
        catch (e) {
          // this.lastError = e;
        }

        this.loading = false;
      },
      error => {
        console.error(error);
        this.loading = false;
        this.lastError = error;
      }
    );
  }

  public clear() {
    this.messages = new Array<EventMessage>();
  }

  private listen() {
    // Não conecta novamente se já estiver escrito, do contrário gera duplicação de ouvintes
    if (this.hasSubcribe) {
      this.loading = false;
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'signalr/messages', {
        // skipNegotiation: true,
        // transport: signalR.HttpTransportType.WebSockets
      })
      // .configureLogging(LogLevel.Debug)
      .build();

    this.hubConnection.onclose(error => {
      console.log(error);
      this.lastError = error;
    });

    this.hubConnection.start()
      .then(() => {
        console.log('Connection started');

        this.hubConnection.invoke("AddToGroup", this.appName)
          .then(() => {
            this.hasSubcribe = true;
            this.loading = false;

            this.hubConnection.on('message', (msg: EventMessage) => {
              console.log(msg);
              this.messages.push(msg);
            });

            this.hubConnection.on('messageError', (msg: EventMessage) => {
              console.log(msg);
              msg.hasError = true;
              this.messages.push(msg);
            });
          })
          .catch(err => {
            console.log(err);
            this.loading = false;
            this.lastError = err;
          });
      })
      .catch(err => {
        this.loading = false;
        this.lastError = err;
        console.log('Error while starting connection: ' + err);
      });
  }

  public setSimulateError() {
    this.http.get(this.baseUrl + `api/Server/EnableError?appName=${this.appName}&value=${this.simulateError}`).subscribe(
      result => {
      },
      error => {
        console.error(error);
        this.lastError = error;
      }
    );
  }

  public isSubscribe(): boolean {
    return this.hasSubcribe;
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}

interface AppInfo {
  appName: string;
  taskId: string;
  simulateError: boolean;
  settings: ConsumerSettigns;
}

interface EventMessage {
  id: string;
  sendDate: Date;
  receiveDate: Date;
  message: string;
  appName: string;
  hasError: boolean;
}

interface ConsumerSettigns {
  bootstrapServers: string;
  groupId: string;
  enableAutoCommit: boolean;
  autoOffSetReset: number;
  enablePartitionEof: boolean;
  topic: string;
  retryStrategy: string;
  retryTopic: string;
  delay: number;
}