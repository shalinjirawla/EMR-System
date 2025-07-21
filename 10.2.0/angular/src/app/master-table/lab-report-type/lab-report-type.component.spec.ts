import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabReportTypeComponent } from './lab-report-type.component';

describe('LabReportTypeComponent', () => {
  let component: LabReportTypeComponent;
  let fixture: ComponentFixture<LabReportTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabReportTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabReportTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
