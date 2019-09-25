import { Component, Inject, OnInit, Input, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";

@Component({
  selector: 'app-chat-room',
  templateUrl: './chat-room.component.html',
  styleUrls: ['./chat-room.component.css'],
  host: {
    '[class.error]': 'lastError',
  },
})
export class ChatRoomComponent implements OnInit, AfterViewChecked {
  public message: string;
  public consumerSettignsJson: string;
  public consumerSettigns: ConsumerSettigns;
  public messages: ChatMessage[];
  private hubConnection: signalR.HubConnection;
  private hasSubcribe: boolean;
  private loading: boolean;
  private lastError: any;
  private appInfo: AppInfo;

  @ViewChild('scrollBottom')
  private myScrollContainer: ElementRef;

  @Input()
  public roomId: string;

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
    this.messages = new Array<ChatMessage>();
  }

  ngOnInit() {
    this.setSettings();
  }

  public setSettings() {
    this.http.get<AppInfo[]>(this.baseUrl + `api/Chat/GetSubscribers?appName=${this.roomId}`).subscribe(
      result => {
        if (result.length > 0) {
          var appInfo = result[0];
          this.simulateError = appInfo.simulateError;
          this.consumerSettigns = appInfo.settings;
          this.consumerSettignsJson = JSON.stringify(this.consumerSettigns, null, 2);
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
    this.consumerSettigns = {
      bootstrapServers: "localhost:9092",
      groupId: this.groupId,
      enableAutoCommit: true,
      autoOffSetReset: 0,
      enablePartionEof: true,
      topic: this.topic,
      retryTopic: this.retryTopic,
      retryStrategy: this.retryStrategy,
      delay: this.delay
    };
    
    if (!this.retryTopic)
      delete this.consumerSettigns.retryTopic;

    if (!this.delay)
      delete this.consumerSettigns.delay;

    this.consumerSettignsJson = JSON.stringify(this.consumerSettigns, null, 2);
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  public subscribe() {
    this.loading = true;
    this.lastError = null;
    this.consumerSettigns = JSON.parse(this.consumerSettignsJson);

    this.http.post<AppInfo>(this.baseUrl + `api/Chat/Subscribe?roomId=${this.roomId}&simulateError=${this.simulateError}`, this.consumerSettigns).subscribe(
      result => {
        this.appInfo = result;
        this.listenChat();
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

    this.http.get(this.baseUrl + `api/Chat/UnSubscribe?roomId=${this.roomId}`).subscribe(
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
    this.messages = new Array<ChatMessage>();
  }

  private listenChat() {
    // Não conecta novamente se já estiver escrito, do contrário gera duplicação de ouvintes
    if (this.hasSubcribe) {
      this.loading = false;
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'signalr/chat', {
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

        this.hubConnection.invoke("AddToGroup", this.roomId)
          .then(() => {
            this.hasSubcribe = true;
            this.loading = false;

            this.hubConnection.on('chat', (msg: ChatMessage) => {
              console.log(msg);
              this.messages.push(msg);
            });

            this.hubConnection.on('chatError', (msg: ChatMessage) => {
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
    this.http.get(this.baseUrl + `api/Chat/EnableError?appName=${this.roomId}&value=${this.simulateError}`).subscribe(
      result => {
      },
      error => {
        console.error(error);
        this.lastError = error;
      }
    );
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

interface ChatMessage {
  id: string;
  sendDate: Date;
  receiveDate: Date;
  message: string;
  roomId: string;
  hasError: boolean;
}

interface ConsumerSettigns {
  bootstrapServers: string;
  groupId: string;
  enableAutoCommit: boolean;
  autoOffSetReset: number;
  enablePartionEof: boolean;
  topic: string;
  retryStrategy: string;
  retryTopic: string;
  delay: number;
}