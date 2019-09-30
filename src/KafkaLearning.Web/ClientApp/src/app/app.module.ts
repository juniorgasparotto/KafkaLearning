import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ListenerComponent } from './listener/listener.component';
import { LogComponent } from './log/log.component';
import { LogService } from './services/log.service';
import { LogViewerComponent } from './log-viewer/log-viewer.component';
import { NgPipesModule } from 'ngx-pipes';
import { ScenarioRetryNextTopicComponent } from './scenarios/scenario-retry-next-topic/scenario-retry-next-topic.component';
import { ScenarioRetryMainTopicComponent } from './scenarios/scenario-retry-main-topic/scenario-retry-main-topic.component';
import { ModalScenariosComponent } from './modal-scenarios/modal-scenarios.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ListenerComponent,
    LogComponent,
    LogViewerComponent,
    ModalScenariosComponent,
    ScenarioRetryNextTopicComponent,
    ScenarioRetryMainTopicComponent,
  ],
  entryComponents: [
    ScenarioRetryNextTopicComponent,
    ScenarioRetryMainTopicComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgPipesModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    ])
  ],
  providers: [LogService],
  bootstrap: [AppComponent]
})
export class AppModule { }
