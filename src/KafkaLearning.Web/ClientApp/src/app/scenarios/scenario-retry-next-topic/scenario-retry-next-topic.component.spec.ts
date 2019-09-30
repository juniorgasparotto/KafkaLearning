import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScenarioRetryNextTopicComponent } from './scenario-retry-next-topic.component';

describe('ScenarioRetryNextTopicComponent', () => {
  let component: ScenarioRetryNextTopicComponent;
  let fixture: ComponentFixture<ScenarioRetryNextTopicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScenarioRetryNextTopicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScenarioRetryNextTopicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
