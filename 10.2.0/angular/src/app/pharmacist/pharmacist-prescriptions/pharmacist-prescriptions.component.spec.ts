import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacistPrescriptionsComponent } from './pharmacist-prescriptions.component';

describe('PharmacistPrescriptionsComponent', () => {
  let component: PharmacistPrescriptionsComponent;
  let fixture: ComponentFixture<PharmacistPrescriptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacistPrescriptionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacistPrescriptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
