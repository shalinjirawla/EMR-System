import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEmergencyCaseComponent } from './create-emergency-case.component';

describe('CreateEmergencyCaseComponent', () => {
  let component: CreateEmergencyCaseComponent;
  let fixture: ComponentFixture<CreateEmergencyCaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateEmergencyCaseComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateEmergencyCaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
