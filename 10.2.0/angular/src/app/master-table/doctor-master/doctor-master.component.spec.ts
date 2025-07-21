import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorMasterComponent } from './doctor-master.component';

describe('DoctorMasterComponent', () => {
  let component: DoctorMasterComponent;
  let fixture: ComponentFixture<DoctorMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
