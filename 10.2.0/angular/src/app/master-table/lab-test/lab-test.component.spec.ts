import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabTestComponent } from './lab-test.component';

describe('LabTestComponent', () => {
  let component: LabTestComponent;
  let fixture: ComponentFixture<LabTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabTestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
