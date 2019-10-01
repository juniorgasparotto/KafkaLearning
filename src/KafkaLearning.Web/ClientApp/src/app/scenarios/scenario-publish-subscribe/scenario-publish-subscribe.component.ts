import { Component, OnInit, ViewChildren, QueryList } from '@angular/core';
import { ListenerComponent } from '../../listener/listener.component';

@Component({
  selector: 'app-scenario-publish-subscribe',
  templateUrl: './scenario-publish-subscribe.component.html',
  styleUrls: ['./scenario-publish-subscribe.component.css']
})
export class ScenarioPublishSubscribeComponent implements OnInit {

  public static FOLDER: string = "scenario-publish-subscribe";
  public static TITLE: string = "Publish/Subscribe";
  public static TITLE_PT_BR: string = "Publish/Subscribe";

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
