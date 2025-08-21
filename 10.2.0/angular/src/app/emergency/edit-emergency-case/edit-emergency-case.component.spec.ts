import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEmergencyCaseComponent } from './edit-emergency-case.component';

describe('EditEmergencyCaseComponent', () => {
  let component: EditEmergencyCaseComponent;
  let fixture: ComponentFixture<EditEmergencyCaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditEmergencyCaseComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditEmergencyCaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
