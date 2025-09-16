import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateMedicineFormTypeComponent } from './createupdate-medicine-form-type.component';

describe('CreateupdateMedicineFormTypeComponent', () => {
  let component: CreateupdateMedicineFormTypeComponent;
  let fixture: ComponentFixture<CreateupdateMedicineFormTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateMedicineFormTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateMedicineFormTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
