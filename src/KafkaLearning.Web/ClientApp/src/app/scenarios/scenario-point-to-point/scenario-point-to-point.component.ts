import { Component, OnInit, ViewChildren, QueryList } from '@angular/core';
import { ListenerComponent } from '../../listener/listener.component';

@Component({
  selector: 'app-scenario-point-to-point',
  templateUrl: './scenario-point-to-point.component.html',
  styleUrls: ['./scenario-point-to-point.component.css']
})
export class ScenarioPointToPointComponent implements OnInit {

  public static FOLDER: string = "scenario-point-to-point";
  public static TITLE: string = "Point to point";
  public static TITLE_PT_BR: string = "Point to point";

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
