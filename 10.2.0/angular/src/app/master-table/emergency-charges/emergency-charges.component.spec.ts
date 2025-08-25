import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmergencyChargesComponent } from './emergency-charges.component';

describe('EmergencyChargesComponent', () => {
  let component: EmergencyChargesComponent;
  let fixture: ComponentFixture<EmergencyChargesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmergencyChargesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmergencyChargesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
