import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditInsuranceClaimComponent } from './edit-insurance-claim.component';

describe('EditInsuranceClaimComponent', () => {
  let component: EditInsuranceClaimComponent;
  let fixture: ComponentFixture<EditInsuranceClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditInsuranceClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditInsuranceClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
