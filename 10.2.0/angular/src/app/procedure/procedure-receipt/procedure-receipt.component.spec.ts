import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcedureReceiptComponent } from './procedure-receipt.component';

describe('ProcedureReceiptComponent', () => {
  let component: ProcedureReceiptComponent;
  let fixture: ComponentFixture<ProcedureReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcedureReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProcedureReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
