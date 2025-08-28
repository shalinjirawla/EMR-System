import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmergencyProcedureComponent } from './emergency-procedure.component';

describe('EmergencyProcedureComponent', () => {
  let component: EmergencyProcedureComponent;
  let fixture: ComponentFixture<EmergencyProcedureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmergencyProcedureComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmergencyProcedureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
