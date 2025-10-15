import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsuranceClaimComponent } from './insurance-claim.component';

describe('InsuranceClaimComponent', () => {
  let component: InsuranceClaimComponent;
  let fixture: ComponentFixture<InsuranceClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsuranceClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InsuranceClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
