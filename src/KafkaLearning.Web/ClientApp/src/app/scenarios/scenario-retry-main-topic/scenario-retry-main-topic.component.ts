import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { ListenerComponent } from '../../listener/listener.component';

@Component({
  selector: 'app-scenario-retry-main-topic',
  templateUrl: './scenario-retry-main-topic.component.html',
  styleUrls: ['./scenario-retry-main-topic.component.css']
})
export class ScenarioRetryMainTopicComponent implements OnInit {
  public static FOLDER: string = "scenario-retry-main-topic";
  public static TITLE: string = "Retry: Always goes back to the main topic and in case of error sends to the next error topic until arriving at DQL";
  public static TITLE_PT_BR: string = "Retry: Sempre volta ao tópico principal e, em caso de erro, envia para o próximo tópico de erro até chegar ao DQL";

  @ViewChildren(ListenerComponent)
  listeners: QueryList<ListenerComponent>;

  constructor() { }

  ngOnInit() {
  }

  public subscribeAll() {
    var array = this.listeners.toArray();
    for (var i in array)
      array[i].subscribe();
  }

  public unSubscribeAll() {
    var array = this.listeners.toArray();
    for (var i in array)
      array[i].unSubscribe();
  }

  public hasSomeSubscribe() {
    var array = this.listeners.toArray();
    for (var i in array)
      if (array[i].isSubscribe())
        return true;

    return false;
  }

  public clearAll() {
    var array = this.listeners.toArray();
    for (var i in array)
      array[i].clear();
  }
}
