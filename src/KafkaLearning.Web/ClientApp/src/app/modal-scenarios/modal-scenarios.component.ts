import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { ScenarioRetryMainTopicComponent } from '../scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ScenarioRetryNextTopicComponent } from '../scenarios/scenario-retry-next-topic/scenario-retry-next-topic.component';

@Component({
  selector: 'app-modal-scenarios',
  templateUrl: './modal-scenarios.component.html',
  styleUrls: ['./modal-scenarios.component.css']
})
export class ModalScenariosComponent implements OnInit {
  private tabs: { [id: string] : boolean } = {};
  private ScenarioRetryMainTopicComponent = ScenarioRetryMainTopicComponent;
  private ScenarioRetryNextTopicComponent = ScenarioRetryNextTopicComponent;

  @Output()
  close: EventEmitter<any> = new EventEmitter();

  @Output()
  change: EventEmitter<any> = new EventEmitter();


  constructor() {
    
  }

  ngOnInit() {    
    this.activeTab(localStorage.getItem('currentScenario'));
  }

  activeTab(name: string) {
    for(var i in this.tabs)
      this.tabs[i] = false;

    this.tabs[name] = true;

  }

  closeModal() {
    this.close.emit();
  }

  changeScenario() {
    for(var i in this.tabs)
      if (this.tabs[i])
        this.change.emit(i);
  }
}
