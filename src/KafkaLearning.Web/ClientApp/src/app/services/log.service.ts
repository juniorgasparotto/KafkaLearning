import { Injectable } from '@angular/core';
import { ILogger, LogLevel, HubConnectionBuilder } from "@aspnet/signalr";

@Injectable()
export class LogService implements ILogger {
    log(logLevel: LogLevel, message: string) {      
        console.log("TESTE:", logLevel, message);
    }
}