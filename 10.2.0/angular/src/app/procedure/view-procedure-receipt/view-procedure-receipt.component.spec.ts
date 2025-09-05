import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewProcedureReceiptComponent } from './view-procedure-receipt.component';

describe('ViewProcedureReceiptComponent', () => {
  let component: ViewProcedureReceiptComponent;
  let fixture: ComponentFixture<ViewProcedureReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewProcedureReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewProcedureReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
