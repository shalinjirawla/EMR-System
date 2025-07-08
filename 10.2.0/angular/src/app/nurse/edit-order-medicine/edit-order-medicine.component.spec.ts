import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditOrderMedicineComponent } from './edit-order-medicine.component';

describe('EditOrderMedicineComponent', () => {
  let component: EditOrderMedicineComponent;
  let fixture: ComponentFixture<EditOrderMedicineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditOrderMedicineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditOrderMedicineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
