import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateDoctorMasterComponent } from './createupdate-doctor-master.component';

describe('CreateupdateDoctorMasterComponent', () => {
  let component: CreateupdateDoctorMasterComponent;
  let fixture: ComponentFixture<CreateupdateDoctorMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateDoctorMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateDoctorMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
