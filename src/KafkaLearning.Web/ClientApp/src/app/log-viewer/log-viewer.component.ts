import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.css']
})
export class LogViewerComponent implements OnInit {
  public logs: Array<LogEvent>;
  private hubConnection: signalR.HubConnection;
  public loading: boolean;

  @Output()
  close: EventEmitter<any> = new EventEmitter();

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.logs = new Array<LogEvent>();
  }

  ngOnInit() {
    // this.http.get(this.baseUrl + `api/Server/SubscribeLogs`).subscribe(
    //   result => {
    //     this.listenLogs();
    //   },
    //   error => {
    //     console.error(error);
    //   }
    // );

    this.update();
  }

  public update() {
    this.loading = true;
    this.http.get<Array<LogEvent>>(this.baseUrl + `api/Server/GetAllLogs`).subscribe(result => {
      this.logs = result;
      this.loading = false;
    }, error => {
      alert("LOG ERROR: " + JSON.stringify(error));
      console.error(error);
      this.loading = false;
    });
  }

  private listenLogs() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/signalr/logs')
      .build();

    this.hubConnection.onclose(error => {
      console.log(error);
    });

    this.hubConnection.start()
      .then(() => {
        this.hubConnection.on('add', (msg: LogEvent) => {
          console.log(msg);
          this.logs.push(msg);
          this.logs = this.logs.slice();
        });
      })
      .catch(err => {
        this.addError(err);
      });
  }

  closeViewer() {
    try {
      this.hubConnection
        .stop()
        .catch(error => {
          this.addError(error);
        });
    }
    catch (e) {
      this.addError(e);
    }

    this.close.emit();
  }

  private addError(err) {
    var le: LogEvent = {
      exception: err,
      level: 1,
      properties: {
        TaskId: {
          value: 0
        },
        ThreadId: {
          value: 0
        }
      },
      messageTemplate: {
        text: err.message
      },
      timestamp: new Date()
    };
    this.logs.push(le);
  }

}

export interface LogEvent {
  timestamp: Date,
  level: number,
  messageTemplate: {
    text: string
  },
  properties: {
    ThreadId: {
      value: number
    },
    TaskId: {
      value: number
    }
  },
  exception: any
}
