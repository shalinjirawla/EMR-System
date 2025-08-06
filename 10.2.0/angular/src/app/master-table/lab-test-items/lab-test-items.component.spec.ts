import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabTestItemsComponent } from './lab-test-items.component';

describe('LabTestItemsComponent', () => {
  let component: LabTestItemsComponent;
  let fixture: ComponentFixture<LabTestItemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabTestItemsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabTestItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
