import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewLabReportComponent } from './view-lab-report.component';

describe('ViewLabReportComponent', () => {
  let component: ViewLabReportComponent;
  let fixture: ComponentFixture<ViewLabReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewLabReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewLabReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
