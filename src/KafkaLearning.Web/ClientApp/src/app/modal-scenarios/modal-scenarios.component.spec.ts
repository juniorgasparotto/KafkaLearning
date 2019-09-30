import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalScenariosComponent } from './modal-scenarios.component';

describe('ModalScenariosComponent', () => {
  let component: ModalScenariosComponent;
  let fixture: ComponentFixture<ModalScenariosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalScenariosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalScenariosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
