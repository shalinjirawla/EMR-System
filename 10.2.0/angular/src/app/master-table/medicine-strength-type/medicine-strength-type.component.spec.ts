import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineStrengthTypeComponent } from './medicine-strength-type.component';

describe('MedicineStrengthTypeComponent', () => {
  let component: MedicineStrengthTypeComponent;
  let fixture: ComponentFixture<MedicineStrengthTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicineStrengthTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicineStrengthTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
