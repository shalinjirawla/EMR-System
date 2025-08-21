import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmergencyTriageComponent } from './emergency-triage.component';

describe('EmergencyTriageComponent', () => {
  let component: EmergencyTriageComponent;
  let fixture: ComponentFixture<EmergencyTriageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmergencyTriageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmergencyTriageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
