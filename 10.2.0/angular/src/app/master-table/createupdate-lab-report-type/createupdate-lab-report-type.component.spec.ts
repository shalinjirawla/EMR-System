import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateLabReportTypeComponent } from './createupdate-lab-report-type.component';

describe('CreateupdateLabReportTypeComponent', () => {
  let component: CreateupdateLabReportTypeComponent;
  let fixture: ComponentFixture<CreateupdateLabReportTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateLabReportTypeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateLabReportTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
