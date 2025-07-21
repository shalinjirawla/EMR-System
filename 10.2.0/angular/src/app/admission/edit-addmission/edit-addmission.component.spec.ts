import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAddmissionComponent } from './edit-addmission.component';

describe('EditAddmissionComponent', () => {
  let component: EditAddmissionComponent;
  let fixture: ComponentFixture<EditAddmissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditAddmissionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditAddmissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
