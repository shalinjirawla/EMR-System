import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateProcedureReceiptComponent } from './create-procedure-receipt.component';

describe('CreateProcedureReceiptComponent', () => {
  let component: CreateProcedureReceiptComponent;
  let fixture: ComponentFixture<CreateProcedureReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateProcedureReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateProcedureReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
