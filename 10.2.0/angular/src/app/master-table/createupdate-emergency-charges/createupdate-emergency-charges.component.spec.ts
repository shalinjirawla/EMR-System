import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateEmergencyChargesComponent } from './createupdate-emergency-charges.component';

describe('CreateupdateEmergencyChargesComponent', () => {
  let component: CreateupdateEmergencyChargesComponent;
  let fixture: ComponentFixture<CreateupdateEmergencyChargesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateEmergencyChargesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateEmergencyChargesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
