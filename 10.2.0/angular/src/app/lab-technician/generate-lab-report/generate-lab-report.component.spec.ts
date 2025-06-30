import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateLabReportComponent } from './generate-lab-report.component';

describe('GenerateLabReportComponent', () => {
  let component: GenerateLabReportComponent;
  let fixture: ComponentFixture<GenerateLabReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenerateLabReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GenerateLabReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
