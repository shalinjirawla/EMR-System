import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiptDetailComponent } from './receipt-detail.component';

describe('ReceiptDetailComponent', () => {
  let component: ReceiptDetailComponent;
  let fixture: ComponentFixture<ReceiptDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceiptDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReceiptDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
