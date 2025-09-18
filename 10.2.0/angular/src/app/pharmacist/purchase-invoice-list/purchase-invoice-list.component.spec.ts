import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseInvoiceListComponent } from './purchase-invoice-list.component';

describe('PurchaseInvoiceListComponent', () => {
  let component: PurchaseInvoiceListComponent;
  let fixture: ComponentFixture<PurchaseInvoiceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseInvoiceListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurchaseInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
