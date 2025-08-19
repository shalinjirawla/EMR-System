import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewReceiptComponent } from './view-receipt.component';

describe('ViewReceiptComponent', () => {
  let component: ViewReceiptComponent;
  let fixture: ComponentFixture<ViewReceiptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewReceiptComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
