import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateAppointmentTypesComponent } from './createupdate-appointment-types.component';

describe('CreateupdateAppointmentTypesComponent', () => {
  let component: CreateupdateAppointmentTypesComponent;
  let fixture: ComponentFixture<CreateupdateAppointmentTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateAppointmentTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateAppointmentTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
