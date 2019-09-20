import { Component, OnInit, Input } from '@angular/core';
import { LogEvent } from '../log-viewer/log-viewer.component';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent implements OnInit {
  private ERROR: string = 'error';
  private INFO: string = 'info';

  @Input()
  public log: LogEvent;

  @Input()
  public odd: boolean;
  
  constructor() { }

  ngOnInit() {
  }

  // public add(log: Log) {
  //   this.logs.push(log);
  // }

  // public error(msg: string) {
  //   var log: Log = {
  //     date: new Date(),
  //     type: this.ERROR,
  //     message: msg
  //   };

  //   this.add(log);
  // }

  // public info(msg: string) {
  //   var log: Log = {
  //     date: new Date(),
  //     type: this.INFO,
  //     message: msg
  //   };

  //   this.add(log);
  // }

}

// export interface Log {
//   date: Date;
//   message: string;
//   type: string;
// }
