import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUpdateTriageComponent } from './create-update-triage.component';

describe('CreateUpdateTriageComponent', () => {
  let component: CreateUpdateTriageComponent;
  let fixture: ComponentFixture<CreateUpdateTriageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateUpdateTriageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateUpdateTriageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
