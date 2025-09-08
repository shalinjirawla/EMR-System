import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDepositTransactionComponent } from './view-deposit-transaction.component';

describe('ViewDepositTransactionComponent', () => {
  let component: ViewDepositTransactionComponent;
  let fixture: ComponentFixture<ViewDepositTransactionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewDepositTransactionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewDepositTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
