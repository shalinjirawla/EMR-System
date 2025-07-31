import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLabReportComponent } from './create-lab-report.component';

describe('CreateLabReportComponent', () => {
  let component: CreateLabReportComponent;
  let fixture: ComponentFixture<CreateLabReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateLabReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateLabReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
