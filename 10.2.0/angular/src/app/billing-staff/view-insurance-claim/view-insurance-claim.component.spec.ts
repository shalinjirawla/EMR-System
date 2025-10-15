import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewInsuranceClaimComponent } from './view-insurance-claim.component';

describe('ViewInsuranceClaimComponent', () => {
  let component: ViewInsuranceClaimComponent;
  let fixture: ComponentFixture<ViewInsuranceClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewInsuranceClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewInsuranceClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
