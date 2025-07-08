import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOrderMedicineComponent } from './create-order-medicine.component';

describe('CreateOrderMedicineComponent', () => {
  let component: CreateOrderMedicineComponent;
  let fixture: ComponentFixture<CreateOrderMedicineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateOrderMedicineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateOrderMedicineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
