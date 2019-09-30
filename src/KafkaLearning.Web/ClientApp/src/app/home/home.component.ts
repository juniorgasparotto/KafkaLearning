import { Component, Inject, OnInit, ComponentFactoryResolver, ViewContainerRef, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ScenarioRetryMainTopicComponent } from '../scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ScenarioRetryNextTopicComponent } from '../scenarios/scenario-retry-next-topic/scenario-retry-next-topic.component';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild('scenarios', { read: ViewContainerRef })
  viewContainerRefScenarios: ViewContainerRef

  public message: string;
  public publisherSettigns: string;
  public viewLogs: boolean;
  public viewScenarios: boolean;
  private title: string;

  private scenarios: any[] = new Array<any>();
  private currentScenario: any;

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    @Inject(ComponentFactoryResolver) private factoryResolver
  ) {
    this.scenarios.push(ScenarioRetryMainTopicComponent);
    this.scenarios.push(ScenarioRetryMainTopicComponent);
  }

  ngOnInit(): void {
    this.publisherSettigns = JSON.stringify({
      "BootstrapServers": "localhost:9092",
      "Topic": "Chat"
    }, null, 2);

    var scenario = localStorage.getItem('currentScenario');
    if (!scenario) {
      scenario = ScenarioRetryMainTopicComponent.NAME;
    }   

    this.changeScenario(scenario);
  }

  addScenarioComponent(componentScenario: any) {
    this.title = componentScenario.TITLE;
    this.viewContainerRefScenarios.clear();

    const factory = this.factoryResolver.resolveComponentFactory(componentScenario);
    const component = factory.create(this.viewContainerRefScenarios.parentInjector);
    this.viewContainerRefScenarios.insert(component.hostView);
    this.currentScenario = component._component;
  }

  sendMessage() {
    var request = {
      settings: JSON.parse(this.publisherSettigns),
      message: {
        message: this.message
      }
    };

    this.http.post<Boolean>(this.baseUrl + `api/Server/Send`, request).subscribe(
      result => {

      },
      error => {
        console.error(error);
      });
  }

  public subscribeAll() {
    this.currentScenario.subscribeAll();
  }

  public unSubscribeAll() {
    this.currentScenario.unSubscribeAll();
  }

  public clearAll() {
    this.currentScenario.clearAll();  
  }

  showLogs() {
    this.viewLogs = true;
  }

  closeLogs() {
    this.viewLogs = false;
  }

  openScenarios() {
    this.viewScenarios = true;
  }

  closeScenarios() {
    this.viewScenarios = false;
  }

  changeScenario(scenario: string) {
    if (this.currentScenario && this.currentScenario.hasSomeSubscribe()) {
      alert("Stop all listeners to change.");
      return;
    }

    this.closeScenarios();

    localStorage.setItem('currentScenario', scenario);
    
    switch (scenario) {
      case ScenarioRetryNextTopicComponent.NAME:
        this.addScenarioComponent(ScenarioRetryNextTopicComponent);
        break;
      case ScenarioRetryMainTopicComponent.NAME:
        this.addScenarioComponent(ScenarioRetryMainTopicComponent);
        break;
    }
  }
}
