import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMedicineListComponent } from './edit-medicine-list.component';

describe('EditMedicineListComponent', () => {
  let component: EditMedicineListComponent;
  let fixture: ComponentFixture<EditMedicineListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditMedicineListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditMedicineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
