import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabTestReceiptComponent } from './lab-test-receipt.component';

describe('LabTestReceiptComponent', () => {
  let component: LabTestReceiptComponent;
  let fixture: ComponentFixture<LabTestReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabTestReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabTestReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
