import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmergencyCaseComponent } from './emergency-case.component';

describe('EmergencyCaseComponent', () => {
  let component: EmergencyCaseComponent;
  let fixture: ComponentFixture<EmergencyCaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmergencyCaseComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmergencyCaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
