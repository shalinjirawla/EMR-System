import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditLabReportComponent } from './edit-lab-report.component';

describe('EditLabReportComponent', () => {
  let component: EditLabReportComponent;
  let fixture: ComponentFixture<EditLabReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditLabReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditLabReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
