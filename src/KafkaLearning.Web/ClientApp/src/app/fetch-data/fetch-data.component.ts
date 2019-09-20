import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css'],
})
export class FetchDataComponent implements OnInit {
  public message: string;
  public messages: ChatMessage[];
  private hubConnection: signalR.HubConnection;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.messages = new Array<ChatMessage>();
  }

  ngOnInit(): void {
    //this.pollUntilTaskFinished();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/signalr/chat', {
        // skipNegotiation: true,
        // transport: signalR.HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection.start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));

    this.hubConnection.on('chat', (data) => {
      console.log(data);
    });

  }

  pollUntilTaskFinished() {
    this.http.get<ChatMessage>(this.baseUrl + 'api/Chat/GetAll').subscribe(
      result => {

        for (var i in result)
          if (result[i].message)
            this.messages.push(result[i]);

        setTimeout(() => this.pollUntilTaskFinished(), 500);
      },
      error => {
        console.error(error);
      }
    );
  }

  public send() {
    this.sendMessage(this.message);
  }

  private sendMessage(_message: string) {
    var request = {
      message: _message
    };

    this.http.post<Boolean>(this.baseUrl + 'api/Chat/Send', request).subscribe(result => {
      alert(result);
    }, error => console.error(error));
  }

}

interface ChatMessage {
  id: string;
  date: Date;
  message: string;
}
