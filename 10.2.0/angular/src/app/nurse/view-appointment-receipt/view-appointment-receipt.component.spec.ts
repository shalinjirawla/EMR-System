import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAppointmentReceiptComponent } from './view-appointment-receipt.component';

describe('ViewAppointmentReceiptComponent', () => {
  let component: ViewAppointmentReceiptComponent;
  let fixture: ComponentFixture<ViewAppointmentReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewAppointmentReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewAppointmentReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
