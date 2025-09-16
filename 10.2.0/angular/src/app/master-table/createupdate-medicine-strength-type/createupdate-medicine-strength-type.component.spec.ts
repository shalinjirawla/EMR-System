import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateMedicineStrengthTypeComponent } from './createupdate-medicine-strength-type.component';

describe('CreateupdateMedicineStrengthTypeComponent', () => {
  let component: CreateupdateMedicineStrengthTypeComponent;
  let fixture: ComponentFixture<CreateupdateMedicineStrengthTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateMedicineStrengthTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateMedicineStrengthTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
