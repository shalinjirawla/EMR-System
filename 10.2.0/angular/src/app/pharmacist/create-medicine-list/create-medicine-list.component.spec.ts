import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMedicineListComponent } from './create-medicine-list.component';

describe('CreateMedicineListComponent', () => {
  let component: CreateMedicineListComponent;
  let fixture: ComponentFixture<CreateMedicineListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateMedicineListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateMedicineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
