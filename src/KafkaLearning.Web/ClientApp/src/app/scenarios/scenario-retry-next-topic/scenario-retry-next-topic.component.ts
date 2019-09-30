import { Component, OnInit, ViewChild, QueryList, ViewChildren } from '@angular/core';
import { ListenerComponent } from '../../listener/listener.component';

@Component({
  selector: 'app-scenario-retry-next-topic',
  templateUrl: './scenario-retry-next-topic.component.html',
  styleUrls: ['./scenario-retry-next-topic.component.css']
})
export class ScenarioRetryNextTopicComponent implements OnInit {
  public static NAME: string = "ScenarioRetryNextTopicComponent";
  public static TITLE: string = "Retry: Always go to next retry topic until DLQ";

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
