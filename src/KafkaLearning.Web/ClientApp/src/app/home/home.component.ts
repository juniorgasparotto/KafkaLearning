import { Component, Inject, OnInit, ComponentFactoryResolver, ViewContainerRef, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ModalScenariosComponent } from '../modal-scenarios/modal-scenarios.component';
import { ScenarioRetryMainTopicComponent } from '../scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ScenarioPointToPointComponent } from '../scenarios/scenario-point-to-point/scenario-point-to-point.component';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild('scenarios', { read: ViewContainerRef })
  viewContainerRefScenarios: ViewContainerRef

  public message: string;
  public producerDefault: string;
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
    this.setConfigs();

    var scenario = ModalScenariosComponent.getComponentByName(localStorage.getItem('currentScenario'));
    if (!scenario) {
      scenario = ModalScenariosComponent.getComponentByName('ScenarioPointToPointComponent');
    }

    this.changeScenario(scenario);
  }

  private setConfigs() {
    this.http.get<any>(this.baseUrl + `api/Server/GetConfigs`).subscribe(
      result => {
        var config = Object.assign({}, environment.kafka.producerDefault);
        config.bootstrapServers = result.producers.topicSample.bootstrapServers;
        this.producerDefault = JSON.stringify(config, null, 2);
      },
      error => {
        console.error(error);
      }
    );
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
      settings: JSON.parse(this.producerDefault),
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
    this.addScenarioComponent(scenarioComponent.component);
    localStorage.setItem('currentScenario', scenarioComponent.name);
  }
}
