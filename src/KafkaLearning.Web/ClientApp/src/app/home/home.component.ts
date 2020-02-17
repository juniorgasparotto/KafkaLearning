import { Component, Inject, OnInit, ComponentFactoryResolver, ViewContainerRef, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ModalScenariosComponent } from '../modal-scenarios/modal-scenarios.component';
import { ScenarioRetryMainTopicComponent } from '../scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ScenarioPointToPointComponent } from '../scenarios/scenario-point-to-point/scenario-point-to-point.component';

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
  public title: string;

  private currentScenario: any;

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    @Inject(ComponentFactoryResolver) private factoryResolver
  ) {
  }

  ngOnInit(): void {
    this.publisherSettigns = JSON.stringify({
      "BootstrapServers": "my-cluster-kafka-bootstrap-project-kafka.192.168.0.10.nip.io:443",
      "Topic": "Chat"
    }, null, 2);

    var scenario = ModalScenariosComponent.getComponentByName(localStorage.getItem('currentScenario'));
    if (!scenario) {
      scenario = ScenarioPointToPointComponent;
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

  changeScenario(scenarioComponent: any) {
    if (this.currentScenario && this.currentScenario.hasSomeSubscribe()) {
      alert("Stop all listeners to change.");
      return;
    }

    this.closeScenarios();
    this.addScenarioComponent(scenarioComponent);
    localStorage.setItem('currentScenario', scenarioComponent.name);
  }
}
