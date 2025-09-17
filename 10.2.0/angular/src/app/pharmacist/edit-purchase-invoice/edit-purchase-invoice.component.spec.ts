import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPurchaseInvoiceComponent } from './edit-purchase-invoice.component';

describe('EditPurchaseInvoiceComponent', () => {
  let component: EditPurchaseInvoiceComponent;
  let fixture: ComponentFixture<EditPurchaseInvoiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPurchaseInvoiceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPurchaseInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
