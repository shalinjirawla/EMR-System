import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientReceiptListComponent } from './patient-receipt-list.component';

describe('PatientReceiptListComponent', () => {
  let component: PatientReceiptListComponent;
  let fixture: ComponentFixture<PatientReceiptListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientReceiptListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientReceiptListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
