import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateEmergencyProcedureComponent } from './createupdate-emergency-procedure.component';

describe('CreateupdateEmergencyProcedureComponent', () => {
  let component: CreateupdateEmergencyProcedureComponent;
  let fixture: ComponentFixture<CreateupdateEmergencyProcedureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateEmergencyProcedureComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateEmergencyProcedureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
