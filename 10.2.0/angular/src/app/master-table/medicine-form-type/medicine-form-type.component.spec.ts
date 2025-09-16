import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineFormTypeComponent } from './medicine-form-type.component';

describe('MedicineFormTypeComponent', () => {
  let component: MedicineFormTypeComponent;
  let fixture: ComponentFixture<MedicineFormTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicineFormTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicineFormTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
