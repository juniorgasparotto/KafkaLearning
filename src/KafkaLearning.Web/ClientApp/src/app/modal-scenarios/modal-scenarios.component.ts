import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { ScenarioRetryMainTopicComponent } from '../scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ScenarioRetryNextTopicComponent } from '../scenarios/scenario-retry-next-topic/scenario-retry-next-topic.component';
import { ScenarioPointToPointComponent } from '../scenarios/scenario-point-to-point/scenario-point-to-point.component';
import { ScenarioPublishSubscribeComponent } from '../scenarios/scenario-publish-subscribe/scenario-publish-subscribe.component';

@Component({
  selector: 'app-modal-scenarios',
  templateUrl: './modal-scenarios.component.html',
  styleUrls: ['./modal-scenarios.component.css']
})
export class ModalScenariosComponent implements OnInit {
  private static TABS: any[] = [
    { name: 'ScenarioPointToPointComponent', component: ScenarioPointToPointComponent, active: false },
    { name: 'ScenarioPublishSubscribeComponent', component: ScenarioPublishSubscribeComponent, active: false },
    { name: 'ScenarioRetryMainTopicComponent', component: ScenarioRetryMainTopicComponent, active: false },
    { name: 'ScenarioRetryNextTopicComponent', component: ScenarioRetryNextTopicComponent, active: false },
  ];

  public tabs: any[] = ModalScenariosComponent.TABS;
  private currentTab: any;
  public lang: string;

  @Output()
  close: EventEmitter<any> = new EventEmitter();

  @Output()
  change: EventEmitter<any> = new EventEmitter();

  ngOnInit() {
    for (var t in this.tabs)
      if (this.tabs[t].name == localStorage.getItem('currentScenario'))
        this.activeTab(this.tabs[t]);
  }

  static getComponentByName(name: string): any {
    for (var t in ModalScenariosComponent.TABS)
      if (ModalScenariosComponent.TABS[t].name == name)
        return ModalScenariosComponent.TABS[t];

    return null;
  }

  activeTab(tab: any) {
    for (var i in this.tabs) {
      this.tabs[i].active = false;
    }

    this.currentTab = tab;
    this.currentTab.active = true;
  }

  isActiveTab(tab: any) {
    return tab.active;
  }

  getTabTitle(tab: any, lang: string) {
    console.log(tab)
    switch (lang) {
      case 'pt-br':
        return tab.component.TITLE_PT_BR;
    }

    return tab.component.TITLE;
  }

  getCurrentTabTitle(lang: string) {
    return this.getTabTitle(this.currentTab, lang);
  }

  getCurrentTabUrlDescription(lang: string) {
    return 'app/scenarios/' + this.currentTab.component.FOLDER + '/description' + (lang ? '-' + lang : '') + '.html';
  }

  changeScenario() {
    this.change.emit(this.currentTab);
  }

  closeModal() {
    this.close.emit();
  }
}
