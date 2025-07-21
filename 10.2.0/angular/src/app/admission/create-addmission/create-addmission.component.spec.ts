import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateAddmissionComponent } from './create-addmission.component';

describe('CreateAddmissionComponent', () => {
  let component: CreateAddmissionComponent;
  let fixture: ComponentFixture<CreateAddmissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateAddmissionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateAddmissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
