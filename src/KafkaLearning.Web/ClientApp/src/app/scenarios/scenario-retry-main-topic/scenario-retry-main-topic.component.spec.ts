import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScenarioRetryMainTopicComponent } from './scenario-retry-main-topic.component';

describe('ScenarioRetryMainTopicComponent', () => {
  let component: ScenarioRetryMainTopicComponent;
  let fixture: ComponentFixture<ScenarioRetryMainTopicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScenarioRetryMainTopicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScenarioRetryMainTopicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
