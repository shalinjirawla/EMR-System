import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPrescriptionsComponent } from './edit-prescriptions.component';

describe('EditPrescriptionsComponent', () => {
  let component: EditPrescriptionsComponent;
  let fixture: ComponentFixture<EditPrescriptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPrescriptionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPrescriptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
